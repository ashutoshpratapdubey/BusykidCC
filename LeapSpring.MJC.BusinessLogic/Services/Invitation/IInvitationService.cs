using LeapSpring.MJC.Core.Domain.Invitation;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Invitation
{
    public interface IInvitationService
    {
        /// <summary>
        /// Insert the family invitation
        /// </summary>
        /// <param name="familyInvitation">Family invitation</param>
        /// <returns>The family invitation</returns>
        FamilyInvitation AddInvitation(FamilyInvitation familyInvitation);

        /// <summary>
        /// Gets the family invitation.
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>The family invitation.</returns>
        FamilyInvitation GetInvitationByToken(Guid token);

        /// <summary>
        /// Update the invitation status
        /// </summary>
        /// <param name="invitationId">Invitation identifier</param>
        /// <param name="invitationStatus">Invitation status</param>
        /// <returns>The family invitation</returns>
        FamilyInvitation UpdateInvitationStatus(int invitationId, InvitationStatus invitationStatus);
    }
}
