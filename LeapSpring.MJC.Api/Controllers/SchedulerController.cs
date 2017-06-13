using LeapSpring.MJC.Infrastructure;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    // Todo: test controller
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/scheduler")]
    public class SchedulerController : ApiController
    {
        // Todo: test method
        // GET: api/scheduler/makepayment
        [HttpGet]
        [Route("makepayment")]
        public void StartTestScheduler()
        {
            JobScheduler.IsStarted = true;
            JobScheduler.Start();
        }

        // Todo: test method
        // GET: api/scheduler/sendpaydaystatus
        [HttpGet]
        [Route("sendpaydaystatus")]
        public void StartSendPayDayStatusJob()
        {
            JobScheduler.IsPayDayStatusSent = true;
            JobScheduler.StartSendPayDayStatusJob();
        }

        // Todo: test method
        // GET: api/scheduler/renewsubscription
        [HttpGet]
        [Route("renewsubscription")]
        public void RenewSubscription()
        {
            JobScheduler.IsRenewSubscription = true;
            JobScheduler.StartRenewSubscriptonJob();
        }

        // Todo: test method
        // GET: api/scheduler/notifiyrenewsubscription
        [HttpGet]
        [Route("notifiyrenewsubscription")]
        public void NotifyRenewSubscription()
        {
            JobScheduler.IsRenewSubscriptionNotification = true;
            JobScheduler.StartSubscriptonRenwalNotificationJob();
        }

        // Todo: test method
        // GET: api/scheduler/remindchorepayment
        [HttpGet]
        [Route("remindchorepayment")]
        public void RemindChorePayment()
        {
            JobScheduler.IsRemindChorePayment = true;
            JobScheduler.StartRemindChorePaymentJob();
        }
    }
}
