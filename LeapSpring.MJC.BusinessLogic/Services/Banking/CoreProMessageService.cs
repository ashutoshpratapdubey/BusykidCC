using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto.Banking;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public class CoreProMessageService : ServiceBase, ICoreProMessageService
    {
        private IAppSettingsService _appSettings;
        private ITransactionService _transactionService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="appSettings">The app settings service.</param>
        /// <param name="transactionService">The transaction service.</param>
        public CoreProMessageService(IRepository repository, IAppSettingsService appSettings, ITransactionService transactionService) : base(repository)
        {
            _appSettings = appSettings;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Receives the message from corepro through azure service bus.
        /// </summary>


        public void ReceiveMessage()
        {
            //code by STPL
            List<MessageEvent> lstMessageEvent = new List<MessageEvent>();
            lstMessageEvent = readTransactionTxtFile();
            MessageEvent messageEvent = new MessageEvent();
            try
            {
                foreach (var item in lstMessageEvent)
                {
                    messageEvent.CustomerId = item.CustomerId;
                    messageEvent.TransactionId = item.TransactionId;
                    messageEvent.UserEventId = item.UserEventId;
                    messageEvent.UserEventType = item.UserEventType;
                    messageEvent.TransactionStatus = item.TransactionStatus;
                    messageEvent.TransactionReturnCode = item.TransactionReturnCode;
                    messageEvent.EventTime = item.EventTime;

                    //Insert EventStatus Log

                    var eventStatus = new EventStatusLog
                    {
                        TransactionID = item.TransactionId,
                        EventTypeStatus = item.UserEventType.ToString()
                    };

                    //AddEventStatusTypeLog(eventStatus);
                    ProcessTransaction(messageEvent);
                }
            }
            catch (Exception Ex)
            {

            }
        }


        //added by STPL
        private void LogError(string todayDate)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", todayDate);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", todayDate);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", todayDate);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", todayDate);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            //string path = "E:\\FTP File\\DataWrite1.txt";
            string path = "C:\\inetpub\\wwwroot\\CorePro\\DataWrite1.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
        public List<MessageEvent> readTransactionTxtFile()
        {
            List<MessageEvent> listOfPersons = new List<MessageEvent>();
            MessageEvent objSingleRecord = new MessageEvent();
            List<MessageEventEnum> objTxnStatus = new List<MessageEventEnum>();
            string strRemoteFileName = string.Empty;
            //try
            //{
            String Host = _appSettings.coreProSftpHost;
            int Port = Convert.ToInt32(_appSettings.coreProSftpPort);
            String RemoteFileName = _appSettings.coreProSftpRemoteFolder;
            String LocalDestinationFilename = _appSettings.CoreproTransactionDetailFilePath;
            String Username = _appSettings.coreProSftpUserName;
            String Password = _appSettings.coreProSftpPassword;



            //using (var sftp = new SftpClient(Host, Port, Username, Password))
            //{
            //    sftp.Connect();
            //    var files = sftp.ListDirectory(RemoteFileName).Where(file => file.LastWriteTimeUtc > DateTime.UtcNow.AddDays(-3)).OrderByDescending(file => file.LastWriteTime);
            //    int chkCounter = 0;
            //    foreach (var fileDetails in files)
            //    {
            //        if (chkCounter > 1)
            //        {
            //            strRemoteFileName = fileDetails.FullName;

            //            //Check Transaction File Existance

            //            var transactionFileExists = Repository.Table<TransactionStatusFileDetails>().Where(p => p.FileName == strRemoteFileName).FirstOrDefault();
            //            if (transactionFileExists == null)
            //            {
            //                var transactionStatus = new TransactionStatusFileDetails
            //                {
            //                    FileName = strRemoteFileName,
            //                    FileUsedDate = DateTime.UtcNow
            //                };
            //                AddTransactionStatusFileName(transactionStatus);

            //                //Copy File data from SFTP to loacl

            //                using (var file = File.OpenWrite(LocalDestinationFilename))
            //                {
            //                    sftp.DownloadFile(strRemoteFileName, file);
            //                }

            using (StreamReader sr = new StreamReader(_appSettings.CoreproTransactionDetailFilePath))
            {
                string line;
                int counter = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (counter != 0)
                    {

                        int TransactionID = string.IsNullOrEmpty(line.Substring(180, 19).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(180, 19).ToString().TrimStart('0'));

                        if (TransactionID != 0)
                            listOfPersons.Add(new MessageEvent
                            {
                                UserEventId = 0,
                                UserEventType = (MessageEventEnum)(string.IsNullOrEmpty(line.Substring(300, 9).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(300, 9).ToString().TrimStart('0'))),
                                TransactionId = string.IsNullOrEmpty(line.Substring(180, 19).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(180, 19).ToString().TrimStart('0')),
                                CustomerId = string.IsNullOrEmpty(line.Substring(0, 10).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(0, 10).ToString().TrimStart('0')),
                                TransactionStatus = "",
                                TransactionReturnCode = "",
                                EventTime = DateTime.UtcNow
                            });

                    }
                    counter++;
                }
            }

            //                    File.Delete(_appSettings.CoreproTransactionDetailFilePath);
            //                }
            //            }
            //            chkCounter++;
            //        }
            //        sftp.Disconnect();
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    string str1 = Ex.ToString();
            //}
            return listOfPersons;
        }

        public void AddTransactionStatusFileName(TransactionStatusFileDetails transactionStatus)
        {
            Repository.Insert(transactionStatus);
        }

        public void AddEventStatusTypeLog(EventStatusLog eventTypeLog)
        {
            Repository.Insert(eventTypeLog);
        }


        /// <summary>
        /// Process the bank transactions.
        /// </summary>
        /// <param name="messageEvent">The message event received from corepro.</param>
        /// <returns></returns>
        private void ProcessTransaction(MessageEvent messageEvent)
        {
            switch (messageEvent.UserEventType)
            {
                case MessageEventEnum.LockedExternalAccount:
                    break;
                case MessageEventEnum.UnlockedExternalAccount:
                    break;
                case MessageEventEnum.CreatedExternalAccount:
                    break;
                case MessageEventEnum.ModifiedExternalAccount:
                    break;
                case MessageEventEnum.VerifiedExternalAccount:
                    break;
                case MessageEventEnum.ResendVerifyExternalAccount:
                    break;
                case MessageEventEnum.VoidedAccountTransaction:
                case MessageEventEnum.VoidedExternalAccountTransaction:
                    _transactionService.MarkAsCancelled(messageEvent);
                    break;
                case MessageEventEnum.ReversedAccountTransaction:
                case MessageEventEnum.ReversedExternalAccountTransaction:
                    _transactionService.MarkAsFalied(messageEvent);
                    break;
                case MessageEventEnum.TransactionSettled:
                    _transactionService.MarkAsSettled(messageEvent);
                    break;
                case MessageEventEnum.TransactionAvailable:
                case MessageEventEnum.AddedFee_or_Credit_to_CustomerAccount:
                    _transactionService.MarkAsCompleted(messageEvent);
                    break;
                case MessageEventEnum.FundingSourceVerified:
                    break;
                case MessageEventEnum.CreatedCustomerAccount:
                    break;
                case MessageEventEnum.CreatedAccountTransaction:
                    _transactionService.MarkAsTransferCreated(messageEvent);
                    break;
            }
        }
    }
}
