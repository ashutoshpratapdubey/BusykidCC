using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using Quartz;


namespace LeapSpring.MJC.Infrastructure.Jobs
{
    class updateChoreRecords : IJob
    {
        private IChoreService _makeChorePaymentService;

        public updateChoreRecords(IChoreService makeChorePaymentService)
        {
            _makeChorePaymentService = makeChorePaymentService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            _makeChorePaymentService.MakeChorePaymentToCorepro();
        }
    }
}
