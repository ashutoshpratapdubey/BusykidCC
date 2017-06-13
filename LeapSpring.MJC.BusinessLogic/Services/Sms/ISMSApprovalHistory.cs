using LeapSpring.MJC.Core.Domain.Sms;
using LeapSpring.MJC.Core.Enums;
using System.Collections.Generic;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public interface ISMSApprovalHistory
    {
        /// <summary>
        /// Adds the sms approval.
        /// </summary>
        /// <param name="familyId">The admin member identifier.</param>
        /// <param name="approvalType">The sms approval</param>
        /// <param name="message">The message to send</param>
        /// <param name="purchaseTransactionId">The purchase transaction or payday transaction identifier</param>
        /// <returns>The sms approval</returns>
        SMSApproval Add(int adminMemberId, ApprovalType approvalType, string message, int? purchaseTransactionId = null);

        /// <summary>
        /// Gets the recent sms approval request.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The sms approval.</returns>
        SMSApproval GetRecentApprovalRequest(int familyMemberId);

        void UpdateSMSReasponse(int familymemberID, bool smsResponse);
        /// <summary>
        /// Reminds the approval process to the admin.
        /// </summary>
        void RemindApproval();

        /// <summary>
        /// Reminds the chore payment approval process to the admin.
        /// </summary>
        void RemindChorePaymentApproval();

        /// <summary>
        /// Gets the not responded sms approvals
        /// </summary>
        /// <param name="isChorePayment">This is chore payment. Default value is <c>False</c>. </param>
        /// <returns>The list of sms approvals.</returns>
        List<SMSApproval> GetNotRespondedSMSApprovals(bool isChorePayment = false);

        /// <summary>
        /// Deactivates the sms approval.
        /// </summary>
        /// <param name="smsApprovalId">The sms approval identifier.</param>
        void MarkAsNotActive(int smsApprovalId);
        SMSApproval AddLessAmount(int adminMemberId, ApprovalType approvalType, string message, int? purchaseTransactionId = null);
    }
}
