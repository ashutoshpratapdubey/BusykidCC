using Quartz;
using LeapSpring.MJC.BusinessLogic.Services.Notification;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotificationAccountVerify : IJob
    {
        private readonly INotificationService _NotificationService;

        public void Execute(IJobExecutionContext context)
        {
            _NotificationService.NotifyVerifyPendingAccount();
        }
    }
}
