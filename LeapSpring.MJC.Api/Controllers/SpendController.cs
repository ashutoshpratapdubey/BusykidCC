using LeapSpring.MJC.BusinessLogic.Services.Spend;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Dto.Spend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/spend")]
    public class SpendController : ApiController
    {
        #region Fields

        private ISpendService _spendService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="spendService">Spend service</param>
        public SpendController(ISpendService spendService)
        {
            _spendService = spendService;
        }

        #endregion

        // GET: api/spend/getgiftcards
        [HttpGet]
        [Route("getgiftcards")]
        public HttpResponseMessage GetGiftCards()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _spendService.GetGiftCards());
        }

        // GET: api/spend/getgiftcardpreview/{isFeatured}
        [HttpGet]
        [Route("getgiftcardpreviews/{isFeatured}")]
        public HttpResponseMessage GetGiftCardPreviews(bool isFeatured = false)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _spendService.GetGiftCardPreviews(isFeatured));
        }

        // GET: api/spend/purchasegiftcard/:giftCardId
        [HttpGet]
        [Route("purchasegiftcard/{giftCardId}")]
        public HttpResponseMessage PurchaseGiftCard(int giftCardId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _spendService.PurchaseGiftCardRequest(giftCardId));
        }

        // GET: api/spend/getpurchasedgiftcards
        [HttpGet]
        [Route("getpurchasedgiftcards")]
        public HttpResponseMessage GetPurchasedGiftCards()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _spendService.GetPurchasedGiftCards());
        }

        // GET: api/spend/deletepurchasedgiftcard
        [HttpDelete]
        [Route("deletepurchasedgiftcard/{purchasedGiftCardId}")]
        public HttpResponseMessage DeletePurchasedGiftCard(int purchasedGiftCardId)
        {
            _spendService.DeletePurchasedGiftCard(purchasedGiftCardId);
            return Request.CreateResponse(HttpStatusCode.OK, "Purchased gift card deleted successfully");
        }

        // PUT: api/spend/cashout
        [HttpPut]
        [Route("cashout")]
        public HttpResponseMessage CashOut([FromBody]CashOut cashOutRequest)
        {
            if (cashOutRequest == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            return Request.CreateResponse(HttpStatusCode.OK, _spendService.CashOutRequest(cashOutRequest));
        }
    }
}
