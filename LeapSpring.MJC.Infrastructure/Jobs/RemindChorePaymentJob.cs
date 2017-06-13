using LeapSpring.MJC.BusinessLogic.Services.Sms;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class RemindChorePaymentJob : IJob
    {
        private ISMSApprovalHistory _smsApprovalHistory;

        public RemindChorePaymentJob(ISMSApprovalHistory smsApprovalHistory)
        {
            _smsApprovalHistory = smsApprovalHistory;
        }

        public void Execute(IJobExecutionContext context)
        {
            _smsApprovalHistory.RemindChorePaymentApproval();
        }
    }
}
