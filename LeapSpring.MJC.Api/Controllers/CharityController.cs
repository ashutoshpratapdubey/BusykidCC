using LeapSpring.MJC.BusinessLogic.Services.Charities;
using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    //[Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/charity")]
    public class CharityController : ApiController
    {
        private ICharityService _charityService;

        public CharityController(ICharityService charityService)
        {
            _charityService = charityService;
        }

        // api/charity/getcharities
        [HttpGet]
        [Route("getcharities")]
        public IList<Charity> GetCharities()
        {
            return _charityService.GetCharities();
        }

        // api/charity/donate
        [HttpPost]
        [Route("donate")]
        public Donation Donate(Donation donation)
        {
            if (donation == null)
                throw new InvalidParameterException("Invalid parameters!");

            return _charityService.Donate(donation);
        }
    }
}
