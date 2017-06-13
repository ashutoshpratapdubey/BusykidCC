using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Settings;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Settings
{
    /// <summary>
    /// Represents a allocation settings service
    /// </summary>
    public class AllocationSettingsService : ServiceBase, IAllocationSettingsService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public AllocationSettingsService(IRepository repository) : base(repository)
        {
        }

        #region Utilities

        /// <summary>
        /// Get default allocation settings
        /// </summary>
        /// <returns>Allocation settings</returns>
        private AllocationSettings GetDefaultAllocation(int familyMemberId)
        {
            var childDob = Repository.Table<FamilyMember>().Where(m => m.Id == familyMemberId && !m.IsDeleted).Select(m => m.DateOfBirth).SingleOrDefault();
            AllocationByAge defaultAllocation = null;

            // Get default allocation by child age
            if (childDob.HasValue)
            {
                // Calculate child member age range
                int age = DateTime.Today.Year - childDob.Value.Year;
                defaultAllocation = Repository.Table<AllocationByAge>().FirstOrDefault(m => m.Age == age);
            }

            if (defaultAllocation == null)
                defaultAllocation = Repository.Table<AllocationByAge>().FirstOrDefault();

            return new AllocationSettings
            {
                FamilyMemberID = familyMemberId,
                Save = defaultAllocation.Save,
                Share = defaultAllocation.Share,
                Spend = defaultAllocation.Spend
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get allocation settings by family member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Allocation settings</returns>
        public AllocationSettings GetByMemberId(int familyMemberId)
        {
            var allocationSettings = Repository.Table<AllocationSettings>().FirstOrDefault(m => m.FamilyMemberID == familyMemberId);
            if (allocationSettings == null) return GetDefaultAllocation(familyMemberId);

            return allocationSettings;
        }

        /// <summary>
        /// Create new allocation settings by member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Allocation settings</returns>
        public AllocationSettings CreateNew(int familyMemberId)
        {
            var allocationSettings = GetDefaultAllocation(familyMemberId);
            Repository.Insert(allocationSettings);

            return allocationSettings;
        }

        /// <summary>
        /// Get default allocation by age
        /// </summary>
        /// <param name="age">Child age</param>
        /// <returns>AllocationByAge</returns>
        public AllocationByAge GetAllocationByAge(int age)
        {
            var allocation = Repository.Table<AllocationByAge>().SingleOrDefault(m => m.Age == age);

            // Get first default allocation, If allocation empty for this age
            return allocation ?? Repository.Table<AllocationByAge>().FirstOrDefault();
        }

        /// <summary>
        /// Update allocation settings
        /// </summary>
        /// <param name="updatedSettings">Allocation settings</param>
        public void Update(AllocationSettings updatedSettings)
        {
            var totalAllocate = updatedSettings.Save + updatedSettings.Share + updatedSettings.Spend;
            if (totalAllocate != 100)
                throw new InvalidParameterException("Total has to equal 100%, please adjust");

            var allocationSettings = Repository.Table<AllocationSettings>().SingleOrDefault(m => m.Id == updatedSettings.Id);
            if (allocationSettings == null) throw new ObjectNotFoundException("Allocation settings not found");

            // Update
            allocationSettings.Save = updatedSettings.Save;
            allocationSettings.Share = updatedSettings.Share;
            allocationSettings.Spend = updatedSettings.Spend;
            Repository.Update(allocationSettings);
        }

        #endregion

    }
}
