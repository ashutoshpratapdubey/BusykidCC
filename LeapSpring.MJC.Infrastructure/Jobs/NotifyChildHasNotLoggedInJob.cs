using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifyChildHasNotLoggedInJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifyChildHasNotLoggedInJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifyChildHasNotLoggedIn();
        }
    }
}
