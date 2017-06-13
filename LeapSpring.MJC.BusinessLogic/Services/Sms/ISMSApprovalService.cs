using LeapSpring.MJC.Core.Domain.Family;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public interface ISMSApprovalService
    {
        /// <summary>
        /// Approves the request.
        /// </summary>
        /// <param name="adminMember">The admin member</param>
        /// <returns>The message</returns>
        Task<string> Approve(FamilyMember adminMember);

        /// <summary>
        /// Disapproves the request.
        /// </summary>
        /// <param name="adminMember">The admin member</param>
        /// <returns>The message</returns>
        string Disapprove(FamilyMember adminMember);

        /// <summary>
        /// Approves the chores
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <returns>The message</returns>
        string ApproveChores(FamilyMember adminMember);

        /// <summary>
        /// Cancel the not responded sms approvals.
        /// </summary>
        /// <param name="isChorePayment">This is chore payment. Default value is <c>False</c>. </param>
        /// <returns></returns>
        void CancelNotRespondedSMS(bool isChorePayment = false);
    }
}
