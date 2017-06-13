using LeapSpring.MJC.Core.Domain.Email;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Linq;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    /// <summary>
    /// Represents a email history service
    /// </summary>
    public class EmailHistoryService : ServiceBase, IEmailHistoryService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">DB Repository</param>
        public EmailHistoryService(IRepository repository) : base(repository)
        {
        }

        /// <summary>
        /// Save email history
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="emailType">Email template type</param>
        public void SaveEmailHistory(int familyMemberId, EmailType emailType)
        {
            var emailHistory = new EmailHistory
            {
                FamilyMemberID = familyMemberId,
                EmailType = emailType,
                TriggeredOn = DateTime.UtcNow
            };

            Repository.Insert(emailHistory);
        }

        /// <summary>
        /// Has sent email
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="emailType">Email template type</param>
        /// <returns>Result</returns>
        public bool HasSent(int familyMemberId, EmailType emailType)
        {
            return Repository.Table<EmailHistory>().Any(m => m.FamilyMemberID == familyMemberId && m.EmailType == emailType);
        }
    }
}
