using System;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using LeapSpring.MJC.Core.Domain.Sms;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public class SMSApprovalHistory : ServiceBase, ISMSApprovalHistory
    {
        #region Fields

        private ITextMessageService _textMessageService;

        #endregion

        #region Ctor

        public SMSApprovalHistory(IRepository repository, ITextMessageService textMessageService) : base(repository)
        {
            _textMessageService = textMessageService;
        }

        #endregion

        /// <summary>
        /// Adds the sms approval.
        /// </summary>
        /// <param name="familyId">The admin member identifier.</param>
        /// <param name="approvalType">The sms approval</param>
        /// <param name="message">The message to send</param>
        /// <param name="purchaseTransactionId">The purchase transaction or payday transaction identifier</param>
        /// <returns>The sms approval</returns>
        public SMSApproval Add(int adminMemberId, ApprovalType approvalType, string message, int? purchaseTransactionId = null)
        {
            var smsApproval = new SMSApproval
            {
                ApprovalType = approvalType,
                IsActive = true,
                Message = message,
                StockPurchaseRequestID = (approvalType == ApprovalType.StockPurchase) ? purchaseTransactionId : null,
                CashOutID = (approvalType == ApprovalType.CashOut) ? purchaseTransactionId : null,
                DonationID = (approvalType == ApprovalType.CharityDonation) ? purchaseTransactionId : null,
                PurchasedGiftCardID = (approvalType == ApprovalType.GiftPurchase) ? purchaseTransactionId : null,
                FamilyMemberID = adminMemberId,
                LastSentOn = DateTime.UtcNow
            };

            Repository.Insert(smsApproval);

            return smsApproval;
        }

        public SMSApproval AddLessAmount(int adminMemberId, ApprovalType approvalType, string message, int? purchaseTransactionId = null)
        {
            var smsApproval = new SMSApproval
            {
                ApprovalType = approvalType,
                IsActive = true,
                Message = message,
                StockPurchaseRequestID = (approvalType == ApprovalType.StockPurchase) ? purchaseTransactionId : null,
                CashOutID = (approvalType == ApprovalType.CashOut) ? purchaseTransactionId : null,
                DonationID = (approvalType == ApprovalType.CharityDonation) ? purchaseTransactionId : null,
                PurchasedGiftCardID = (approvalType == ApprovalType.GiftPurchase) ? purchaseTransactionId : null,
                FamilyMemberID = adminMemberId,
                LastSentOn = DateTime.UtcNow,
                IsReminded = true
            };

            Repository.Insert(smsApproval);

            return smsApproval;
        }
        /// <summary>
        /// Gets the current sms approval request.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The sms approval.</returns>
        public SMSApproval GetRecentApprovalRequest(int familyMemberId)
        {
            return Repository.Table<SMSApproval>()
               .Where(p => p.FamilyMemberID == familyMemberId && p.IsActive)
               .OrderByDescending(p => p.LastSentOn)
               .FirstOrDefault();
        }


        public void UpdateSMSReasponse(int familyMemberID,bool smsResponse)
        {
            var updateSMSResponse = Repository.Table<SMSApproval>()
                .Where(p => p.ApprovalType == ApprovalType.ChorePayment && p.FamilyMemberID == familyMemberID && p.IsActive && !p.IsReminded && p.LastSentOn <= DateTime.UtcNow)
                .FirstOrDefault();
            updateSMSResponse.responseStatus = smsResponse;
            Repository.Update(updateSMSResponse);
        }
        /// <summary>
        /// Reminds the approval process to the admin.
        /// </summary>
        public void RemindApproval()
        {
            var past4HoursFromNow = DateTime.UtcNow.AddHours(-4);
            var pendingSMSApprovals = Repository.Table<SMSApproval>()
                .Include(p => p.FamilyMember)
                .Where(p => p.IsActive && !p.IsReminded && p.LastSentOn <= past4HoursFromNow && p.ApprovalType != ApprovalType.ChorePayment)
                .ToList();

            foreach (var smsApproval in pendingSMSApprovals)
            {
                if (string.IsNullOrEmpty(smsApproval.FamilyMember.PhoneNumber) || string.IsNullOrEmpty(smsApproval.Message))
                    continue;

                var message = smsApproval.Message + "\nThis request will be automatically disapproved in 4 hours if no action is taken.";
                _textMessageService.Send(smsApproval.FamilyMember.PhoneNumber, message);

                smsApproval.LastSentOn = smsApproval.LastSentOn.AddHours(4);
                smsApproval.IsReminded = true;
                Repository.Update(smsApproval);
            }
        }

        /// <summary>
        /// Reminds the chore payment approval process to the admin.
        /// </summary>
        public void RemindChorePaymentApproval()
        {
            var pendingSMSApprovals = Repository.Table<SMSApproval>()
                .Include(p => p.FamilyMember)
                .Where(p => p.ApprovalType == ApprovalType.ChorePayment && p.IsActive && !p.IsReminded && p.LastSentOn <= DateTime.UtcNow)
                .ToList();

            foreach (var smsApproval in pendingSMSApprovals)
            {
                if (string.IsNullOrEmpty(smsApproval.FamilyMember.PhoneNumber) || string.IsNullOrEmpty(smsApproval.Message))
                    continue;

                _textMessageService.Send(smsApproval.FamilyMember.PhoneNumber, smsApproval.Message);
                smsApproval.LastSentOn = DateTime.UtcNow;
                smsApproval.IsReminded = true;
                Repository.Update(smsApproval);
            }
        }

        /// <summary>
        /// Gets the not responded sms approvals
        /// </summary>
        /// <param name="isChorePayment">This is chore payment. Default value is <c>False</c>. </param>
        /// <returns>The list of sms approvals.</returns>
        public List<SMSApproval> GetNotRespondedSMSApprovals(bool isChorePayment = false)
        {
            if (isChorePayment)
            {
                return Repository.Table<SMSApproval>()
                .Include(p => p.FamilyMember)
                .Include(p => p.FamilyMember.User)
                .Where(p => p.ApprovalType == ApprovalType.ChorePayment && p.IsActive && p.IsReminded && p.LastSentOn <= DateTime.UtcNow)
                .ToList();
            }

            var past4HoursFromNow = DateTime.UtcNow.AddHours(-4);
            return Repository.Table<SMSApproval>()
                .Include(p => p.FamilyMember)
                .Where(p => p.IsActive && p.IsReminded && p.LastSentOn <= past4HoursFromNow && p.ApprovalType != ApprovalType.ChorePayment)
                .ToList();
        }

        /// <summary>
        /// Deactivates the sms approval.
        /// </summary>
        /// <param name="smsApprovalId">The sms approval identifier.</param>
        public void MarkAsNotActive(int smsApprovalId)
        {
            var smsApproval = Repository.Table<SMSApproval>().SingleOrDefault(p => p.Id == smsApprovalId);
            if (smsApproval == null)
                return;
            smsApproval.IsActive = false;
            Repository.Update(smsApproval);
        }
    }
}
