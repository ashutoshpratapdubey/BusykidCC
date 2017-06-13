using System;
using Quartz;
using LeapSpring.MJC.BusinessLogic.Services.Sms;

namespace LeapSpring.MJC.Infrastructure.Jobs
{
    public class CancelNotRespondedSMSJob : IJob
    {
        private ISMSApprovalService _smsApprovalService;

        public CancelNotRespondedSMSJob(ISMSApprovalService smsApprovalService)
        {
            _smsApprovalService = smsApprovalService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _smsApprovalService.CancelNotRespondedSMS();
        }
    }
}
