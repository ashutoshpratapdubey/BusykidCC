using LeapSpring.MJC.Core.Domain.Email;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    /// <summary>
    /// Represents a interface of email template service
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Get email template by template type
        /// </summary>
        /// <param name="emailTemplateType">Email template type</param>
        /// <returns>Email template</returns>
        EmailTemplate GetByType(EmailTemplateType emailTemplateType);
    }
}
