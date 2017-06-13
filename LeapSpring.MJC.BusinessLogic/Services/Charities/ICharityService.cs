using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Domain.Family;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Charities
{
    /// <summary>
    /// Represents a charity service.
    /// </summary>
    public interface ICharityService
    {
        /// <summary>
        /// Make a donation approval request to the parent
        /// </summary>
        /// <param name="donation">The donation</param>
        /// <returns>The donation</returns>
        Donation Donate(Donation donation);

        /// <summary>
        /// Gets the list of charities.
        /// </summary>
        /// <returns>The charities list.</returns>
        IList<Charity> GetCharities();

        /// <summary>
        /// Approves the donation.
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <param name="donationId">The donation identifier.</param>
        /// <returns>The approved donation.</returns>
        Donation ApproveDonation(FamilyMember adminMember, int donationId);

        /// <summary>
        /// Disapproves the donation
        /// </summary>
        /// <param name="donationId">The donation identifier</param>
        /// <returns></returns>
        void DisapproveDonation(int donationId);
    }
}
