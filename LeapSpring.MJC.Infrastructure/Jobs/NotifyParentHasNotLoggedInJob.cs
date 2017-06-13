using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifyParentHasNotLoggedInJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifyParentHasNotLoggedInJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifyParentHasNotLoggedIn();
        }
    }
}