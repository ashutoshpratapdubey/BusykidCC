using LeapSpring.MJC.BusinessLogic.Services.Banking;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class CoreProMessageJob : IJob
    {
        private ICoreProMessageService _coreProMessageService;

        public CoreProMessageJob(ICoreProMessageService coreProMessageService)
        {
            _coreProMessageService = coreProMessageService;
        }   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            _coreProMessageService.ReceiveMessage();
        }
    }
}
