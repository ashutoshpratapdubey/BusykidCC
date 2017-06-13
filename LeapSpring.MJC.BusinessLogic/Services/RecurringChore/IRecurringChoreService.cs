using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.BusinessLogic.Services.RecurringChore
{
    public interface IRecurringChoreService
    {
        /// <summary>
        /// Creates the recurring chores.
        /// </summary>
        /// <param name="frequencyType">The frquency type.</param>
        /// <param name="recurringChoreId">The recurring chore identifier.</param>
        /// <param name="familyId">The family identifier.</param>
        /// <param name="isJob">Is Job.</param>
        /// <param name="weekDayName">The week day name.</param>
        void CreateChores(FrequencyType frequencyType, int? recurringChoreId = null, int? familyId = null, bool isJob = false, DayOfWeek? weekDayName = null);

        /// <summary>
        /// Updates the recurring chopres
        /// </summary>
        /// <param name="recurringChoreId">The recurring chore identifier.</param>S
        /// <param name="frequencyType">The frquency type.</param>
        /// <param name="familyId">The family identifier.</param>
        /// <param name="weekDayName">The week day name.</param>
        void UpdateRecurringChore(int recurringChoreId, FrequencyType frequencyType, int? familyId = null, DayOfWeek? weekDayName = null);
    }
}
