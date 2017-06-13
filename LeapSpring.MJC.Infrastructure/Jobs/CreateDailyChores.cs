using LeapSpring.MJC.BusinessLogic.Services.RecurringChore;
using LeapSpring.MJC.Core.Enums;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class CreateDailyChores : IJob
    {
        private readonly IRecurringChoreService _recurringChoreService;

        public CreateDailyChores(IRecurringChoreService recurringChoreService)
        {
            _recurringChoreService = recurringChoreService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _recurringChoreService.CreateChores(FrequencyType.Daily, null, null, true);
        }
    }
}
