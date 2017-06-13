using LeapSpring.MJC.BusinessLogic.Services.Invitation;
using LeapSpring.MJC.Core.Domain.Invitation;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
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
    [RoutePrefix("api/invitation")]
    public class InvitationController : ApiController
    {
        private IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost]
        [Route("add")]
        public FamilyInvitation CreateInvitation([FromBody]FamilyInvitation invitation)
        {
            return _invitationService.AddInvitation(invitation);
        }

        [HttpGet]
        [Route("getinvitationbytoken/{token:Guid}")]
        public FamilyInvitation GetInvitationByToken(Guid token)
        {
            return _invitationService.GetInvitationByToken(token);
        }

        [HttpPut]
        [Route("updateinvitationstatus")]
        public FamilyInvitation UpdateInvitationStatus(int invitationId, int invitationStatus)
        {
            var status = (InvitationStatus)invitationStatus;
            return _invitationService.UpdateInvitationStatus(invitationId, status);
        }
    }
}
