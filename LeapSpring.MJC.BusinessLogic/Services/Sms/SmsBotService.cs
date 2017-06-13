using LeapSpring.MJC.Core.Dto.Sms;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Sms;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.BusinessLogic.Services.RecurringChore;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.BusinessLogic.Services.Save;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.BusinessLogic.Services.Spend;
using LeapSpring.MJC.Core.Domain.Spend;
using System.Web;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    /// <summary>
    /// Represents a sms bot service
    /// </summary>
    public class SmsBotService : ServiceBase, ISmsBotService
    {
        private IFamilyService _familyService;
        private IChoreService _choreService;
        private IAppSettingsService _appSettingsService;
        private IEarningsService _earningsService;
        private IRecurringChoreService _recurringChoreService;
        private ITextMessageService _textMessageService;
        private ISMSApprovalService _smsApprovalService;
        private string _errMsg = "Invalid command";

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public SmsBotService(IRepository repository, IFamilyService familyService, IChoreService choreService, IAppSettingsService appSettingsService,
            IEarningsService earningsService, IRecurringChoreService recurringChoreService,
            ITextMessageService textMessageService, ISMSApprovalService smsApprovalService) : base(repository)
        {
            _familyService = familyService;
            _choreService = choreService;
            _appSettingsService = appSettingsService;
            _earningsService = earningsService;
            _recurringChoreService = recurringChoreService;
            _textMessageService = textMessageService;
            _smsApprovalService = smsApprovalService;
        }

        #region Utilities

        /// <summary>
        /// Check message command type
        /// </summary>
        /// <param name="message">Body message</param>
        /// <returns>Command type and text as Tuple</returns>
        private Tuple<SmsCommandType, string, string> ProcessMessage(string from, string message, bool isChild)
        {
            var wrongCommandType = SmsCommandType.Wrong;
            var commandType = message.ToEnum(wrongCommandType);
            var childName = string.Empty;
            var amount = string.Empty;
            var commandMsg = message.Split(' ');

            if (commandType == SmsCommandType.Help || commandType == SmsCommandType.Yes || (commandType == SmsCommandType.No && commandMsg.Length == 1) || commandType == SmsCommandType.Always)
                return new Tuple<SmsCommandType, string, string>(commandType, null, null);

            if (isChild)
                return new Tuple<SmsCommandType, string, string>(commandType, null, null);
            else
            {

                if (commandMsg.Length < 2)
                    return new Tuple<SmsCommandType, string, string>(SmsCommandType.Wrong, null, null);

                // Check single word command
                commandType = commandMsg[0].ToEnum(wrongCommandType);
                if (commandType == wrongCommandType)
                    return new Tuple<SmsCommandType, string, string>(SmsCommandType.Wrong, null, null);

                childName = commandMsg[1];
                if (commandType == SmsCommandType.Bonus)
                {
                    if (commandMsg.Length < 3)
                        return new Tuple<SmsCommandType, string, string>(SmsCommandType.Wrong, null, null);

                    amount = commandMsg.LastOrDefault().Replace("$", "");
                    if (commandMsg.Length > 3) // Get child full name
                        childName = string.Format("{0} {1}", commandMsg[1], commandMsg[2]);
                }
                else if (commandMsg.Length > 2) // Get child full name
                    childName = string.Format("{0} {1}", commandMsg[1], commandMsg[2]);

                return new Tuple<SmsCommandType, string, string>(commandType, childName, amount.Replace("$", ""));
            }
        }

        /// <summary>
        /// Get child status message
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        /// <param name="childName">Child name</param>
        /// <returns>Result message</returns>
        private string GetKidStatus(int familyId, string childName)
        {
            var childMember = _familyService.GetMember(familyId, childName, MemberType.Child);
            if (childMember == null)
                return "Child not found";

            var dueChoreCount = _choreService.GetChoreCount(ChoreDueType.Overdue, childMember.Id);
            var todayChoreCount = _choreService.GetChoreCount(ChoreDueType.Today, childMember.Id);

            return string.Format("{0}- Chores Past Due: {1}, Chores Today: {2}", childMember.Firstname.FirstCharToUpper(), dueChoreCount, todayChoreCount);
        }

        /// <summary>
        /// Get child status message
        /// </summary>
        /// <param name="childMemberId">Family member</param>
        /// <returns>Result message</returns>
        private string GetKidStatus(int childMemberId)
        {
            var dueChoreCount = _choreService.GetChoreCount(ChoreDueType.Overdue, childMemberId);
            var todayChoreCount = _choreService.GetChoreCount(ChoreDueType.Today, childMemberId);

            // Get family member
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).SingleOrDefault(p => p.Id == childMemberId);
            var familyDomainUrl = _appSettingsService.SiteUrl + "#/family/" + familyMember.User?.Family?.UniqueName;
            return string.Format("You have {0} chores today and {1} past due chores. --BusyKid.\n{2}", todayChoreCount, dueChoreCount, familyDomainUrl);
        }

        /// <summary>
        /// Get child balance message
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        /// <param name="childName">Child name</param>
        /// <returns>Result message</returns>
        private string GetKidBalance(int familyId, string childName)
        {
            var childMember = _familyService.GetMember(familyId, childName, MemberType.Child);
            if (childMember == null)
                return "Child not found";

            var earnings = _earningsService.GetByMemberId(childMember.Id);
            if (earnings == null)
                return "No earnings";

            return string.Format("{0}- Spend: $ {1}, Save: $ {2}, Share: $ {3}", childMember.Firstname.FirstCharToUpper(), earnings.Spend, earnings.Save, earnings.Share);
        }

        /// <summary>
        /// Get child balance message
        /// </summary>
        /// <param name="childMemberId">Family member</param>
        /// <returns>Result message</returns>
        private string GetKidBalance(int childMemberId)
        {
            var earnings = _earningsService.GetByMemberId(childMemberId);
            if (earnings == null)
                return "No earnings";

            // Get family member
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).SingleOrDefault(p => p.Id.Equals(childMemberId));
            var familyDomainUrl = _appSettingsService.SiteUrl + "#/family/" + familyMember.User?.Family?.UniqueName;
            return string.Format("Your current balances are, Spend: $ {0}, Save: $ {1}, Share: $ {2}. --BusyKid.\n{3}", earnings.Spend, earnings.Save, earnings.Share, familyDomainUrl);
        }

        /// <summary>
        /// Add chore workflow
        /// </summary>
        /// <param name="familyMember">family member</param>
        /// <param name="messageBody">child name</param>
        /// <returns>Result message</returns>
        private string AddChoreWorkflow(FamilyMember familyMember, string messageBody, SmsCommandType commandType)
        {
            var replyMessage = string.Empty;
            FamilyMember childMember = null;
            ChoreWorkflow choreWorkflow = null;
            var choreWorkflowQuery = Repository.Table<ChoreWorkflow>().Include(m => m.ChildMember).Include(m => m.ChildMember.User)
                .Where(m => m.FamilyMemberID == familyMember.Id && m.WorkflowStatus == WorkflowStatus.Active);

            if (commandType == SmsCommandType.AddChore)
                choreWorkflowQuery = choreWorkflowQuery.Where(m => m.ChildMember.Firstname.ToLower() == messageBody);

            if (commandType == SmsCommandType.AddChore)
            {
                childMember = _familyService.GetMember(familyMember.User.FamilyID, messageBody, MemberType.Child);
                if (childMember == null) return "Child not found";

                choreWorkflow = new ChoreWorkflow
                {
                    FamilyMemberID = familyMember.Id,
                    ChildMemberID = childMember.Id
                };
                Repository.Insert(choreWorkflow);
                return "What chore would you like?";
            }

            choreWorkflow = choreWorkflowQuery.OrderByDescending(m => m.Id).FirstOrDefault();
            if (choreWorkflow == null)
                return _errMsg;


            if (string.IsNullOrEmpty(choreWorkflow.Name))
            {
                choreWorkflow.Name = messageBody;
                replyMessage = "Chore value?";
            }
            else if (!choreWorkflow.Value.HasValue)
            {
                var choreValue = decimal.Zero;
                if (!decimal.TryParse(messageBody.Replace("$", ""), out choreValue))
                    return _errMsg;

                choreWorkflow.Value = choreValue;
                replyMessage = string.Format("On which frequency should {0} do this?. Text back 'Once', 'Daily' or 'Weekly'", choreWorkflow.ChildMember.Firstname);
            }
            else if (!choreWorkflow.FrequencyType.HasValue)
            {
                choreWorkflow.FrequencyType = messageBody.FirstCharToUpper().ToEnum(FrequencyType.Daily);
                replyMessage = string.Format("When should {0} do this?", choreWorkflow.ChildMember.Firstname);
            }
            else if (string.IsNullOrEmpty(choreWorkflow.FrequencyRange))
            {
                choreWorkflow.FrequencyRange = messageBody.ConvertToDayFullName();
                if (choreWorkflow.FrequencyType.Value == FrequencyType.Once)
                    choreWorkflow.DueDate = GetNextDateByDayName(choreWorkflow.FrequencyRange);

                SaveChoreWorkflow(choreWorkflow);
                replyMessage = string.Format("Thanks! I've added that chore to {0}.", choreWorkflow.ChildMember.Firstname);
            }
            else
                return _errMsg;

            Repository.Update(choreWorkflow);
            return replyMessage;
        }

        private string SendBonus(string childName, FamilyMember adminMember, decimal amount)
        {
            var childMember = _familyService.GetMember(adminMember.User.FamilyID, childName, MemberType.Child);
            if (childMember == null)
                return "Child not found";

            var childBonus = new ChildBonus
            {
                ChildID = childMember.Id,
                Date = DateTime.UtcNow,
                ContributorID = adminMember.Id,
                Note = "Bonus sent through SMS",
                Amount = amount
            };

            try
            {
                _earningsService.SendBonus(childBonus, adminMember.Id);
                return "Bonus successfully sent to " + childName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get random joke
        /// </summary>
        /// <returns>Joke</returns>
        private string GetJoke()
        {
            var joke = Repository.Table<Joke>().RandomElement();
            if (joke != null)
                return joke.Text;
            else
                return string.Empty;
        }

        /// <summary>
        /// Get help sms for how to use
        /// </summary>
        /// <param name="isChild">Is Child</param>
        /// <returns>Help message</returns>
        private string GetHelp(bool isChild)
        {
            if (isChild)
            {
                return string.Format("Text 'BALANCE' To get balance. {0}"
                    + "Text 'MY CHORES' To get chores status. {0}"
                    + "Text 'JOKE', To get Joke.", "\n");
            }
            else
            {
                return string.Format("Text 'ADDCHORE <KID NAME>' To add chore. {0}"
                    + "Text 'BONUS <KID NAME> $x' To send bonus. {0}"
                    + "Text 'BALANCE <KID NAME>' To get the Kid's balance in each bucket. {0}"
                    + "Text 'STATUS <KID NAME>' To get the Kid's chore status.", "\n");
            }
        }

        /// <summary>
        /// Get next date by day name
        /// </summary>
        /// <param name="dayName">Day name</param>
        /// <returns>Date</returns>
        private DateTime GetNextDateByDayName(string dayName)
        {
            var todayDate = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                if (todayDate.DayOfWeek.ToString() == dayName)
                    return todayDate;

                todayDate = todayDate.AddDays(1);
            }

            return DateTime.Now;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sms receive
        /// </summary>
        /// <param name="smsResponse">Sms response</param>
        async public void Receive(SmsResponse smsResponse)
        {
            var familyMember = _familyService.GetMemberByPhone(smsResponse.From);
            if (familyMember == null)
            {
                return;
            }

            var isChild = familyMember.MemberType == MemberType.Child;
            smsResponse.Body = smsResponse.Body.ToLower().TrimStart(' ');
            var commandType = ProcessMessage(smsResponse.From, smsResponse.Body, isChild);
            var message = string.Empty;
            switch (commandType.Item1)
            {
                case SmsCommandType.KidStatus:
                    message = GetKidStatus(familyMember.User.FamilyID, commandType.Item2);
                    break;
                case SmsCommandType.KidBalance:
                    if (isChild)
                        message = GetKidBalance(familyMember.Id);
                    else
                        message = GetKidBalance(familyMember.User.FamilyID, commandType.Item2);
                    break;
                case SmsCommandType.MyChoreStatus:
                    message = GetKidStatus(familyMember.Id);
                    break;
                case SmsCommandType.Yes:
                    message = await _smsApprovalService.Approve(familyMember);
                    break;
                case SmsCommandType.No:
                    message = _smsApprovalService.Disapprove(familyMember);
                    break;
                case SmsCommandType.AddChore:
                    message = AddChoreWorkflow(familyMember, commandType.Item2, commandType.Item1);
                    break;
                case SmsCommandType.Joke:
                    message = GetJoke();
                    break;
                case SmsCommandType.Help:
                    message = GetHelp(isChild);
                    break;
                case SmsCommandType.Bonus:
                    decimal amount;
                    var result = decimal.TryParse(commandType.Item3, out amount);
                    if (!result)
                        message = _errMsg;
                    else
                        message = SendBonus(commandType.Item2, familyMember, decimal.Parse(commandType.Item3));
                    break;
                case SmsCommandType.Wrong:
                    if (!isChild)
                        message = AddChoreWorkflow(familyMember, smsResponse.Body, commandType.Item1);
                    else
                        message = _errMsg;
                    break;
                case SmsCommandType.Always:
                    _familyService.MarkAsPayDayAutoApproval(familyMember);
                    message = _smsApprovalService.ApproveChores(familyMember);
                    break;
            }

            // Skip, If message is empty
            if (string.IsNullOrEmpty(message)) return;

            _textMessageService.Send(smsResponse.From, message);
        }

        async public void ReceiveDummy(string msgRes, string phoneNumber)
        {
            var familyMember = _familyService.GetMemberByPhone(phoneNumber);
            if (familyMember == null)
            {
                //_textMessageService.Send(smsResponse.From, "Invalid user");
                return;
            }
            var message = string.Empty;
            if (msgRes == "Yes")
                message = await _smsApprovalService.Approve(familyMember);
            else
                message = _smsApprovalService.Disapprove(familyMember);
        }

        /// <summary>
        /// Save chore workflow
        /// </summary>
        /// <param name="choreWorkflow">Chore workflow</param>
        public void SaveChoreWorkflow(ChoreWorkflow choreWorkflow)
        {
            choreWorkflow.WorkflowStatus = WorkflowStatus.Completed;
            Repository.Update(choreWorkflow);

            // Add new chore from chore work flow via sms
            var chore = new Chore
            {
                ChoreStatus = ChoreStatus.Active,
                CreatedTime = DateTime.UtcNow,
                DueDate = choreWorkflow.DueDate,
                FamilyMemberID = choreWorkflow.ChildMemberID,
                FrequencyRange = choreWorkflow.FrequencyRange,
                FrequencyType = choreWorkflow.FrequencyType.Value,
                Name = choreWorkflow.Name.FirstCharToUpper(),
                Value = choreWorkflow.Value.Value
            };
            chore = _choreService.Add(chore, choreWorkflow.ChildMember.User.FamilyID, DateTime.Now.DayOfWeek);
            if (chore.FrequencyType != FrequencyType.Once)
                _recurringChoreService.UpdateRecurringChore(chore.Id, chore.FrequencyType, choreWorkflow.ChildMember.User.FamilyID, DateTime.Now.DayOfWeek);
        }

        #endregion
    }
}
