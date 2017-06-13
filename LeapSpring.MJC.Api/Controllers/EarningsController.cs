using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Infrastructure;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Enums;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.Core.Filters;
using System.Threading.Tasks;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using System;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/earnings")]
    public class EarningsController : ApiController
    {
        #region Fields

        private IEarningsService _earningsService;

        public EarningsBucketType EarningsBucketType { get; set; }
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;



        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="earningsService">Earnings service</param>
        /// <param name="accountService">Account service</param>
        /// <param name="currentUserService">Current user service</param>
        public EarningsController(IEarningsService earningsService, IAccountService accountService, ICurrentUserService currentUserService)
        {
            _earningsService = earningsService;
            _accountService = accountService;
            _currentUserService = currentUserService;

        }

        #endregion

        #region Methods

        // GET: api/earnings/getchildfinancialoverview/{weekDay}/{familymemberid}
        [HttpGet]
        [Route("getchildfinancialoverview/{weekDay}/{familymemberid}")]
        public ChildFinancialOverview GetChildFinancialOverview(DayOfWeek weekDay, int? familyMemberId = null)
        {

            return _earningsService.GetChildFinancialOverview(weekDay, familyMemberId);
        }

        // GET: api/earnings/getbymemberid/{memberId}
        [HttpGet]
        [Route("getbymemberid/{memberId}")]
        public HttpResponseMessage GetByMemberId(int memberId)
        {
            if (!_accountService.BelongsToThisFamily(memberId))
                throw new UnauthorizedAccessException();

            return Request.CreateResponse(HttpStatusCode.OK, _earningsService.GetByMemberId(memberId));
        }

        // GET: api/earnings/movemoney?{:sourceBucket}&{:destinationBucket}&{:amount}
        [HttpGet]
        [Route("movemoney")]
        public HttpResponseMessage MoveMoney(int sourceBucket, int destinationBucket, decimal amount)
        {
            if (sourceBucket <= 0 || destinationBucket <= 0 || amount <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            var childEarnings = _earningsService.MoveMoney((EarningsBucketType)sourceBucket, (EarningsBucketType)destinationBucket, amount);
            return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Money moved successfully", Earnings = childEarnings });
        }

        // GET: api/earnings/getbuckettypes
        [HttpGet]
        [Route("getbuckettypes")]
        public HttpResponseMessage GetBucketTypes()
        {
            return Request.CreateResponse(HttpStatusCode.OK, EarningsBucketType.EnumToSelectItem());
        }

        // GET: api/earnings/sendbonus
        [HttpPut]
        [Route("sendbonus")]
        public void SendBonus(ChildBonus bonus)
        {
            if (bonus == null)
                throw new InvalidParameterException("Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            _earningsService.SendBonus(bonus);
        }

        // api/transactionhistory/getallowancein
        [HttpGet]
        [Route("removeapproval/{choreId}")]
        public HttpResponseMessage RemoveApproval(int choreId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _earningsService.RemoveApprovalService(choreId));
        }

        [HttpGet]
        [Route("approveforpayeday/{choreId}")]
        public HttpResponseMessage approveForpayeday(int choreId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _earningsService.ApproveForPayday(choreId));
        }
        #endregion
    }
}
