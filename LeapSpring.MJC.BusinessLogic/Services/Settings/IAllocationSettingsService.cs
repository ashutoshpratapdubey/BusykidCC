using LeapSpring.MJC.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Settings
{
    /// <summary>
    /// Represents a interface of allocation settings service
    /// </summary>
    public interface IAllocationSettingsService
    {
        /// <summary>
        /// Get allocation settings by family member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Allocation settings</returns>
        AllocationSettings GetByMemberId(int familyMemberId);
        
        /// <summary>
        /// Create new allocation settings by member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Allocation settings</returns>
        AllocationSettings CreateNew(int familyMemberId);

        /// <summary>
        /// Get default allocation by age
        /// </summary>
        /// <param name="age">Child age</param>
        /// <returns>AllocationByAge</returns>
        AllocationByAge GetAllocationByAge(int age);

        /// <summary>
        /// Update allocation settings
        /// </summary>
        /// <param name="updatedSettings">Allocation settings</param>
        void Update(AllocationSettings updatedSettings);
    }
}
