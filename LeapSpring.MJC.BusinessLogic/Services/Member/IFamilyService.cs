using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Member
{
    public interface IFamilyService
    {
        /// <summary>
        /// Insert new family member
        /// </summary>
        /// <param name="familyMember"></param>
        /// <returns>The family member.</returns>
        FamilyMember AddMember(FamilyMember familyMember);

        /// <summary>
        /// Update the family member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>The family member.</returns>
        FamilyMember UpdateMember(FamilyMember member);

        /// <summary>
        /// Delete family member by identifier
        /// </summary>
        /// <param name="memberId">Member identifier</param>
        void DeleteMember(int memberId);

        /// <summary>
        /// Update member information
        /// </summary>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="address">Address</param>
        /// <param name="city">City</param>
        /// <param name="stateId">State identifier</param>
        /// <param name="ssn">Social security number</param>
        /// <returns>Family member</returns>
        FamilyMember UpdateMemberInfo(DateTime dateOfBirth, string address, string city, int stateId, string ssn);

        /// <summary>
        /// Gets the family by identifier
        /// </summary>
        /// <returns>The family.</returns>
        Family GetFamilyById(int familyId);

        /// <summary>
        /// Gets the family member.
        /// </summary>
        /// <returns>The family member.</returns>
        FamilyMember GetMember();
        FamilyMember GetChildMember(int memberId);

        /// <summary>
        /// Get admin member
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns>The family member</returns>
        FamilyMember GetAdmin(int? familyId = null);

        /// <summary>
        /// Gets the family member by id.
        /// </summary>
        /// <param name="familyMemberId"></param>
        /// <returns>The family member.</returns>
        FamilyMember GetMemberById(int familyMemberId);

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <param name="memberType">The member type.</param>
        /// <returns>The list of family members.</returns>
        List<FamilyMember> GetMembers(MemberType memberType);

        /// <summary>
        /// Gets all childrens of the family.
        /// </summary>
        /// <returns>The list of family members.</returns>
        IList<FamilyMember> GetChildrens();

        /// <summary>
        /// Upload profile Image
        /// </summary>
        /// <param name="profileImage">The profile image data.</param>
        /// <returns>The image url</returns>
        Task<string> UploadProfileImage(ProfileImage profileImage);

        /// <summary>
        /// Update the admin pin number.
        /// </summary>
        /// <param name="pin">Pin number</param>
        /// <returns>The family member.</returns>
        FamilyMember UpdateAdminPin(string pin);
       
        
        /// <summary>
        /// Update the member pin number.
        /// </summary>
        /// <param name="pin">Pin number</param>
        /// <param name="memberId">Member identifier</param>
        /// <returns>The family member.</returns>
        FamilyMember UpdateMemberPin(string pin, int memberId);

        /// <summary>
        /// Updates the admin phonenumber.
        /// </summary>
        /// <param name="phonenumber">The phonenumber.</param>
        /// <returns>FamilyMember.</returns>
        FamilyMember UpdateAdminPhonenumber(string phonenumber);
        
        /// <summary>
        /// Updates the child phonenumber.
        /// </summary>
        /// <param name="phonenumber">The phonenumber.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <returns>FamilyMember.</returns>
        FamilyMember UpdateMemberPhonenumber(string phonenumber, int memberId);

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <returns>The list of family members.</returns>
        Dictionary<MemberType, List<FamilyMember>> GetAllMembers();

        /// <summary>
        /// Get family member by phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Family member</returns>
        FamilyMember GetMemberByPhone(string phoneNumber);

        /// <summary>
        /// Get member by family id and firstname
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        /// <param name="firstname">Firstname</param>
        /// <param name="memberType">Member type</param>
        /// <returns>Family member</returns>
        FamilyMember GetMember(int familyId, string firstname, MemberType memberType);

     
        /// <summary>
        /// Get child count
        /// </summary>
        /// <returns>Child coun</returns>
        int GetChildCount();

        /// <summary>
        /// Get signup progress
        /// </summary>
        /// <returns>Signup status</returns>
        SignUpStatus GetSignUpProgress();

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <param name="familyName">The family name.</param>
        /// <returns>Family members.</returns>
        List<FamilyMember> GetFamilyMembersByName(string familyName);

        /// <summary>
        /// Get states
        /// </summary>
        /// <returns>States</returns>
        IList<State> GetStates();

        /// <summary>
        /// Enables the payday auto approval
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        void MarkAsPayDayAutoApproval(FamilyMember adminMember);

        /// <summary>
        /// Gets the family subscription
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns></returns>
        FamilySubscription GetFamilySubscription(int? familyId = null);

        /// <summary>
        /// Get the promocodce details from URL flow
        /// </summary>
        /// <param name="familyId"></param>
        /// <returns></returns>
        FamilyMember GetPrePromoCodeStatus(int? familyId = null);
        /// <summary>
        /// Updates the family subscription
        /// </summary>
        /// <returns></returns>
        FamilySubscription UpdatetFamilySubscription(FamilySubscription familySubscription);

        /// <summary>
        /// Get incomplete admins
        /// </summary>
        /// <returns>Family members</returns>
        List<FamilyMember> GetIncompleteAdmins();

        /// <summary>
        /// Get members by last loggedin
        /// </summary>
        /// <param name="memberType">Member type</param>
        /// <param name="loggedOnDate">Logged on date</param>
        /// <returns>Family members</returns>
        List<FamilyMember> GetMembersByLastLoggedIn(MemberType memberType, DateTime loggedOnDate);

        /// <summary>
        /// Get childrens by last loggedin
        /// </summary>
        /// <param name="loggedOnDate">Logged on date</param>
        /// <returns>Family members</returns>
        List<FamilyMember> GetChildrensByLastLoggedIn(DateTime loggedOnDate);

        /// <summary>
        /// Gets the upcoming subscriptions.
        /// </summary>
        /// <returns>The list of family member</returns>
        List<FamilyMember> GetUpcomingSubscriptions();

        /// <summary>
        /// Gets the family by member identifier
        /// </summary>
        /// <returns>The family.</returns>
        Family GetFamilyByMemberId(int familyMemberId);

        /// <summary>
        /// Update the Child member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>The family member.</returns>
       

     

        /// <summary>
        /// Toggles the email notification subscription
        /// </summary>
        void ToggleEmailSubscription();

        /// <summary>
        /// Checks that the use has trial.
        /// </summary>
        /// <returns><c>True</c>, If has trial. <c>False</c>, Otherwise.</returns>
        bool HasTrial();

        /// <summary>
        /// Marks the has trial as used.
        /// </summary>
        void MarkTrialAsUsed();

      

        

        

    }
}
