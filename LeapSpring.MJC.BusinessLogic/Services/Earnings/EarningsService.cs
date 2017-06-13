using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Earnings;
using LeapSpring.MJC.Core.Domain.Settings;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using System.Web;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using LeapSpring.MJC.Core.Dto.Banking;
using Renci.SshNet;
using LeapSpring.MJC.Core.Domain;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Domain.Sms;

namespace LeapSpring.MJC.BusinessLogic.Services.Earnings
{
    /// <summary>
    /// Represents a earnings service
    /// </summary>
    public class EarningsService : ServiceBase, IEarningsService
    {
        private IBankService _bankService;
        private ITransactionService _transactionService;
        private ICurrentUserService _currentUserService;
        private ITextMessageService _textMessageService;
        private IAppSettingsService _appSettingsService;

        /// <param name="bankService">Bank authorize service</param>
        /// <param name="transactionService">Transaction service</param>
        public EarningsService(IRepository repository, IBankService bankService, ITransactionService transactionService,
            ICurrentUserService currentUserService, ITextMessageService textMessageService, IAppSettingsService appSettingsService) : base(repository)
        {
            _bankService = bankService;
            _transactionService = transactionService;
            _currentUserService = currentUserService;
            _textMessageService = textMessageService;
            _appSettingsService = appSettingsService;
        }

        #region Methods

        /// <summary>
        /// Add child earnings
        /// </summary>
        /// <param name="childEarnings">Child earnings</param>
        public void Add(ChildEarnings childEarnings)
        {
            Repository.Insert(childEarnings);
        }

        /// <summary>
        /// Update child earnings
        /// </summary>
        /// <param name="childEarnings">Child earnings</param>
        public void Update(ChildEarnings childEarnings)
        {
            Repository.Update(childEarnings);
        }

        /// <summary>
        /// Create new child earnings
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        public void CreateNew(int familyMemberId)
        {
            var childEarings = new ChildEarnings
            {
                FamilyMemberID = familyMemberId
            };

            Repository.Insert(childEarings);
        }

        /// <summary>
        /// Get child earnings by family member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Child earnings</returns>
        public ChildEarnings GetByMemberId(int familyMemberId)
        {
            return Repository.Table<ChildEarnings>().SingleOrDefault(m => m.FamilyMemberID == familyMemberId);
        }

