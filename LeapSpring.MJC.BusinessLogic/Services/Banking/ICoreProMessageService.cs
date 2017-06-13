using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public interface ICoreProMessageService
    {
        /// <summary>
        /// Receives the message from corepro through azure bus service.
        /// </summary>
        /// 
        //void ReceiveMessage();
        void ReceiveMessage();
    }
}
