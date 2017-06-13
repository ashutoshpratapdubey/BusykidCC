using LeapSpring.MJC.BusinessLogic.Services.Sms;
using Quartz;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class RemindSMSApprovalJob : IJob
    {
        private ISMSApprovalHistory _smsApprovalHistory;

        public RemindSMSApprovalJob(ISMSApprovalHistory smsApprovalHistory)
        {
            _smsApprovalHistory = smsApprovalHistory;
        }

        public void Execute(IJobExecutionContext context)
        {
            _smsApprovalHistory.RemindApproval();
        }
    }
}
