using LeapSpring.MJC.BusinessLogic.Services.SubscriptionService;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    class RenewSubscriptionJob : IJob
    {
        private ISubscriptionService _subscriptionService;

        public RenewSubscriptionJob(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public void Execute(IJobExecutionContext context)
        {
            _subscriptionService.RenewSubscription();
        }
    }
}
