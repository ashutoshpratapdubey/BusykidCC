using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.Core.Dto;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/transactionhistory")]
    public class TransactionHistoryController : ApiController
    {
        #region Fields

        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IAccountService _accountService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="transactionHistoryService">Transaction history service</param>
        /// <param name="accountService">Account service</param>
        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService, IAccountService accountService)
        {
            _transactionHistoryService = transactionHistoryService;
            _accountService = accountService;
        }

        #endregion

        #region Methods

        // api/transactionhistory/getalltransactions
        [HttpGet]
        [Route("getalltransactions/{familyMemberId}")]
        public Dictionary<DateTime, List<TransactionHistory>> GetAllTransactions(int? familyMemberId = null)
        {
            if (familyMemberId.HasValue && !_accountService.BelongsToThisFamily(familyMemberId.Value))
                throw new UnauthorizedAccessException();

            return _transactionHistoryService.GetAllTransactions(familyMemberId);
        }

        // api/transactionhistory/getallowancein
        [HttpGet]
        [Route("getallowancein/{familyMemberId}")]
        public Dictionary<DateTime, List<TransactionHistory>> GetAllowanceIn(int? familyMemberId = null)
        {
            if (familyMemberId.HasValue && !_accountService.BelongsToThisFamily(familyMemberId.Value))
                throw new UnauthorizedAccessException();

            return _transactionHistoryService.GetAllowanceIn(familyMemberId);
        }

        // api/transactionhistory/getallowanceout
        [HttpGet]
        [Route("getallowanceout/{familyMemberId}")]
        public Dictionary<DateTime, List<TransactionHistory>> GetAllowanceOut(int? familyMemberId = null)
        {
            if (familyMemberId.HasValue && !_accountService.BelongsToThisFamily(familyMemberId.Value))
                throw new UnauthorizedAccessException();

            return _transactionHistoryService.GetAllowanceOut(familyMemberId);
        }

        #endregion

    }
}
