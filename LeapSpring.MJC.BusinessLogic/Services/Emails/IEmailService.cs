using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Emails
{
    /// <summary>
    /// Represents a interface of email service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send email through send grid
        /// </summary>
        /// <param name="to">To mail</param>
        /// <param name="subject">Subject</param>
        /// <param name="content">Body content</param>
        /// <returns>Result</returns>
        Task<bool> Send(string to, string subject, string content);
    }
}
