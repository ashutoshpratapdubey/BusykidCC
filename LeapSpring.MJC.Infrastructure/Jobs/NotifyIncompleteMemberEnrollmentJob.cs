using LeapSpring.MJC.BusinessLogic.Services.Notification;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class NotifyIncompleteMemberEnrollmentJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotifyIncompleteMemberEnrollmentJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.NotifyIncompleteNewMemberEntrollment();
        }
    }
}
