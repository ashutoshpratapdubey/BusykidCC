using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifyNoChoreCompletedJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifyNoChoreCompletedJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifyNoChoreCompleted();
        }
    }
}
