using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Dto.Chores;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Domain.Family;

namespace LeapSpring.MJC.BusinessLogic.Services.ChoreService
{
    public interface IChoreService
    {
        /// <summary>
        /// Adds the specified chore.
        /// </summary>
        /// <param name="chore">Chore.</param>
        /// <param name="familyId">Family identifier</param>
        /// <returns>Chore.</returns>
        Chore Add(Chore chore, int? familyId = null, DayOfWeek? dayOfWeek = null);

        /// <summary>
        /// Gets the chore by its identifier.
        /// </summary>
        /// <param name="choreId"></param>
        /// <returns>The chore.</returns>
        Chore GetById(int choreId);

        /// <summary>
        /// Gets the system chores.
        /// </summary>
        /// <returns>System Chores.</returns>
        List<SystemChore> GetSystemChores();

        /// <summary>
        /// Gets the system chores by age range.
        /// </summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="skipCount">Skip count</param>
        /// <param name="takeCount">Take count</param>
        /// <returns>System Chores.</returns>
        SuggestedChores GetSystemChoresByAgeRange(int memberId, int skipCount, int takeCount = 10);

        /// <summary>
        /// Searches on the chores by keyword given.
        /// </summary>
        /// <param name="familyMemberId">Family member identifier.</param>
        /// <param name="keyWord">The keyword to find.</param>
        /// <returns>Suggessted Chores.</returns>
        List<SuggestedChore> SearchChores(int familyMemberId, string keyWord);

        /// <summary>
        /// Gets the chore count.
        /// </summary>
        /// <param name="choreDueType">The chore type</param>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="choreStatus">Chore status</param>
        /// <returns>The chore count.</returns>
        int GetChoreCount(ChoreDueType choreDueType, int? familyMemberId = null, ChoreStatus choreStatus = ChoreStatus.Active);

        /// <summary>
        /// Gets the chores by family member identifier.
        /// </summary>
        /// <param name="familyMemberId">Family member identifier.</param>
        /// <returns>Chores.</returns>
        List<Chore> GetChoresByFamilyMemberId(int? familyMemberId = null);

        /// <summary>
        /// Get chores by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="familyMemberId">The family member Identifier.</param>
        /// <returns>The list of chores</returns>
        IList<Chore> GetChoresByDate(DateTime date, int? familyMemberId);

        /// <summary>
        /// Updates the chore.
        /// </summary>
        /// <param name="chore">The chore.</param>
        /// <param name="weekDayName">The week day name.</param>
        /// <returns>The updated chore.</returns>
        Chore Update(Chore chore, DayOfWeek? weekDayName = null);

        /// <summary>
        /// Deletes the chore.
        /// </summary>
        /// <param name="choreId">The chore identifier.</param>
        void Delete(int choreId);

        /// <summary>
        /// Disapprove today completed chores
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        void DisapproveTodayChores(int familyMemberId);

        void CancelChore();

        /// <summary>
        /// Get childrens completed chores detail //Todo method name
        /// </summary>
        /// <returns>Result (List<ParentPhone, ChildName, ChoresCount>)</returns>
        IList<Tuple<string, string, int>> GetTodayHeros();

        /// <summary>
        /// Approves the payment for the payday.
        /// </summary>
        /// <param name="familyId">The family identifier</param>
        /// <returns></returns>
        void ApprovePayDayPayment(int familyId);

        /// <summary>
        /// Disapproves the payment for the payday.
        /// </summary>
        /// <param name="familyId">The family identifier</param>
        /// <param name="isJob">The is job.</param>
        /// <returns></returns>
        void DisapprovePayDayPayment(int familyId, bool isJob = false);

        /// <summary>
        /// Get admins with no activity by kids
        /// </summary>
        /// <param name="lastActivityDate">Last activity date</param>
        /// <returns>Family members</returns>
        List<FamilyMember> GetAdminsWithNoActivityByKids(DateTime lastActivityDate);

        /// <summary>
        /// Get continuous child activity
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Family members</returns>
        List<FamilyMember> GetContinuousChildActivity(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the completed chores grouped by family.
        /// </summary>
        /// <param name="startDate">The start date of the week</param>
        /// <param name="endDate">The end date of the week (Friday is the payday)</param>
        /// <returns>The grouped chores list.</returns>
        IEnumerable<IGrouping<int, Chore>> GetCompletedChoresByFamily(DateTime startDate, DateTime endDate);
    }
}
