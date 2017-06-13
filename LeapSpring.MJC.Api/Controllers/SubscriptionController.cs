using LeapSpring.MJC.BusinessLogic.Services.SubscriptionService;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/subscription")]
    public class SubscriptionController : ApiController
    {
        #region Fields

        private ISubscriptionService _subscriptionService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionService"></param>
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        #endregion

        // Get: api/subscription/getsubscription
        [HttpGet]
        [Route("getsubscription")]
        public HttpResponseMessage GetSubscription()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _subscriptionService.GetById());
        }

        // Get: api/subscription/getsubscriptionstatus
        [HttpGet]
        [Route("getsubscriptionstatus")]
        public HttpResponseMessage GetSubscriptionStatus()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _subscriptionService.GetSubscriptionStatus());
        }

        // Get: api/subscription/validatepromocode
        [HttpGet]
        [Route("validatepromocode/{promoCode}")]
        public HttpResponseMessage ValidatePromoCode(string promoCode)
        {
            if (string.IsNullOrEmpty(promoCode)) throw new InvalidParameterException("Invalid parameters!");

            return Request.CreateResponse(HttpStatusCode.OK, _subscriptionService.ValidatePromoCode(promoCode));
        }

        // Post: api/subscription/subscribe
        [HttpPost]
        [Route("subscribe")]
        public HttpResponseMessage Subscribe([FromBody] Subscription subscription)
        {
            if (subscription == null) throw new InvalidParameterException("Invalid parameters!");
            _subscriptionService.Subscribe(subscription);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT: api/subscription/cancelsubscription
        [HttpPut]
        [Route("cancelsubscription")]
        public async Task<HttpResponseMessage> CancelSubscription()
        {
            await _subscriptionService.CancelSubscription();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
