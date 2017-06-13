using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifyContinuousChildActivityJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifyContinuousChildActivityJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifyContinuousChildActivity();
        }
    }
}
