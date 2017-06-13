using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.BusinessLogic.Services.SubscriptionService;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Email;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;

namespace LeapSpring.MJC.BusinessLogic.Services.Notification
{
    public class NotificationService : ServiceBase, INotificationService
    {
        private ITextMessageService _textMessageService;
        private IEmailTemplateService _emailTemplateService;
        private IEmailService _emailService;
        private IEmailHistoryService _emailHistoryService;
        private ISMSApprovalHistory _smsApprovalHistory;
        private IFamilyService _familyService;
        private IChoreService _choreService;
        private IEarningsService _earningServices;


        public NotificationService(IRepository _repository, ITextMessageService textMessageService, IEmailTemplateService emailTemplateService,
            IEmailService emailService, IEmailHistoryService emailHistoryService, ISMSApprovalHistory smsApprovalHistory,
            IFamilyService familyService, IChoreService choreService, IEarningsService earningServices) : base(_repository)
        {
            _textMessageService = textMessageService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _emailHistoryService = emailHistoryService;
            _smsApprovalHistory = smsApprovalHistory;
            _familyService = familyService;
            _choreService = choreService;
            _earningServices = earningServices;
        }

        /// <summary>
        /// Send pay day notification.
        /// </summary>

        public void SendPayDayMessage()
        {
            var dtTodayUtc = DateTime.UtcNow;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;

            var nextPayDate = cstTimeZoneTime.GetNextPayDay().Date.AddHours(15);
            decimal amountdeduction = decimal.Zero;
            var startDate = nextPayDate.AddDays(-7);
            decimal memberPrevAmount = decimal.Zero;

            var completedChoresByFamily = _choreService.GetCompletedChoresByFamily(startDate, nextPayDate);
            foreach (var familyChores in completedChoresByFamily)
            {
                //Update Include flag for perticular family
                foreach (var choreDetail in familyChores)
                {
                    var includedChore = Repository.Table<Chore>().SingleOrDefault(p => p.Id.Equals(choreDetail.Id));
                    includedChore.IncludedFlag = true;
                    Repository.Update(includedChore);
                }

                var amountToBePaid = familyChores.Sum(m => m.Value);

                var childMembers = familyChores.Select(p => p.FamilyMember).Distinct().ToList();

                //Check for previous Week childs amount
                foreach (var child in childMembers)
                {
                    //memberPrevAmount = _earningServices.CalculateChildChoreStatusAmount(child.Id);
                    if (memberPrevAmount != decimal.Zero)
                        amountToBePaid = amountToBePaid - memberPrevAmount;
                }




                var childCount = Repository.Table<FamilyMember>().Count(m => m.User.FamilyID == familyChores.Key && m.MemberType == MemberType.Child && !m.IsDeleted);
                if (childCount == 0)
                    continue;
                var admin1 = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == familyChores.Key);
                var admin = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == familyChores.Key
                && m.MemberType == MemberType.Admin
                && m.User.Family.FamilySubscription.Status == SubscriptionStatus.Active);

                if (admin == null)
                    continue;

                ////Check Financial Account Verification
                var hasFinancialAccount = Repository.Table<FinancialAccount>().Where(p => p.FamilyMemberID == admin.Id).FirstOrDefault().Status;
                if (hasFinancialAccount != FinancialAccountStatus.Verified)
                    continue;

                //Check The amount less than $1
                if (amountToBePaid < 1)
                {
                    var lessAmountMessage = "Your family total for the week is less than $1.00. BusyKid cannot process transactions less than $1.00.";
                    _smsApprovalHistory.AddLessAmount(admin.Id, ApprovalType.ChorePayment, lessAmountMessage);

                    if (!string.IsNullOrEmpty(admin.PhoneNumber))
                        _textMessageService.Send(admin.PhoneNumber, lessAmountMessage);

                    continue;
                }

                if (admin.PayDayAutoApproval)
                {
                    foreach (var chore in familyChores)
                    {
                        chore.ChoreStatus = ChoreStatus.CompletedAndApproved;
                        Repository.Update(chore);
                    }
                    continue;
                }

                decimal totalAmountToBePaid = 0;
                var meesagesum = "Total";

                var childPaymentDetails = string.Empty;
                foreach (var child in childMembers)
                {
                    var amountToBePaidToChild = familyChores.Where(m => m.FamilyMemberID == child.Id).Sum(m => m.Value);
                    if (amountToBePaidToChild == 0)
                        continue;
                    var separator = "\n";

                    //amountdeduction = _earningServices.CalculateChildChoreStatusAmount(child.Id);
                    amountToBePaidToChild = amountToBePaidToChild - amountdeduction;
                    if (amountToBePaidToChild <= 0)
                        continue;

                    totalAmountToBePaid = totalAmountToBePaid + amountToBePaidToChild;
                    childPaymentDetails += $"{ child.Firstname }: ${amountToBePaidToChild:N2}{separator}";
                }
                childPaymentDetails += $"{ meesagesum }: ${totalAmountToBePaid:N2}";

