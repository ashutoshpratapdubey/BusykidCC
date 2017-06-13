using LeapSpring.MJC.Core.Dto.Sms;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    /// <summary>
    /// Represents a interface of sms bot service
    /// </summary>
    public interface ISmsBotService
    {
        /// <summary>
        /// Sms receive
        /// </summary>
        /// <param name="smsResponse">Sms response</param>
        void Receive(SmsResponse smsResponse);
        void ReceiveDummy(string oneValue, string twoValue);
    }
}
