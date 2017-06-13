using LeapSpring.MJC.BusinessLogic.Services.RecurringChore;
using LeapSpring.MJC.Core.Enums;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class CreateWeeklyChores : IJob
    {
        private readonly IRecurringChoreService _recurringChoreService;

        public CreateWeeklyChores(IRecurringChoreService recurringChoreService)
        {
            _recurringChoreService = recurringChoreService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _recurringChoreService.CreateChores(FrequencyType.Weekly, null, null, true);
        }
    }
}
