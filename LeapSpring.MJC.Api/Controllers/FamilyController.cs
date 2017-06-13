using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using CorePro.SDK.Models;
using CorePro.SDK;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.Data.Repository;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/family")]
    public class FamilyController : ApiController
    {
        private IFamilyService _familyService;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;
        private ICoreProService _coreproService;

        /// <summary>
        /// Ctor
        /// </summaryfamilyService
        /// <param name="allocationSettingsService">Family service</param>
        /// <param name="accountService">Account service</param>
        /// <param name="currentUserService">Current user service</param>
        public FamilyController(IFamilyService familyService, IAccountService accountService, ICurrentUserService currentUserService, ICoreProService CoreProService)
        {
            _familyService = familyService;
            _accountService = accountService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route("add")]
        public FamilyMember CreateMember([FromBody]FamilyMember member)
        {
            if (member == null)
                throw new InvalidParameterException("Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            return _familyService.AddMember(member);
        }

        [HttpPut]
        [Route("update")]
        public FamilyMember Update([FromBody]FamilyMember member)
        {
            if (member == null)
                throw new InvalidParameterException("Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            return _familyService.UpdateMember(member);
        }
        // DELETE: api/family/deletemember
        [HttpDelete]
        [Route("delete/{memberId}")]
        public HttpResponseMessage DeleteMember(int memberId)
        {
            if (_currentUserService.MemberType != MemberType.Admin || !_accountService.BelongsToThisFamily(memberId))
                throw new UnauthorizedAccessException();

            _familyService.DeleteMember(memberId);
            return Request.CreateResponse(HttpStatusCode.OK, "Member successfully deleted.");
        }

        // PUT: api/family/updatememberinfo
        [HttpPut]
        [Route("updatememberinfo")]
        public HttpResponseMessage UpdateMemberInfo(DateTime dateOfBirth, string address, string city, int stateId, string ssn)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(city) || stateId <= 0 || string.IsNullOrEmpty(ssn))
                throw new InvalidParameterException("Invalid parameters!");

            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            _familyService.UpdateMemberInfo(dateOfBirth, address, city, stateId, ssn);
            return Request.CreateResponse(HttpStatusCode.OK, "Member information updated successfully");
        }

        [HttpGet]
        [Route("getfamilybyid/{familyId}")]
        public Family GetFamily(int familyId)
        {
            return _familyService.GetFamilyById(familyId);
        }

        [HttpGet]
        [Route("getmember")]
        public FamilyMember GetMember()
        {
            return _familyService.GetMember();
        }

        [HttpGet]
        [Route("getchildmember")]
        public FamilyMember GetChildMember(int memberId)
        {
            return _familyService.GetChildMember(memberId);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getbyid/{familyMemberId}")]
        public FamilyMember GetById(int familyMemberId)
        {
            if (familyMemberId.Equals(0))
                throw new InvalidParameterException("Invalid parameters!");

            if (!_accountService.BelongsToThisFamily(familyMemberId))
                throw new UnauthorizedAccessException();

            return _familyService.GetMemberById(familyMemberId);
        }

        // GET: api/family/getchildrens
        [HttpGet]
        [Route("getchildrens")]
        public IList<FamilyMember> GetChildrens()
        {
            if (_currentUserService.MemberType != MemberType.Admin && _currentUserService.MemberType != MemberType.Parent)
                throw new UnauthorizedAccessException();

            return _familyService.GetChildrens();
        }

        [HttpPut]
        [Route("uploadimage")]
        public async Task<HttpResponseMessage> UploadImage([FromBody]ProfileImage profileImage)
        {
            //string path1 = "C:\\inetpub\\wwwroot\\CorePro\\DataWrite9.txt";
            //using (StreamWriter writer = new StreamWriter(path1, true))
            //{
            //    writer.WriteLine(profileImage.FamilyMemberId + "***" + profileImage.Base64ImageUrl);
            //    writer.Close();
            //}
            if (profileImage == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid parameters!");

            var imageUrl = await _familyService.UploadProfileImage(profileImage);
            return Request.CreateResponse(HttpStatusCode.OK, imageUrl);
        }

        [HttpPut]
        [Route("updateadminpin")]
        public HttpResponseMessage UpdateAdminPin(string pin)
        {
            if (string.IsNullOrEmpty(pin))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid pin!");

            var familyMember = _familyService.UpdateAdminPin(pin);
            return Request.CreateResponse(HttpStatusCode.OK, familyMember);
        }

        [HttpPut]
        [Route("updatememberpin")]
        public HttpResponseMessage UpdateMemberPin(string pin, int memberId)
        {
            if (string.IsNullOrEmpty(pin))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid pin!");

            if (!_accountService.BelongsToThisFamily(memberId))
                throw new UnauthorizedAccessException();

            var familyMember = _familyService.UpdateMemberPin(pin, memberId);
            return Request.CreateResponse(HttpStatusCode.OK, familyMember);
        }

        [HttpPut]
        [Route("updateadminphone")]
        public FamilyMember UpdateAdminPhonenumber(string phonenumber)
        {
            if (string.IsNullOrEmpty(phonenumber))
                throw new InvalidParameterException("Invalid phonenumber!");

            return _familyService.UpdateAdminPhonenumber(phonenumber);
        }


        [HttpPut]
        [Route("updatememberphone")]
        public FamilyMember UpdateMemberPhonenumber(string phonenumber, int memberId)
        {
            if (string.IsNullOrEmpty(phonenumber))
                throw new InvalidParameterException("Invalid phonenumber!");

            if (!_accountService.BelongsToThisFamily(memberId))
                throw new UnauthorizedAccessException();

            return _familyService.UpdateMemberPhonenumber(phonenumber, memberId);
        }

        // GET: api/family/getallmembers
        [HttpGet]
        [Route("getallmembers")]
        public Dictionary<MemberType, List<FamilyMember>> GetAllMembers()
        {
            return _familyService.GetAllMembers();
        }

        // GET: api/family/getchildcount
        [HttpGet]
        [Route("getchildcount")]
        public HttpResponseMessage GetChildCount()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _familyService.GetChildCount());
        }

        [HttpGet]
        [Route("getsignupprogress")]
        public SignUpStatus GetSignUpProgress()
        {
            return _familyService.GetSignUpProgress();
        }

        // GET: api/family/getstates
        [HttpGet]
        [Route("getstates")]
        public HttpResponseMessage GetStates()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _familyService.GetStates());
        }

        // [AllowAnonymous]
        [HttpGet]
        [Route("getfamilybyname")]
        public List<FamilyMember> GetFamilyMembersByName(string familyName)
        {
            return _familyService.GetFamilyMembersByName(familyName);
        }

        [HttpPut]
        [Route("toggleemailsubscription")]
        public HttpResponseMessage ToggleEmailSubscription()
        {
            _familyService.ToggleEmailSubscription();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("hastrial")]
        public HttpResponseMessage HasTrial()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _familyService.HasTrial());
        }

        [HttpPut]
        [Route("marktrialasused")]
        public HttpResponseMessage MarkTrialAsUsed()
        {
            _familyService.MarkTrialAsUsed();
            return Request.CreateResponse(HttpStatusCode.OK);
        }





    }
}
