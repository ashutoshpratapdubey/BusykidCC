using LeapSpring.MJC.BusinessLogic.Services.PhoneConfirmation;
using LeapSpring.MJC.Core.Dto.Accounts;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/phoneverification")]
    public class PhoneVerificationController : ApiController
    {
        private IPhoneConfirmationService _phoneConfirmationService;

        public PhoneVerificationController(IPhoneConfirmationService phoneConfirmationService)
        {
            _phoneConfirmationService = phoneConfirmationService;
        }

        [HttpGet]
        [Route("getverificationcode/{phoneNumber}")]
        public HttpResponseMessage GetVerificationCode(string phoneNumber)
        {
            _phoneConfirmationService.GetVerificationCode(phoneNumber);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("verifyCode")]
        public HttpResponseMessage VerifyOneTimePassword([FromBody]string verficationCode)
        {
            if (string.IsNullOrEmpty(verficationCode))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            var phoneNumberConfirmation = _phoneConfirmationService.VerifyCode(verficationCode);
            return Request.CreateResponse(!phoneNumberConfirmation.Equals("Verified") ? HttpStatusCode.BadRequest : HttpStatusCode.OK, phoneNumberConfirmation);
        }

        //[HttpPut]
        //[Route("verifyCode")]
        //public HttpResponseMessage VerifyOneTimePassword([FromBody]string verficationCode)
        //{
        //    //if (string.IsNullOrEmpty(verficationCode))
        //    //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

        //    //var phoneNumberConfirmation = _phoneConfirmationService.VerifyCode(verficationCode);
        //    var phoneNumberConfirmation = "Verified";
        //    return Request.CreateResponse(!phoneNumberConfirmation.Equals("Verified") ? HttpStatusCode.BadRequest : HttpStatusCode.OK, phoneNumberConfirmation);
        //}




    }
}
