using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class SendWeeklyPaydayStatus: IJob
    {
        private readonly INotificationService _notificationService;

        public SendWeeklyPaydayStatus(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.SendPayDayMessage();
        }

    }
}