                var message = $"Tomorrow is payday! Here is a summary of earnings: \n{childPaymentDetails}"
                    + " \n\nReply YES or NO.\nRespond within 2 hours to ensure Friday payday.";

                _smsApprovalHistory.Add(admin.Id, ApprovalType.ChorePayment, message);

                if (!string.IsNullOrEmpty(admin.PhoneNumber))
                    _textMessageService.Send(admin.PhoneNumber, message);

                //SendMessagePaydayNotProceedService(startDate, nextPayDate, admin.Id);
            }
        }
        /// <summary>
        /// Notify incomplete member entrollment
        /// </summary>
        public void NotifyIncompleteNewMemberEntrollment()
        {
            // Get incomplete admin users
            var adminUsers = _familyService.GetIncompleteAdmins();
            // Get email template by incomplete new member enrollment type
            var incompleteMemberEmailTemplate = _emailTemplateService.GetByType(EmailTemplateType.IncompleteNewMemberEnrollment);

            foreach (var admin in adminUsers)
            {
                if (admin.IsUnSubscribed)
                    continue;

                // Get email histories by incomplete new member enrollment type 
                var emailHistory = Repository.Table<EmailHistory>().SingleOrDefault(p => p.FamilyMemberID == admin.Id && p.EmailType == EmailType.IncompleteNewMemberEnrollment);
                if (emailHistory == null)
                {
                    // Send email
                    _emailService.Send(admin.User.Email, incompleteMemberEmailTemplate.Subject, incompleteMemberEmailTemplate?.Content ?? "Incomplete New Member Enrollment");
                    // Save email history
                    _emailHistoryService.SaveEmailHistory(admin.Id, EmailType.IncompleteNewMemberEnrollment);
                }
            }
        }

        /// <summary>
        /// Notify parent has not loggedin
        /// </summary>
        public void NotifyParentHasNotLoggedIn()
        {
            var threeDaysAgo = DateTime.UtcNow.AddDays(-4).Date;
            var fourteenDaysAgo = DateTime.UtcNow.AddDays(-15).Date;

            // Get admin members by loggedon date
            var threeDaysAgoAdminUsers = _familyService.GetMembersByLastLoggedIn(MemberType.Admin, threeDaysAgo);
            var fourteenDaysAgoAdminUsers = _familyService.GetMembersByLastLoggedIn(MemberType.Admin, fourteenDaysAgo);

            if (threeDaysAgoAdminUsers.Count > 0)
                SendEmailToAdmin(threeDaysAgoAdminUsers, EmailTemplateType.ParentHasNotLoggedInThreeDays, EmailType.ParentHasNotLoggedInThreeDays);

            if (fourteenDaysAgoAdminUsers.Count > 0)
                SendEmailToAdmin(fourteenDaysAgoAdminUsers, EmailTemplateType.ParentHasNotLoggedInTwoWeeks, EmailType.ParentHasNotLoggedInTwoWeeks);
        }

        /// <summary>
        /// Notify child has not loggedin
        /// </summary>
        public void NotifyChildHasNotLoggedIn()
        {
            var threeDaysAgo = DateTime.UtcNow.AddDays(-4).Date;
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-8).Date;
            var fourteenDaysAgo = DateTime.UtcNow.AddDays(-15).Date;

            // Get members by loggedon date
            var threeDaysAgoMembers = _familyService.GetChildrensByLastLoggedIn(threeDaysAgo);
            if (threeDaysAgoMembers.Count > 0)
                SendEmailToAdmin(threeDaysAgoMembers, EmailTemplateType.ChildHasNotLoggedInThreeDays, EmailType.ChildHasNotLoggedInThreeDays);

            var sevenDaysAgoMembers = _familyService.GetChildrensByLastLoggedIn(sevenDaysAgo);
            if (sevenDaysAgoMembers.Count > 0)
                SendEmailToAdmin(sevenDaysAgoMembers, EmailTemplateType.ChildHasNotLoggedInSevenDays, EmailType.ChildHasNotLoggedInSevenDays);

            var fourteenDaysAgoMembers = _familyService.GetChildrensByLastLoggedIn(fourteenDaysAgo);
            if (fourteenDaysAgoMembers.Count > 0)
                SendEmailToAdmin(fourteenDaysAgoMembers, EmailTemplateType.ChildHasNotLoggedInTwoWeeks, EmailType.ChildHasNotLoggedInTwoWeeks);
        }

        /// <summary>
        /// Notify no chore completed
        /// </summary>
        public void NotifyNoChoreCompleted()
        {
            var threeDaysAgo = DateTime.UtcNow.AddDays(-4).Date;
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-8).Date;

            // Get incomplete chores of admins
            var threeDaysNoActivitiesByKids = _choreService.GetAdminsWithNoActivityByKids(threeDaysAgo);
            if (threeDaysNoActivitiesByKids.Count > 0)
                SendEmailToAdmin(threeDaysNoActivitiesByKids, EmailTemplateType.NoChoreCompletedThreeDays, EmailType.NoChoreCompletedThreeDays);

            var sevenDaysNoActivitiesByKids = _choreService.GetAdminsWithNoActivityByKids(sevenDaysAgo);
            if (sevenDaysNoActivitiesByKids.Count > 0)
                SendEmailToAdmin(sevenDaysNoActivitiesByKids, EmailTemplateType.NoChoreCompletedSevenDays, EmailType.NoChoreCompletedSevenDays);
        }

        /// <summary>
        /// Notify continuous child activity
        /// </summary>
        public void NotifyContinuousChildActivity()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-8).Date;
            var fourteenDaysAgo = DateTime.UtcNow.AddDays(-15).Date;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-31).Date;
            var yesterDay = DateTime.UtcNow.AddDays(-1).Date;

            var sevenContinuousChildActivity = _choreService.GetContinuousChildActivity(sevenDaysAgo, yesterDay);
            if (sevenContinuousChildActivity.Count > 0)
                SendEmailToAdmin(sevenContinuousChildActivity, EmailTemplateType.SevenStraightDaysofActivity, EmailType.SevenStraightDaysofActivity);

            var fourteenContinuousChildActivity = _choreService.GetContinuousChildActivity(fourteenDaysAgo, yesterDay);
            if (fourteenContinuousChildActivity.Count > 0)
                SendEmailToAdmin(fourteenContinuousChildActivity, EmailTemplateType.TwoWeeksStraightDaysofActivity, EmailType.TwoWeeksStraightDaysofActivity);

            var oneMonthContinuousChildActivity = _choreService.GetContinuousChildActivity(thirtyDaysAgo, yesterDay);
            if (oneMonthContinuousChildActivity.Count > 0)
                SendEmailToAdmin(oneMonthContinuousChildActivity, EmailTemplateType.OneMonthStraightDaysofActivity, EmailType.OneMonthStraightDaysofActivity);
        }

        /// <summary>
        /// Notifies the admin about subscription renewal.
        /// </summary>
        public void NotifySubscriptionRenewal()
        {
            var adminMembers = _familyService.GetUpcomingSubscriptions();
            if (adminMembers != null)
                SendEmailToAdmin(adminMembers, EmailTemplateType.OneYearRenewalApproaching, EmailType.OneYearRenewalApproaching);
        }

        private void SendEmailToAdmin(List<FamilyMember> familyMembers, EmailTemplateType emailTemplateType, EmailType emailType)
        {
            var emailTemplate = _emailTemplateService.GetByType(emailTemplateType);
            foreach (var member in familyMembers)
            {
                if (member.IsUnSubscribed)
                    continue;

                // Send email
                _emailService.Send(member.User.Email, emailTemplate.Subject, emailTemplate.Content);
                // Save email history
                _emailHistoryService.SaveEmailHistory(member.Id, emailType);
            }
        }

        //public void NotifyVerifyPendingAccount()
        //{
        //    var memberId = adminMemberId ?? _currentUserService.MemberID;
        //    return Repository.Table<FinancialAccount>().Include(p => p.FamilyMember).Include(p => p.FamilyMember.User)
        //        .Include(p => p.FamilyMember.User.Family).SingleOrDefault(p => p.FamilyMemberID == memberId);
        //}

        public void NotifyVerifyPendingAccount()
        {
            int userFamilyMemberID = 0;
            string Email = string.Empty;
            DateTime dateAdded;
            var getMemberList = Repository.Table<FinancialAccount>().Include(p => p.FamilyMember).Include(p => p.FamilyMember.User)
               .Include(p => p.FamilyMember.User.Family).Where(p => p.Status == FinancialAccountStatus.Unverified).ToList();


            foreach (var item in getMemberList)
            {
                userFamilyMemberID = item.FamilyMemberID;

                var finencialAccountRowDetail = Repository.Table<User>().Where(p => p.Id == userFamilyMemberID).FirstOrDefault();
                Email = finencialAccountRowDetail.Email;
                dateAdded = item.DateAdded;
                DateTime newdateAdded = System.DateTime.Now.AddDays(-3);

                if (newdateAdded >= dateAdded)
                    NotificationVerifySendMail(Email);

                var finencialAccuntDetail = Repository.Table<FinancialAccount>().SingleOrDefault(p => p.FamilyMemberID == userFamilyMemberID);
                finencialAccuntDetail.isVerifyMailsent = true;
                Repository.Update(finencialAccuntDetail);

            }
        }

        public async void NotificationVerifySendMail(string Email)
        {
            var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.NotificationAccountVerify);
            var bodyContent = emailTemplate?.Content ?? "Notification Account Verify";
            await _emailService.Send(Email, emailTemplate.Subject, bodyContent);
        }



    }
}
