namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public interface ITextMessageService
    {
        /// <summary>
        /// Sends the text message.
        /// </summary>
        /// <param name="toPhoneNumber">The Reciepient Phone Number.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>The Message Status.</returns>
        string Send(string toPhoneNumber, string message);
    }
}
