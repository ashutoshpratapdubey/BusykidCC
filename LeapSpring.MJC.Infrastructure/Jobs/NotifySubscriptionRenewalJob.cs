using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifySubscriptionRenewalJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifySubscriptionRenewalJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifySubscriptionRenewal();
        }
    }
}
