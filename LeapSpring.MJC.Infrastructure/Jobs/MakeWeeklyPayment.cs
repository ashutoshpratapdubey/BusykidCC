using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class MakeWeeklyPayment : IJob
    {
        private readonly IEarningsService _earningsService;
        private readonly ISMSApprovalService _smsApprovalService;

        public MakeWeeklyPayment(IEarningsService earningsService, ISMSApprovalService smsApprovalService)
        {
            _earningsService = earningsService;
            _smsApprovalService = smsApprovalService;
        }

        public void Execute(IJobExecutionContext context)
        {
            // Cancels the un-approved chore payments. (i.e., Disaproves the not responded chore payment notifications.)
            _smsApprovalService.CancelNotRespondedSMS(true);
        }
    }
}
