using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core.Dto.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Twilio.TwiML;

namespace LeapSpring.MJC.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/smsbot")]
    public class SmsBotController : ApiController
    {
        #region Fields

        private ISmsBotService _smsBotService;
        private IEmailService _emailService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="smsBotService">Sms bot service</param>
        public SmsBotController(ISmsBotService smsBotService, IEmailService emailService)
        {
            _smsBotService = smsBotService;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        // POST: api/smsbot/receive
        [HttpPost]
        [Route("receive")]
        public HttpResponseMessage Receive(SmsResponse smsResponse)
        {
            var twilioResponse = new TwilioResponse();
            _smsBotService.Receive(smsResponse);
            //twilioResponse.Sms("Test"); // Todo: Reply here
            return Request.CreateResponse(HttpStatusCode.OK, twilioResponse.Element, new MediaTypeHeaderValue("text/xml"));
        }

        

        // POST: api/smsbot/payment
        [HttpPost]
        [Route("payment")]
        public HttpResponseMessage Payment([FromBody]dynamic dynamic)
        {
            var test = HttpContext.Current.Request.Params;
            var test1 = HttpContext.Current;

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("sendmail")]
        async public Task<bool> Send()
        {
            var result = await _emailService.Send("", "", "");

            return true;
        }

        #endregion
    }
}
