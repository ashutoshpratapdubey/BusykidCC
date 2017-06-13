using LeapSpring.MJC.Data.Repository;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    /// <summary>
    /// Represents a email service
    /// </summary>
    public class EmailService : ServiceBase, IEmailService
    {
        private string _apiKey;
        private string _fromMail;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">DB Repository</param>
        public EmailService(IRepository repository) : base(repository)
        {
            _apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            _fromMail = ConfigurationManager.AppSettings["FromMail"];
        }

        #region Utilities

        /// <summary>
        /// Prepare email template values
        /// </summary>
        /// <param name="bodyContent">Email body content</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="resetLink">Reset link</param>
        /// <returns>Body content</returns>
        private string SetSubscriptionLink(string bodyContent)
        {
            var emailSubscriptionLink = HttpContext.Current != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "#/myaccount/emailSubscription" : string.Empty;
            bodyContent = bodyContent.Replace("{{emailSubscriptionLink}}", emailSubscriptionLink);

            return bodyContent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send email through send grid
        /// </summary>
        /// <param name="to">To mail</param>
        /// <param name="subject">Subject</param>
        /// <param name="content">Body content</param>
        /// <returns>Result</returns>
        async public Task<bool> Send(string to, string subject, string content)
        {
            // Adds the email subscription link.
            content = SetSubscriptionLink(content);

            SendGridAPIClient sendGrid = new SendGridAPIClient(_apiKey);

            Email from = new Email(_fromMail, "BusyKid");
            Email toMail = new Email(to);
            Content sgContent = new Content("text/html", content);
            Mail mail = new Mail(from, subject ?? "BusyKid", toMail, sgContent);
            //mail.AddAttachment(); // Todo attachment
            var result = await sendGrid.client.mail.send.post(requestBody: mail.Get());

            return result?.StatusCode == HttpStatusCode.Accepted ?? false;
        }
        #endregion


    }
}
