using LeapSpring.MJC.BusinessLogic.Services.Sms;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    class SendMessagePaydayNotProceed : IJob
    {
        private ISendMessageService _ISendMessageService;

        public SendMessagePaydayNotProceed(ISendMessageService SendMessageService)
        {
            _ISendMessageService = SendMessageService;
        }
        public void Execute(IJobExecutionContext context)
        {
            _ISendMessageService.SendMessagePaydayNotProceedService();
        }
    }
}
