using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    class CancelPandingPaydayRequest : IJob
    {

        private IChoreService _cancelChoreService;

        public CancelPandingPaydayRequest(IChoreService cancelChoreService)
        {
            _cancelChoreService = cancelChoreService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            _cancelChoreService.CancelChore();
        }

    }
}
