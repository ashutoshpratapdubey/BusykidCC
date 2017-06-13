using LeapSpring.MJC.Core.Domain.Email;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    /// <summary>
    /// Represents a email template service
    /// </summary>
    public class EmailTemplateService : ServiceBase, IEmailTemplateService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">DB Repository</param>
        public EmailTemplateService(IRepository repository) : base(repository)
        {
        }

        #region Methods

        /// <summary>
        /// Get email template by template type
        /// </summary>
        /// <param name="emailTemplateType">Email template type</param>
        /// <returns>Email template</returns>
        public EmailTemplate GetByType(EmailTemplateType emailTemplateType)
        {
            if (emailTemplateType == EmailTemplateType.IAVWelcome)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.MicroDepositWelcome)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.IncompleteNewMemberEnrollment)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.OneYearRenewalApproaching)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.OneYearRenewal)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.AccountCancellation)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.PasswordReset)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.NotificationAccountVerify)
               return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.Pinupdated)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else if (emailTemplateType == EmailTemplateType.PasswordUpdate)
                return Repository.Table<EmailTemplate>().FirstOrDefault(m => m.EmailTemplateType == emailTemplateType);
            else
                return null;
        }

        #endregion
    }
}
