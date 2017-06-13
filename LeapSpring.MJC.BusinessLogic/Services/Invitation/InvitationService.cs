using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Invitation;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LeapSpring.MJC.BusinessLogic.Services.Invitation
{
    public class InvitationService : ServiceBase, IInvitationService
    {
        private ICurrentUserService _currentUserService;
        private ITextMessageService _textMessageService;
        private IAppSettingsService _appSettingsService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="currentUserService">Current user service</param>
        /// <param name="textMessageService">Text message service</param>
        public InvitationService(IRepository repository, ICurrentUserService currentUserService, ITextMessageService textMessageService, IAppSettingsService appSettingsService) : base(repository)
        {
            _currentUserService = currentUserService;
            _textMessageService = textMessageService;
            _appSettingsService = appSettingsService;
        }

        /// <summary>
        /// Insert the family invitation
        /// </summary>
        /// <param name="familyInvitation">Family invitation</param>
        /// <returns>The family invitation</returns>
        public FamilyInvitation AddInvitation(FamilyInvitation familyInvitation)
        {
            if (familyInvitation == null)
                throw new InvalidParameterException("Invalid parameters!");

            familyInvitation.PhoneNumber = familyInvitation.PhoneNumber.RemoveCountyCode();
            var member = Repository.Table<FamilyMember>().Where(p => p.PhoneNumber == familyInvitation.PhoneNumber);
            if (member.Any())
                throw new InvalidParameterException("Entered phone number is already used by another user");

            var invitation = new FamilyInvitation
            {
                PhoneNumber = familyInvitation.PhoneNumber,
                Token = Guid.NewGuid(),
                MemberType = familyInvitation.MemberType,
                CreationDate = DateTime.UtcNow,
                Status = InvitationStatus.WaitingForSignup,
                FamilyID = _currentUserService.FamilyID
            };

            Repository.Insert(invitation);

            // Sms invitation
            var family = Repository.Table<Family>().Where(p => p.Id.Equals(invitation.FamilyID)).SingleOrDefault();
            var url = HttpContext.Current != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "#/invite/" + invitation.Token : string.Empty;
            var message = string.Format(_appSettingsService.FamilyInvitationMessage, family != null ? family.Name : string.Empty, url);
            _textMessageService.Send(invitation.PhoneNumber, message);

            return invitation;
        }

        /// <summary>
        /// Gets the family invitation.
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>The family invitation.</returns>
        public FamilyInvitation GetInvitationByToken(Guid token)
        {
            if (token == Guid.Empty)
                throw new ArgumentNullException("Please provide a valid token");

            var familyInvitation = Repository.Table<FamilyInvitation>().Include(m => m.Family).SingleOrDefault(m => m.Token.Equals(token) && m.Status == InvitationStatus.WaitingForSignup);
            if (familyInvitation == null)
                throw new InvalidParameterException("Your invitation is already accepted.");

            return familyInvitation;
        }

        /// <summary>
        /// Update the invitation status
        /// </summary>
        /// <param name="invitationId">Invitation identifier</param>
        /// <param name="invitationStatus">Invitation status</param>
        /// <returns>The family invitation</returns>
        public FamilyInvitation UpdateInvitationStatus(int invitationId, InvitationStatus invitationStatus)
        {
            var invitation = Repository.Table<FamilyInvitation>().SingleOrDefault(m => m.Id.Equals(invitationId));
            if (invitation == null)
                throw new InvalidParameterException("Invitation not found!");

            invitation.Status = InvitationStatus.AcceptedInvitation;
            Repository.Update(invitation);
            return invitation;
        }
    }
}
