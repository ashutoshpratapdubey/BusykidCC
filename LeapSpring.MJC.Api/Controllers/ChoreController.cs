using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.Core.Domain.Chore;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Cors;
using LeapSpring.MJC.BusinessLogic.Services.RecurringChore;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Dto.Chores;
using LeapSpring.MJC.Core;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/chore")]
    public class ChoreController : ApiController
    {
        #region Fields

        private readonly IChoreService _choreService;
        private readonly IRecurringChoreService _recurringChoreService;
        private readonly ICurrentUserService _currentUserService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="choreService"></param>
        /// <param name="recurringChoreService"></param>
        /// <param name="currentUserService">Current user service</param>
        public ChoreController(IChoreService choreService, IRecurringChoreService recurringChoreService, ICurrentUserService currentUserService)
        {
            _choreService = choreService;
            _recurringChoreService = recurringChoreService;
            _currentUserService = currentUserService;
        }

        #endregion

        #region Methods

        [HttpPut]
        [Route("addchore/{choreCreatedDay}")]
        public HttpResponseMessage Add([FromBody]Chore chore, DayOfWeek choreCreatedDay)
        {
            if (chore == null)
                throw new InvalidParameterException("Please provide a valid chore");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            chore = _choreService.Add(chore, null, choreCreatedDay);
            if (chore.FrequencyType != FrequencyType.Once)
                _recurringChoreService.CreateChores(chore.FrequencyType, chore.Id, _currentUserService.FamilyID, false, choreCreatedDay);

            return Request.CreateResponse(HttpStatusCode.OK, chore);
        }

        [HttpPut]
        [Route("update/{choreEditedDay}")]
        public Chore UpdateChore([FromBody]Chore chore, DayOfWeek? choreEditedDay = null)
        {
            if (chore == null)
                throw new InvalidParameterException("Invalid parameters!");

            chore = _choreService.Update(chore, choreEditedDay);

            if (chore.FrequencyType != FrequencyType.Once)
                _recurringChoreService.UpdateRecurringChore(chore.Id, chore.FrequencyType, _currentUserService.FamilyID, choreEditedDay);

            return chore;
        }

        // api/chore/delete/0
        [HttpDelete]
        [Route("delete/{choreId}")]
        public void DeleteChore(int choreId)
        {
            if (choreId == 0)
                throw new InvalidParameterException("Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            _choreService.Delete(choreId);
        }

        [HttpGet]
        [Route("getsystemchores")]
        public IList<SystemChore> GetSystemChores()
        {
            return _choreService.GetSystemChores();
        }

        [HttpGet]
        [Route("getsystemchoresbyagerange")]
        public SuggestedChores GetSystemChoresByAgeRange(int memberId, int skipCount)
        {
            return _choreService.GetSystemChoresByAgeRange(memberId, skipCount);
        }

        [HttpGet]
        [Route("searchchores")]
        public HttpResponseMessage SearchChores(int familyMemberId, string keyword)
        {
            if (familyMemberId.Equals(0) || string.IsNullOrEmpty(keyword))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            var suggestedChores = _choreService.SearchChores(familyMemberId, keyword);
            return Request.CreateResponse(HttpStatusCode.OK, suggestedChores);
        }

        [HttpGet]
        [Route("getchoresbyfamilymember")]
        public IList<Chore> GetChoresByFamilyMemberId(int? memberId = null)
        {
            return _choreService.GetChoresByFamilyMemberId(memberId);
        }

        // api/chore/getchoresbydate?date=12/15/16$familyMemberId=0
        [HttpGet]
        [Route("getchoresbydate")]
        public IList<Chore> GetChoresByDate(DateTime date, int? familyMemberId)
        {
            if (familyMemberId == 0 || date == null)
                throw new InvalidParameterException("Invalid parameters!");
            var familyID = _currentUserService.FamilyID;
           
            return _choreService.GetChoresByDate(date.Date, familyMemberId);
        }

        // api/chore/getbyid/{choreId}
        [HttpGet]
        [Route("getbyid/{choreId}")]
        public Chore GetChoreById(int choreId)
        {
            if (choreId == 0)
                throw new InvalidParameterException("Invalid parameters!");

            return _choreService.GetById(choreId);
        }

        #endregion
    }
}
