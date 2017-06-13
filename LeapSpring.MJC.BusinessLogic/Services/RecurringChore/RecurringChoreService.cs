using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Data.Repository;
using System.Linq;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Data.Entity;
using LeapSpring.MJC.BusinessLogic.Services.Member;

namespace LeapSpring.MJC.BusinessLogic.Services.RecurringChore
{
    public class RecurringChoreService : ServiceBase, IRecurringChoreService
    {
        private readonly IChoreService _choreService;
        private readonly IFamilyService _familyService;

        public RecurringChoreService(IRepository repository, IChoreService choreService, IFamilyService familyService) : base(repository)
        {
            _choreService = choreService;
            _familyService = familyService;
        }

        /// <summary>
        /// Creates the recurring chores.
        /// </summary>
        /// <param name="frequencyType">The frquency type.</param>
        /// <param name="recurringChoreId">The recurring chore identifier.</param>
        /// <param name="familyId">The family identifier.</param>
        /// <param name="isJob">Is Job.</param>
        /// <param name="weekDayName">The weekday name.</param>
        public void CreateChores(FrequencyType frequencyType, int? recurringChoreId = null, int? familyId = null, bool isJob = false, DayOfWeek? weekDayName = null)
        {
            var recurringChores = (!recurringChoreId.HasValue) ? GetRecurringChores(frequencyType) : Repository.Table<Chore>().Where(p => p.Id.Equals(recurringChoreId.Value));
            foreach (var recurringChore in recurringChores.ToList())
            {
                if (string.IsNullOrEmpty(recurringChore.FrequencyRange))
                    continue;

                if (isJob)
                {
                    // Get Family ID
                    var family = _familyService.GetFamilyByMemberId(recurringChore.FamilyMemberID);
                    if (family?.FamilySubscription?.Status == SubscriptionStatus.Active)
                        familyId = family.Id;
                    else
                        continue;
                }

                var weekDays = recurringChore.FrequencyRange.Split(',');
                var choreAddedDay = weekDayName;

                foreach (var weekDay in weekDays)
                {
                    var dayOfWeek = (DayOfWeek)(Enum.Parse(typeof(DayOfWeek), weekDay));
                    var dueDate = DateTime.UtcNow.AddDays(-1 * (DateTime.UtcNow.DayOfWeek - dayOfWeek)).ToUniversalTime();

                    var isPastDay = (weekDayName.HasValue) ? ((dayOfWeek - weekDayName.Value) < 0) : ((dueDate - DateTime.UtcNow).Days) < 0;

                    if (!isPastDay)
                    {
                        var chore = new Chore();
                        chore.Name = recurringChore.Name;
                        chore.Value = recurringChore.Value;
                        chore.ImageUrl = recurringChore.ImageUrl;
                        chore.DueDate = weekDayName.HasValue ? DateTime.MinValue : dueDate;
                        chore.SystemChoreID = recurringChore.SystemChoreID;
                        chore.FrequencyType = recurringChore.FrequencyType;
                        chore.FrequencyRange = string.Empty;
                        chore.CreatedTime = weekDayName.HasValue ? DateTime.MinValue : DateTime.UtcNow;
                        chore.ChoreStatus = ChoreStatus.Active;
                        chore.RecurringChoreID = recurringChore.Id;
                        chore.FamilyMemberID = recurringChore.FamilyMemberID;

                        var dayName = weekDayName.HasValue ? (DayOfWeek?)dayOfWeek : null;
                        _choreService.Add(chore, familyId, dayName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the recurring chores based on given recurrence type.
        /// </summary>
        /// <param name="recurrenceType">The recurrence type.</param>
        /// <returns>The recurring chores.</returns>
        public IQueryable<Chore> GetRecurringChores(FrequencyType recurrenceType)
        {
            return Repository.Table<Chore>().Where(p => p.FrequencyType == recurrenceType && p.ChoreStatus == ChoreStatus.Active && !p.RecurringChoreID.HasValue
                            && !p.IsDeleted && !p.FamilyMember.IsDeleted);
        }

        /// <summary>
        /// Updates the recurring chopres
        /// </summary>
        /// <param name="recurringChoreId">The recurring chore identifier.</param>S
        /// <param name="frequencyType">The frquency type.</param>
        /// <param name="familyId">The family identifier.</param>
        /// <param name="weekDayName">The week day name.</param>
        public void UpdateRecurringChore(int recurringChoreId, FrequencyType frequencyType, int? familyId = null, DayOfWeek? weekDayName = null)
        {
            var recurringChores = Repository.Table<Chore>().Where(p => p.RecurringChoreID == recurringChoreId && p.ChoreStatus == ChoreStatus.Active).ToList();
            foreach (var recurringChore in recurringChores)
            {
                Repository.Delete(recurringChore);
            }
            CreateChores(frequencyType, recurringChoreId, familyId, false, weekDayName);
        }
    }
}
