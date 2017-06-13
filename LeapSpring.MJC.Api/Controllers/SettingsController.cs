using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Domain.Settings;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeapSpring.MJC.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/settings")]
    public class SettingsController : ApiController
    {
        #region Fields

        private IAllocationSettingsService _allocationSettingsService;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="allocationSettingsService">Allocation settings service</param>
        /// <param name="accountService">Account service</param>
        /// <param name="currentUserService">Current user service</param>
        public SettingsController(IAllocationSettingsService allocationSettingsService, IAccountService accountService, ICurrentUserService currentUserService)
        {
            _allocationSettingsService = allocationSettingsService;
            _accountService = accountService;
            _currentUserService = currentUserService;
        }

        #endregion

        // GET: api/settings/getallocationsettings/{familyMemberId}
        [HttpGet]
        [Route("getallocationsettings/{familymemberid}")]
        public HttpResponseMessage GetAllocationSettings(int familyMemberId)
        {
            if (!_accountService.BelongsToThisFamily(familyMemberId))
                throw new UnauthorizedAccessException();

            return Request.CreateResponse(HttpStatusCode.OK, _allocationSettingsService.GetByMemberId(familyMemberId));
        }

        // GET: api/settings/getallocationbyage/{age}
        [HttpGet]
        [Route("getallocationbyage/{age}")]
        public HttpResponseMessage GetAllocationByAge(int age)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _allocationSettingsService.GetAllocationByAge(age));
        }

        // PUT: api/settings/updateallocationsettings
        [HttpPut]
        [Route("updateallocationsettings")]
        public HttpResponseMessage UpdateAllocationSettings([FromBody]AllocationSettings allocationSettings)
        {
            if (allocationSettings == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            _allocationSettingsService.Update(allocationSettings);
            return Request.CreateResponse(HttpStatusCode.OK, "Allocation settings saved successfully");
        }
    }
}
