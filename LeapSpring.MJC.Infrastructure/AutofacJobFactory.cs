using Autofac;
using Autofac.Core.Lifetime;
using Quartz;
using Quartz.Spi;

namespace LeapSpring.MJC.Infrastructure
{
    public class AutofacJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public AutofacJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            using (var scope = _container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
            {
                return (IJob)scope.Resolve(bundle.JobDetail.JobType);
            }
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
