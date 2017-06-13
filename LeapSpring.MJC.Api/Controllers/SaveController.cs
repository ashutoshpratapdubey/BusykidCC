using LeapSpring.MJC.BusinessLogic.Services.Save;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Dto.Save;
using LeapSpring.MJC.Core.Filters;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/save")]
    public class SaveController : ApiController
    {
        #region Fields

        private ISaveService _saveService;

        #endregion

        #region Ctor

        public SaveController(ISaveService saveService)
        {
            _saveService = saveService;
        }

        #endregion

        // GET: api/save/getstockgiftcards/{isFeaturedStock}
        [HttpGet]
        [Route("getstockgiftcards/{isFeaturedStock}")]
        public HttpResponseMessage GetStockGiftCards(bool isFeaturedStock = false)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _saveService.GetStockGiftCards(isFeaturedStock));
        }

        // GET: api/save/getpurchasedstockgiftcards
        [HttpGet]
        [Route("getpurchasedstockgiftcards")]
        public HttpResponseMessage GetPurchasedStockGiftCards()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _saveService.GetPurchasedStockGiftCards());
        }

        // GET: api/save/getdisapprovedstockgiftcards
        [HttpGet]
        [Route("getdisapprovedstockgiftcards")]
        public HttpResponseMessage GetDisapprovedStockGiftCards()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _saveService.GetDisapprovedStockGiftCards());
        }

        // GET: api/save/getstockgiftquotes
        [HttpGet]
        [Route("getstockgiftquotes")]
        async public Task<IList<GiftStockQuote>> GetStockGiftQuotes()
        {
            return await _saveService.GetStockGiftQuotes();
        }

        // PUT: api/save/updatestockgiftquotes
        [HttpPut]
        [Route("updatestockgiftquotes")]
        async public Task UpdateStockGiftQuotes()
        {
            await _saveService.UpdateStockGiftQuotes();
        }

        // POST: api/save/puchasestock
        [HttpPost]
        [Route("initiatestockpurchase")]
        public HttpResponseMessage InitiateStockPurchase([FromBody]StockPurchaseRequest stockPurchaseRequest)
        {
            if (stockPurchaseRequest == null)
                throw new InvalidParameterException("Invalid parameters!");
            return Request.CreateResponse(HttpStatusCode.OK, _saveService.InitiateStockPurchase(stockPurchaseRequest));
        }
    }
}