        /// <summary>
        /// Pay chores payment
        /// </summary>
        public void Pay()
        {
            var nextPayDate = DateTime.UtcNow.ToLocalTime().GetNextPayDay().ToPayDayTime().ToUniversalTime();
            var startDate = DateTime.UtcNow.ToLocalTime().GetLastPayDay().ToPayDayTime().ToUniversalTime();
            decimal memberPrevAmount = decimal.Zero;

            var Previousthirtyday = Convert.ToDateTime(startDate).AddDays(-30);

            var groupedChoresByFamily = Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                                        .Where(m => m.ChoreStatus == ChoreStatus.CompletedAndApproved
                                        && m.CompletedOn >= Previousthirtyday
                                        && m.CompletedOn < nextPayDate
                                         && !m.BankTransactionID.HasValue && !m.IsDeleted && !m.FamilyMember.IsDeleted).ToList()
                                        .GroupBy(m => m.FamilyMember.User.FamilyID);

            foreach (var familyChores in groupedChoresByFamily)
            {
                var totalPayment = familyChores.Sum(m => m.Value);

                var childMembers = familyChores.Select(p => p.FamilyMember).Distinct().ToList();

                foreach (var child in childMembers)
                {
                    if (memberPrevAmount != decimal.Zero)
                        totalPayment = totalPayment - memberPrevAmount;

                    //includedPayDayAmount = CalculateChildChoreStatusAmountPayday(child.Id);
                    //totalPayment = totalPayment + includedPayDayAmount;
                }

                var adminMember = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == familyChores.Key && m.MemberType == MemberType.Admin);
                try
                {
                    if (totalPayment < 0)
                        throw new InvalidOperationException("Payday cannot be processed since they have a -ve payday balance.");

                    if (!_bankService.IsBankLinked(adminMember.Id))
                        throw new InvalidOperationException("Bank is not linked or verified!");

                    // Tranfer amount to corepro account
                    var transactionResult = _transactionService.Transfer(adminMember.Id, totalPayment, PaymentType.Chore, TransferType.ExternalToInetrnalAccount);

                    // If transaction failure, continue to next family
                    if (!transactionResult.HasValue)
                        continue;

                    foreach (var chore in familyChores)
                    {
                        chore.BankTransactionID = transactionResult;
                        Repository.Update(chore);
                    }

                    var choresByfamilyMembers = familyChores.GroupBy(p => p.FamilyMemberID).ToList();
                    foreach (var childChores in choresByfamilyMembers)
                    {
                        var totalChorePayment = childChores.Sum(m => m.Value);
                        var child = childChores.FirstOrDefault().FamilyMember;

                        if (!string.IsNullOrEmpty(child.PhoneNumber))
                        {
                            var message = $"It's payday! you have been sent ${totalChorePayment:N2}. This will be processed in 1-2 days.";
                            _textMessageService.Send(child.PhoneNumber, message);
                        }
                        /*else //update 02/24/2017 CT: fix defect for Parent SMS Confirmation
                        {
                            var message = $"It's payday for {child.Firstname.FirstCharToUpper()}! Let them know they've received ${totalChorePayment:N2}. It will be processed in 1-2 days.";
                            if (adminMember != null && !string.IsNullOrEmpty(adminMember.PhoneNumber))
                                _textMessageService.Send(adminMember.PhoneNumber, message);
                        }*/
                    }
                }
                catch (Exception ex)
                {
                    _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, totalPayment);
                    continue;
                }
            }
        }

        public void PayDayChanges(int familyID)
        {
            var nextPayDate = DateTime.UtcNow.ToLocalTime().GetNextPayDay().ToPayDayTime().ToUniversalTime();
            var startDate = DateTime.UtcNow.ToLocalTime().GetLastPayDay().ToPayDayTime().ToUniversalTime();

            var Previousthirtyday = Convert.ToDateTime(startDate).AddDays(-30);

            var familyChores = Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                                        .Where(m => m.ChoreStatus == ChoreStatus.CompletedAndApproved && m.FamilyMember.User.FamilyID == familyID
                                        && m.IncludedFlag == true
                                        && !m.BankTransactionID.HasValue && !m.IsDeleted && !m.FamilyMember.IsDeleted).ToList();

            var totalPayment = familyChores.Sum(m => m.Value);

            var childMembers = familyChores.Select(p => p.FamilyMember).Distinct().ToList();

            var adminMember = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == familyID && m.MemberType == MemberType.Admin);
            try
            {
                var includedFamilyChores = Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                                     .Where(m => m.IncludedFlag == true && m.FamilyMember.User.FamilyID == familyID).ToList();


                foreach (var includedChore in includedFamilyChores)
                {
                    includedChore.IncludedFlag = false;
                    Repository.Update(includedChore);
                }

                if (totalPayment < 1)
                {
                    foreach (var changedChore in familyChores)
                    {
                        var updatedChore = Repository.Table<Chore>().Where(p => p.Id == changedChore.Id).FirstOrDefault();
                        updatedChore.ChoreStatus = ChoreStatus.Completed;
                        Repository.Update(changedChore);
                    }
                    return;
                }
                if (totalPayment < 0)
                    throw new InvalidOperationException("Payday cannot be processed since they have a -ve payday balance.");

                if (!_bankService.IsBankLinked(adminMember.Id))
                    throw new InvalidOperationException("Bank is not linked or verified!");

                // Tranfer amount to corepro account
                var transactionResult = _transactionService.Transfer(adminMember.Id, totalPayment, PaymentType.Chore, TransferType.ExternalToInetrnalAccount);

                // If transaction failure, continue to next family
                if (!transactionResult.HasValue)
                    return;

                foreach (var chore in familyChores)
                {
                    chore.BankTransactionID = transactionResult;
                    Repository.Update(chore);
                }



                var choresByfamilyMembers = familyChores.GroupBy(p => p.FamilyMemberID).ToList();
                foreach (var childChores in choresByfamilyMembers)
                {
                    var totalChorePayment = childChores.Sum(m => m.Value);
                    var child = childChores.FirstOrDefault().FamilyMember;

                    if (!string.IsNullOrEmpty(child.PhoneNumber))
                    {
                        var message = $"It's payday! you have been sent ${totalChorePayment:N2}. This will be processed in 1-2 days.";
                        _textMessageService.Send(child.PhoneNumber, message);
                    }
                }
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, totalPayment);

            }
            //}
        }

        /// <summary>
        /// Gets the financial overview of a child.
        /// </summary>
        /// <param name="weekDay">The weekday name</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        public ChildFinancialOverview GetChildFinancialOverview(DayOfWeek weekDay, int? familyMemberId = null)
        {
            //Change TimeZone
            var dtTodayUtc = DateTime.UtcNow;

            var pendingPayDayApprovalAmount = decimal.Zero;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);

            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var nextPayDayOld = DateTime.UtcNow.Date;
            bool pendingChoreStatus = false;

            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.UserID.Equals(_currentUserService.MemberID) && !p.IsDeleted);
            var ChildMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(memberId) && !p.IsDeleted);



            // Get day of next payday to get completed chore values
            var nextPayDate = DateTime.UtcNow.GetNextPayDay().ToPayDayTime().Date;
            var startDate = DateTime.UtcNow.GetLastPayDay().Date;

            //Date to check amount
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;

            //var payDayTime = Extension.GetPaydayTime();
            if (weekDay == DayOfWeek.Friday)// Get last payday if today is friday
            {
                nextPayDate = startDate;
            }

            // Computes the amount to be paid on next pay day

            var chkStartDate = cstTimeZoneTime.getStartDate();
            var nextPayDateCheck = cstTimeZoneTime.getEndDate();

            var Previousthirtyday = Convert.ToDateTime(nextPayDateCheck).AddDays(-30);
            var completedChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                        && p.CompletedOn >= chkStartDate && p.CompletedOn < nextPayDateCheck
                                                        && p.IsCompleted == true && p.ChoreStatus == ChoreStatus.Completed
                                                        && !p.BankTransactionID.HasValue && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value);

            var nextPayAmount = decimal.Zero;
            if (completedChores.Any())
                nextPayAmount = completedChores.Sum();



            // Gets the allocation settings of current child
            var allocationSettings = Repository.Table<AllocationSettings>().SingleOrDefault(p => p.FamilyMemberID.Equals(memberId));

            // Get expected earnings
            var nextPayDistribution = new ChildEarnings();
            if (allocationSettings != null)
            {
                nextPayDistribution.Save = (nextPayAmount * (allocationSettings.Save / 100));
                nextPayDistribution.Share = (nextPayAmount * (allocationSettings.Share / 100));
                nextPayDistribution.Spend = (nextPayAmount * (allocationSettings.Spend / 100));
            }

            // Gets child earnings
            var earnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID.Equals(memberId));



            var familyID = Repository.Table<User>().Where(m => m.Id == memberId).FirstOrDefault().FamilyID;
            var AdminFamilyMemberID = Repository.Table<User>().Where(m => m.FamilyID == familyID && m.Email != null).FirstOrDefault().Id;
            var IsSmsActive = Repository.Table<SMSApproval>().Where(M => M.FamilyMemberID == AdminFamilyMemberID && M.ApprovalType == ApprovalType.ChorePayment).OrderByDescending(p => p.Id).FirstOrDefault();

            if (IsSmsActive != null && IsSmsActive.IsActive) //New logic has been added after 25 May
                pendingPayDayApprovalAmount = GetPendingAmount(memberId, utcOffset);

            if (pendingPayDayApprovalAmount > 0)
                pendingChoreStatus = true;

            //Get Child Latest corepro details
            var chkTXNDate = nextPayDateCheck.AddDays(-7); //New logic has been added after 25 May
            var transactionDetails = Repository.Table<BankTransaction>().Where(p => p.FamilyMemberID == AdminFamilyMemberID && p.CreatedOn >= chkTXNDate && p.PaymentType == PaymentType.Chore).OrderByDescending(p => p.Id).FirstOrDefault();
            bool childPendingStatus = false;
            if (IsSmsActive != null)
            {
                if (transactionDetails != null)
                    childPendingStatus = transactionDetails.TransactionStatus == TransactionStatus.Completed ? false : true;
                else
                {
                    if (IsSmsActive.IsActive && pendingPayDayApprovalAmount > 0)
                        childPendingStatus = true;
                }
            }

            //Approve Disapprove amount adjustment
            var ChildDisaprovedandPendingAmt = CalculateChildChoreStatusAmountDB(memberId);
            if (ChildDisaprovedandPendingAmt < 0) //New logic has been added after 25 May
            {
                nextPayAmount = nextPayAmount - ChildDisaprovedandPendingAmt;
            }

            int timeToday = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).TimeOfDay.Hours;

            if ((utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DayOfWeek == DayOfWeek.Thursday && timeToday >= 10) || (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday))
            {
                // Get added day of next payday
                nextPayDayOld = DateTime.UtcNow.GetNextPayDay(DayOfWeek.Friday, 7).Date;
                // Get added day of next payday for friday
            }
            else
                //Get day of next payday
                nextPayDayOld = DateTime.UtcNow.GetNextPayDay(DayOfWeek.Friday).Date;


            DateTime dt = new DateTime();
            dt = Convert.ToDateTime(nextPayDayOld);
            int PayDaydate = dt.Day;

            string PayDaydateSuffix = (PayDaydate % 10 == 1 && PayDaydate != 11) ? "st" : (PayDaydate % 10 == 2 && PayDaydate != 12) ? "nd"
                                : (PayDaydate % 10 == 3 && PayDaydate != 13) ? "rd" : "th";

            string PayDayMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month);

            var nextPayDay = PayDayMonth + " " + PayDaydate + "" + PayDaydateSuffix;

            // Prepare child financial overview
            var childFinancialOverview = new ChildFinancialOverview
            {
                NextPayDate = nextPayDay,
                NextPayAmount = nextPayAmount,
                AllocationSettings = allocationSettings != null ? allocationSettings : new AllocationSettings(),
                Earnings = earnings != null ? earnings : new ChildEarnings(),
                NextPayDistribution = nextPayDistribution,
                pendingChoreStatus = pendingChoreStatus,
                childChoreStatus = childPendingStatus
            };

            return childFinancialOverview;
        }

        public decimal CalculateChildChoreStatusAmountDB(int memberId)
        {
            var dtTodayUtc = DateTime.UtcNow;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            var total = decimal.Zero;
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;
            var chkStartDate = cstTimeZoneTime.getStartDate();
            var nextPayDateCheck = cstTimeZoneTime.getEndDate();
            decimal DisapprovedAndPendingChores = decimal.Zero;
            // var nextPayAmount1 = decimal.Zero;

            //var ChildMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(memberId) && !p.IsDeleted);

            var Choredetail = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                        && p.IsCompleted == true && (p.RemoveAprovalDate != null || p.ApprovedApprovalDate != null)).ToList();

            var Chorebankdetail = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                   && p.IsCompleted == true && p.RemoveAprovalDate == null && p.ApprovedApprovalDate == null && p.BankTransactionID != null).ToList();



            if (Choredetail.Count > 0)
            {
                foreach (var item in Choredetail)
                {
                    DisapprovedAndPendingChores = decimal.Zero;

                    if (item.RemoveAprovalDate != null && item.ApprovedApprovalDate != null)
                    {
                        if (item.RemoveAprovalDate > item.ApprovedApprovalDate)
                        {
                            DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                           && p.CompletedOn < chkStartDate && p.Id == item.Id && p.IncludedFlag == false
                                           && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.DisApproved)
                                           && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();

                            //total = total + DisapprovedAndPendingChores;

                        }
                        else
                        {
                            DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                         && p.ApprovedApprovalDate >= chkStartDate && p.Id == item.Id && p.IncludedFlag == false
                                         && p.CompletedOn < chkStartDate
                                         && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.Completed)
                                         && !p.BankTransactionID.HasValue && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();

                            total = total - DisapprovedAndPendingChores;
                        }
                    }
                    else if (item.ApprovedApprovalDate != null && item.RemoveAprovalDate == null)
                    {
                        DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                          && p.CompletedOn < chkStartDate && p.Id == item.Id && p.IncludedFlag == false
                                          && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.Completed)
                                          && !p.BankTransactionID.HasValue && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();
                        total = total - DisapprovedAndPendingChores;
                    }
                    else if (item.RemoveAprovalDate != null && item.ApprovedApprovalDate == null)
                    {
                        DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                       && p.CompletedOn < chkStartDate && p.Id == item.Id && p.IncludedFlag == false
                                       && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.DisApproved)
                                       && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();

                        //total = total + DisapprovedAndPendingChores;

                    }
                }

                DisapprovedAndPendingChores = total;

            }
            else if (Chorebankdetail.Count > 0)
            {
                foreach (var item in Chorebankdetail)
                {

                    var banktransaction = Repository.Table<BankTransaction>().Where(p => p.Id == item.BankTransactionID && p.TransactionStatus == TransactionStatus.Completed).FirstOrDefault();
                    if (banktransaction != null && banktransaction.LastUpdatedOn != null)
                    {
                        var Previousthirtyday = Convert.ToDateTime(banktransaction.LastUpdatedOn.ToString()).AddDays(-30);

                        DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                            && p.CompletedOn >= Previousthirtyday && p.CompletedOn < banktransaction.LastUpdatedOn && p.Id == item.Id
                                            && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.DisApproved) && p.IncludedFlag == false
                                            && !p.BankTransactionID.HasValue && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();


                        //total = total + DisapprovedAndPendingChores;
                    }


                }

                DisapprovedAndPendingChores = total;

            }

            return DisapprovedAndPendingChores;
        }
        //added by stpl
        public childApprovalDetails RemoveApprovalService(int choreId)
        {
            var changedChoresStatus = Repository.Table<Chore>().Where(p => p.Id == choreId).FirstOrDefault();
            if ((changedChoresStatus.ChoreStatus == ChoreStatus.Completed && changedChoresStatus.BankTransactionID == null)
                || (changedChoresStatus.ChoreStatus == ChoreStatus.CompletedAndApproved && changedChoresStatus.BankTransactionID == null))
            {
                changedChoresStatus.ChoreStatus = ChoreStatus.DisApproved;
                changedChoresStatus.RemoveAprovalDate = DateTime.UtcNow;
            }
            else if ((changedChoresStatus.ChoreStatus == ChoreStatus.CompletedAndApproved && changedChoresStatus.BankTransactionID != null))
            {
                changedChoresStatus.ChoreStatus = ChoreStatus.DisapprovedAndPending;
                changedChoresStatus.RemoveAprovalDate = DateTime.UtcNow;
            }
            else if ((changedChoresStatus.ChoreStatus == ChoreStatus.CompletedAndPaid && changedChoresStatus.BankTransactionID != null))
            {
                changedChoresStatus.ChoreStatus = ChoreStatus.DisapprovedAndPending;
                changedChoresStatus.RemoveAprovalDate = DateTime.UtcNow;
            }
            Repository.Update(changedChoresStatus);

            //Set 
            var childReturnStatus = new childApprovalDetails
            {
                ApprovalStatus = changedChoresStatus.ChoreStatus,
                Amount = changedChoresStatus.Value
            };

            return childReturnStatus;
        }

        public childApprovalDetails ApproveForPayday(int choreId)
        {

            var changedChoresStatus = Repository.Table<Chore>().Where(p => p.Id == choreId).FirstOrDefault();
            if (changedChoresStatus.BankTransactionID != null)
            {
                var chkBankTransaction = Repository.Table<BankTransaction>().Where(p => p.Id == changedChoresStatus.BankTransactionID).FirstOrDefault();
                if (chkBankTransaction.TransactionStatus == TransactionStatus.Pending)
                {
                    changedChoresStatus.ChoreStatus = ChoreStatus.CompletedAndApproved;
                    changedChoresStatus.ApprovedApprovalDate = DateTime.UtcNow;
                }
                else if (chkBankTransaction.TransactionStatus == TransactionStatus.Completed)
                {
                    changedChoresStatus.ChoreStatus = ChoreStatus.CompletedAndPaid;
                    changedChoresStatus.ApprovedApprovalDate = DateTime.UtcNow;
                }
            }
            else
            {
                changedChoresStatus.ChoreStatus = ChoreStatus.Completed;
                changedChoresStatus.ApprovedApprovalDate = DateTime.UtcNow;
            }

            Repository.Update(changedChoresStatus);

            //Set 
            var childReturnStatus = new childApprovalDetails
            {
                ApprovalStatus = changedChoresStatus.ChoreStatus,
                Amount = changedChoresStatus.Value
            };

            return childReturnStatus;
        }

        public void chkEnumTest()
        {
            List<MessageEvent> listOfPersons = new List<MessageEvent>();
            List<MessageEventEnum> objTxnStatus = new List<MessageEventEnum>();
            DirectoryInfo dir = new DirectoryInfo(_appSettingsService.CoreproTransactionDetailFilePath);
            FileInfo[] TXTFiles = dir.GetFiles("*.txt");

            if (TXTFiles.Length == 0)
                return;

            foreach (var file in TXTFiles)
            {

                if (file.Exists)
                {
                    string[] files = Directory.GetFiles(_appSettingsService.CoreproTransactionDetailFilePath);

                    foreach (string fileName in files)
                    {
                        using (StreamReader sr = new StreamReader(fileName))
                        {
                            string line;

                            int counter = 0;
                            MessageEvent objSingleRecord = new MessageEvent();
                            while ((line = sr.ReadLine()) != null)
                            {
                                int strLength = line.Length;
                                if (counter != 0)
                                {
                                    foreach (var lines in line)
                                    {

                                        if (objTxnStatus.Contains((MessageEventEnum)(string.IsNullOrEmpty(line.Substring(300, 10).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(300, 10).ToString().TrimStart('0')))))
                                            objSingleRecord.UserEventType = (MessageEventEnum)(string.IsNullOrEmpty(line.Substring(300, 10).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(300, 10).ToString().TrimStart('0')));
                                        objSingleRecord.UserEventId = 0;
                                        objSingleRecord.TransactionId = string.IsNullOrEmpty(line.Substring(180, 19).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(180, 19).ToString().TrimStart('0'));
                                        objSingleRecord.CustomerId = string.IsNullOrEmpty(line.Substring(0, 10).ToString().TrimStart('0')) ? 0 : Convert.ToInt32(line.Substring(0, 10).ToString().TrimStart('0'));
                                        objSingleRecord.TransactionStatus = "";
                                        objSingleRecord.TransactionReturnCode = "";
                                        objSingleRecord.EventTime = DateTime.UtcNow;
                                    }
                                    listOfPersons.Add(objSingleRecord);
                                }
                                counter++;
                            }
                        }
                    }
                }
            }
        }

        public bool ShowThirtyDaysLink(int choreId)
        {
            var dtTodayUtc = DateTime.UtcNow;
            var choreCompletedOn = Repository.Table<Chore>().Where(p => p.Id == choreId).FirstOrDefault().CompletedOn;
            if (dtTodayUtc.AddDays(-30) < choreCompletedOn)
                return true;
            else
                return false;
        }

        public decimal GetPendingAmount(int memberId, DateTimeOffset utcOffset)
        {
            var pendingAmount = decimal.Zero;
            var startDate = DateTime.UtcNow;

            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            var currentDay = (int)utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DayOfWeek;
            int currentHour = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).TimeOfDay.Hours;

            DateTime todayDate = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;

            if ((currentDay > 4) || (currentDay == 4 && currentHour >= 10))
            {
                DateTime lastThursday = DateTime.UtcNow.Date;
                if (todayDate.DayOfWeek == DayOfWeek.Thursday && currentHour >= 10)
                {
                    lastThursday = todayDate.Date;
                }
                else
                {
                    lastThursday = DateTime.UtcNow.Date.AddDays(-1);
                    while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
                        lastThursday = lastThursday.AddDays(-1);
                }
                startDate = lastThursday.AddHours(15);
            }
            else
                startDate = todayDate.getStartDate();


            var completedPendingAmount = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                         && p.CompletedOn <= startDate
                                                         && (p.IsCompleted == true && p.ChoreStatus == ChoreStatus.Completed))
                                                         .ToList().Select(p => p.Value);

            if (completedPendingAmount.Any())
                pendingAmount = completedPendingAmount.Sum();
            return pendingAmount;
        }

        public decimal GetBeforePendingApprovedAmount(int memberId)
        {
            var dtTodayUtc = DateTime.UtcNow;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            var total = decimal.Zero;
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;
            var chkStartDate = cstTimeZoneTime.getStartDate();
            var nextPayDateCheck = cstTimeZoneTime.getEndDate();
            decimal DisapprovedAndPendingChores = decimal.Zero;
            var nextPayAmount1 = decimal.Zero;

            var Choredetail = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                      && p.IsCompleted == true && (p.RemoveAprovalDate != null || p.ApprovedApprovalDate != null)).ToList();

            //var Chorebankdetail = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
            //                                     && p.IsCompleted == true && p.RemoveAprovalDate == null && p.ApprovedApprovalDate == null && p.BankTransactionID != null).ToList();


            if (Choredetail.Count > 0)
            {
                foreach (var item in Choredetail)
                {
                    if (item.ApprovedApprovalDate > item.RemoveAprovalDate)
                    {
                        DisapprovedAndPendingChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                       && p.CompletedOn < chkStartDate && p.Id == item.Id
                                       && p.IsCompleted == true && (p.ChoreStatus == ChoreStatus.CompletedAndPaid)
                                       && !p.IsDeleted && !p.FamilyMember.IsDeleted).ToList().Select(p => p.Value).FirstOrDefault();
                        total = total - DisapprovedAndPendingChores;
                    }
                }
            }

            return total;
        }
        public decimal GetPendingApprovedAmount(int memberId)
        {
            var pendingAmount = decimal.Zero;
            //Calculate the approved payment amount
            var approvedForPayment = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                        && (p.IsCompleted == true && p.ChoreStatus == ChoreStatus.CompletedAndApproved))
                                                        .ToList().Select(p => p.Value);

            if (approvedForPayment.Any())
                pendingAmount = approvedForPayment.Sum();

            //decimal memberPrevAmount = decimal.Zero;
            //memberPrevAmount = GetBeforePendingApprovedAmount(memberId);

            //if (memberPrevAmount != decimal.Zero)
            //{
            //    pendingAmount = pendingAmount - memberPrevAmount;
            //    if (pendingAmount < decimal.Zero)
            //        pendingAmount = decimal.Zero;
            //}
            return pendingAmount;
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        /// <summary>
        /// Gets the total earnigs of all childrens of the family
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns>The total amount in  buckets.</returns>
        public decimal GetTotalEarningsByFamily(int familyId)
        {
            var totalEarnings = Repository.Table<ChildEarnings>().Where(p => p.FamilyMember.User.FamilyID == familyId).ToList();
            return totalEarnings.Any() ? totalEarnings.Sum(p => p.Save) + totalEarnings.Sum(p => p.Share) + totalEarnings.Sum(p => p.Spend) : 0;
        }

        /// <summary>
        /// Resets the all child's buckets of the family
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        public void ResetChildEarningsByFamily(int familyId)
        {
            // Update the child earnings
            var childEarnings = Repository.Table<ChildEarnings>()
                .Where(p => p.FamilyMember.User.FamilyID == familyId)
                .ToList();

            foreach (var childEarning in childEarnings)
            {
                childEarning.Save = 0;
                childEarning.Share = 0;
                childEarning.Spend = 0;
                Repository.Update(childEarning);
            }
        }

        /// <summary>
        /// Move money
        /// </summary>
        /// <param name="sourceBucket">Source bucket</param>
        /// <param name="destinationBucket">Destination bucket</param>
        /// <param name="amount">Amount</param>
        /// <returns>Child earnings</returns>
        public ChildEarnings MoveMoney(EarningsBucketType sourceBucket, EarningsBucketType destinationBucket, decimal amount)
        {
            if (sourceBucket == destinationBucket)
                throw new InvalidParameterException("You can't move same bucket");

            // Gets child earnings
            var earnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID.Equals(_currentUserService.MemberID));
            if (earnings == null) throw new ObjectNotFoundException("Child earnings is empty");

            var insufBalanceMsg = "Insufficient balance to complete the transfer";
            // Reduce amount from source bucket
            switch (sourceBucket)
            {
                case EarningsBucketType.Save:
                    if (earnings.Save < amount) throw new InvalidOperationException(insufBalanceMsg);

                    earnings.Save -= amount;
                    break;
                case EarningsBucketType.Share:
                    if (earnings.Share < amount) throw new InvalidOperationException(insufBalanceMsg);

                    earnings.Share -= amount;
                    break;
                case EarningsBucketType.Spend:
                    if (earnings.Spend < amount) throw new InvalidOperationException(insufBalanceMsg);

                    earnings.Spend -= amount;
                    break;
            }

            // Add amount to desination
            switch (destinationBucket)
            {
                case EarningsBucketType.Save:
                    earnings.Save += amount;
                    break;
                case EarningsBucketType.Share:
                    earnings.Share += amount;
                    break;
                case EarningsBucketType.Spend:
                    earnings.Spend += amount;
                    break;
            }

            Repository.Update<ChildEarnings>(earnings);
            return earnings;
        }

        /// <summary>
        /// Send bonus to the child
        /// </summary>
        /// <param name="bonus">The bonus.</param>
        public void SendBonus(ChildBonus bonus, int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;

            // Gets current authenticated member
            var member = Repository.Table<FamilyMember>().Include(p => p.User).SingleOrDefault(p => p.Id == memberId);

            // Gets the admin id of the family if current member type is parent
            if (member.MemberType != MemberType.Admin)
            {
                member = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == member.User.FamilyID && m.MemberType == MemberType.Admin);
            }

            // Checks whether the member has linked thier bank account or not
            if (!_bankService.IsBankLinked(member.Id))
                throw new InvalidOperationException("Bank is not linked or verified!");

            try
            {
                // Tranfer amount to corepro account
                var transactionResult = _transactionService.Transfer(member.Id, bonus.Amount, PaymentType.Bonus, TransferType.ExternalToInetrnalAccount);

                // If transaction failure, skips the process
                if (!transactionResult.HasValue)
                    throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                // Insert bonus
                bonus.Date = DateTime.UtcNow;
                bonus.ContributorID = memberId;
                bonus.BankTransactionID = transactionResult.Value;
                Repository.Insert(bonus);

                // Sms receive bonus message to child
                var child = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).SingleOrDefault(p => p.Id == bonus.ChildID && !p.IsDeleted);
                if (child != null)
                {
                    if (!string.IsNullOrEmpty(child.PhoneNumber))
                    {
                        var message = string.Format(_appSettingsService.ChildHasPhoneReceiveBonusMessage,
                            child.Firstname, bonus.Amount, bonus.Contributor.Firstname);
                        _textMessageService.Send(child.PhoneNumber, message);
                    }
                    else
                    {
                        var admin = Repository.Table<FamilyMember>().SingleOrDefault(m => m.Id == memberId);
                        if (admin != null && !string.IsNullOrEmpty(admin.PhoneNumber))
                        {
                            var message = string.Format(_appSettingsService.ChildHasNoPhoneReceiveBonusMessage,
                                admin.Firstname, child.Firstname, bonus.Amount);
                            _textMessageService.Send(admin.PhoneNumber, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(memberId, ex.Message, bonus.Amount);
                throw new InvalidOperationException(ex.Message);
            }
        }

        public bool ShowApproveDisapprovelink(int choreId)
        {
            // If Chore is paid no link
            var choreStatusDetails = Repository.Table<Chore>().Where(m => m.Id == choreId).FirstOrDefault();
            if (choreStatusDetails.ChoreStatus == ChoreStatus.CompletedAndPaid || choreStatusDetails.ChoreStatus == ChoreStatus.CompletedAndApproved)
                return false;

            return true;

        }


        /// <summary>
        /// Can transact from earnings bucket
        /// </summary>
        /// <param name="bucketType">Earnings bucket type</param>
        /// <param name="amount">Amount</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>Result</returns>
        public bool CanTransact(EarningsBucketType bucketType, decimal amount, int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;

            switch (bucketType)
            {
                case EarningsBucketType.Save:
                    return Repository.Table<ChildEarnings>().Any(m => m.FamilyMemberID == memberId && m.Save >= amount);
                case EarningsBucketType.Share:
                    return Repository.Table<ChildEarnings>().Any(m => m.FamilyMemberID == memberId && m.Share >= amount);
                case EarningsBucketType.Spend:
                    return Repository.Table<ChildEarnings>().Any(m => m.FamilyMemberID == memberId && m.Spend >= amount);
                default:
                    return false;
            }
        }



        #endregion
    }
}
