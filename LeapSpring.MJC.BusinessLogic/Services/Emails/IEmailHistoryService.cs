using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    public interface IEmailHistoryService
    {
        /// <summary>
        /// Save email history
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="emailType">Email template type</param>
        void SaveEmailHistory(int familyMemberId, EmailType emailType);

        /// <summary>
        /// Has sent email
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="emailType">Email template type</param>
        /// <returns>Result</returns>
        bool HasSent(int familyMemberId, EmailType emailType);
    }
}