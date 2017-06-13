using LeapSpring.MJC.Data.Repository;
using System;
using System.Linq;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using System.Collections.Generic;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public class SendMessageService : ServiceBase, ISendMessageService
    {
        private readonly ICurrentUserService _currentUserService;
        private ITextMessageService _textMessageService;
        private ISendMessageService _ISendMessageService;
        public SendMessageService(IRepository repository, ICurrentUserService currentUserService, ITextMessageService textMessageService, ISendMessageService SendMessageService) : base(repository)
        {
            _textMessageService = textMessageService;
            _currentUserService = currentUserService;
            _ISendMessageService = SendMessageService;
        }
        public void SendMessagePaydayNotProceedService()
        {
            //string message = string.Empty;
            //string adminnumber = "";
            //var dtTodayUtc = DateTime.UtcNow;
            //var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            //var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            //var nextPayDayOld = DateTime.UtcNow.Date;
            //DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;
            //var chkStartDate = cstTimeZoneTime.getStartDate();
            //var nextPayDateCheck = cstTimeZoneTime.getEndDate();

            //var StatusDetail = Repository.Table<Statuslog>().Where(p => p.currentdate >= chkStartDate && p.currentdate < nextPayDateCheck).ToList();

            //if (StatusDetail.Count > 0)
            //{
            //    foreach (var item in StatusDetail)
            //    {
            //        var childmember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.UserID.Equals(item.ChildFamilyMemberID) && !p.IsDeleted);
            //        var familymember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.UserID.Equals(item.ParentFamilyMemberID));
            //        message = childmember.Firstname + " " + "payday cannot be processed since they have a payday balance of" + " " + item.Amount + ".";
            //        adminnumber = familymember.PhoneNumber;
            //        if (item.Status == 1)
            //        {
            //            if (!string.IsNullOrEmpty(adminnumber))
            //                _textMessageService.Send(adminnumber, message);
            //        }
            //    }
            //}

        }
    }
}
