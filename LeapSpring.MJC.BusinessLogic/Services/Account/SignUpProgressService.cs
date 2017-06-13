using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Dto.Accounts;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    /// <summary>
    /// Represents a sign up progress service
    /// </summary>
    public class SignUpProgressService : ServiceBase, ISignUpProgressService
    {
        private ICurrentUserService _currentUserService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="currentUserService">Current user service</param>
        public SignUpProgressService(IRepository repository, ICurrentUserService currentUserService) : base(repository)
        {
            _currentUserService = currentUserService;
        }

        #region Methods

        /// <summary>
        /// Update signup progress
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        public void UpdateSignUpProgress(int? familyId = null)
        {
            familyId = familyId ?? _currentUserService.FamilyID;

            var family = Repository.Table<Family>().Where(m => m.Id == familyId).SingleOrDefault();
            if (family == null)
                throw new InvalidParameterException("Invalid Family!");

            var signUpProgress = GetDetailedSignUpProgress(familyId);
            if (signUpProgress.IsLinkedToBank || signUpProgress.IsLinkedToCreditCard)
                family.SignUpStatus = SignUpStatus.Completed;
            else if (signUpProgress.IsAssignedSomeChores)
                family.SignUpStatus = SignUpStatus.AddedChore;
            else if (signUpProgress.IsAddedChild)
                family.SignUpStatus = SignUpStatus.AddedChild;
            else
                family.SignUpStatus = SignUpStatus.SingedUp;

            Repository.Update(family);
        }

        /// <summary>
        /// Update signup progress
        /// </summary>
        /// <param name="signUpStatus">Signup status</param>
        /// <param name="familyId">Family identifier</param>
        public void UpdateSignUpProgress(SignUpStatus signUpStatus, int? familyId = null)
        {
            familyId = familyId ?? _currentUserService.FamilyID;
            if (familyId == null)
                return;

            var family = Repository.Table<Family>().Where(m => m.Id == familyId).SingleOrDefault();
            if (family == null)
                throw new InvalidParameterException("Invalid Family!");

            if (family.SignUpStatus != SignUpStatus.Completed)
            {
                switch (family.SignUpStatus)
                {
                    case SignUpStatus.SingedUp:
                        if (signUpStatus == SignUpStatus.AddedChild)
                            family.SignUpStatus = SignUpStatus.AddedChild;
                        break;
                    case SignUpStatus.AddedChild:
                    case SignUpStatus.AddedChore:
                        if (signUpStatus == SignUpStatus.AddedChore)
                            family.SignUpStatus = SignUpStatus.AddedChore;

                        if (signUpStatus == SignUpStatus.Completed)
                            family.SignUpStatus = SignUpStatus.Completed;
                        break;
                }

                Repository.Update(family);
            }
        }

        /// <summary>
        /// Get detailed sign up progress
        /// </summary>
        /// <returns>Sign up progress</returns>
        public SignUpProgress GetDetailedSignUpProgress(int? familyId = null)
        {
            familyId = familyId ?? _currentUserService.FamilyID;
            var adminMember = Repository.Table<FamilyMember>().Include(p => p.User).SingleOrDefault(m => m.User.FamilyID == familyId && m.MemberType == MemberType.Admin);

            var childMember = Repository.Table<FamilyMember>().OrderByDescending(m => m.Id).FirstOrDefault(m => m.User.FamilyID == familyId && m.MemberType == MemberType.Child && !m.IsDeleted);
            var hasChores = Repository.Table<Chore>().Any(m => m.FamilyMember.User.FamilyID == familyId && !m.FamilyMember.IsDeleted);
            var financialAccount = Repository.Table<FinancialAccount>().SingleOrDefault(m => m.FamilyMemberID == adminMember.Id);
            //added for credit card
            var CCAccount = Repository.Table<CreditCardAccount>().SingleOrDefault(m => m.FamilyMemberID == adminMember.Id);
            return new SignUpProgress
            {
                MemberId = adminMember.Id,
                IsAccountCreated = true,
                IsAddedChild = childMember != null,
                IsAssignedSomeChores = hasChores,
                IsLinkedToBank = (financialAccount != null && financialAccount.ExternalAccountID.HasValue),
                BankStatus = financialAccount?.Status.ToString(),
                LastChildId = childMember?.Id,
                HasPin = string.IsNullOrEmpty(adminMember.User.PIN) ? false : true,
                HasPhoneNumber = string.IsNullOrEmpty(adminMember.PhoneNumber) ? false : true,
                IsLinkedToCreditCard = (CCAccount != null && CCAccount.customer_vault_id != null),
            };
        }

        /// <summary>
        /// Get signup progress
        /// </summary>
        /// <returns>Signup status</returns>
        public SignUpStatus GetSignUpProgress()
        {
            var family = Repository.Table<Family>().Where(m => m.Id == _currentUserService.FamilyID).SingleOrDefault();
            if (family == null)
                throw new InvalidParameterException("Invalid Family!");

            return family.SignUpStatus;
        }


        #endregion
    }
}
