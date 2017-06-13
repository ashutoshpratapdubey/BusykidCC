using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core;
using Twilio;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public class TextMessageService : ITextMessageService
    {
        private IAppSettingsService _appSettingsService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public TextMessageService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        /// <summary>
        /// Sends the text message.
        /// </summary>
        /// <param name="toPhoneNumber">The Reciepient Phone Number.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>The Message Status.</returns>
        public string Send(string toPhoneNumber, string message)
        {
            toPhoneNumber = toPhoneNumber.AppendCountyCode();
            var twilioClient = new TwilioRestClient(_appSettingsService.TwilioAccountSid, _appSettingsService.TwilioAuthToken); // Authenticate twilio account
            var smsStatus = twilioClient.SendMessage(_appSettingsService.TwilioPhoneNumber, toPhoneNumber, "BusyKid: " + message); // Send Message
            var textStatus = smsStatus?.Status ?? "Failed";
            return textStatus;
        }
    }
}
