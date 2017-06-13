using LeapSpring.MJC.Core.Domain.Chore;
using System;
using System.Collections.Generic;
using System.Linq;
using LeapSpring.MJC.Core.Dto.Chores;
using LeapSpring.MJC.Data.Repository;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Core.Enums;
using System.Data.Entity;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;

namespace LeapSpring.MJC.BusinessLogic.Services.ChoreService
{
    public class ChoreService : ServiceBase, IChoreService
    {
        private ISignUpProgressService _signUpProgressService;
        private ICurrentUserService _currentUserService;
        private ITransactionService _transactionService;
        private IEarningsService _earningServices;
        public ChoreService(IRepository repository, ISignUpProgressService signUpProgressService, ICurrentUserService currentUserService, ITransactionService transactionService, IEarningsService earningServices) : base(repository)
        {
            _signUpProgressService = signUpProgressService;
            _currentUserService = currentUserService;
            _transactionService = transactionService;
            _earningServices = earningServices;
        }

        /// <summary>
        /// Adds the specified chore.
        /// </summary>
        /// <param name="chore">Chore.</param>
        /// <param name="familyId">Family identifier</param>
        /// <returns>Chore.</returns>
        public Chore Add(Chore chore, int? familyId = null, DayOfWeek? dayOfWeek = null)
        {
            var todayDate = DateTime.UtcNow;
            if (dayOfWeek.HasValue)
                todayDate = DateTime.UtcNow.AddDays(-1 * (DateTime.UtcNow.DayOfWeek - dayOfWeek.Value)).ToUniversalTime();

            if ((chore.DueDate == null || chore.DueDate.Equals(DateTime.MinValue)) && (chore.FrequencyType == FrequencyType.Once || chore.RecurringChoreID != null))
                chore.DueDate = todayDate;

            if ((chore.CreatedTime == null || chore.CreatedTime.Equals(DateTime.MinValue)))
                chore.CreatedTime = todayDate;

            this.Repository.Insert(chore);

            if (chore.IsSystemChoreUpdated)
                UpdateSystemChore(chore.FamilyMemberID, (int)chore.SystemChoreID, chore.Name, chore.Value, chore.FrequencyType);

            // Update signup progress
            _signUpProgressService.UpdateSignUpProgress(SignUpStatus.AddedChore, familyId);
            return chore;
        }

        /// <summary>
        /// Gets the chore by its identifier.
        /// </summary>
        /// <param name="choreId"></param>
        /// <returns>The chore.</returns>
        public Chore GetById(int choreId)
        {
            return Repository.Table<Chore>().Include(p => p.FamilyMember).SingleOrDefault(p => p.Id.Equals(choreId));
        }

        /// <summary>
        /// Gets the system chores.
        /// </summary>
        /// <returns>System Chores.</returns>
        public List<SystemChore> GetSystemChores()
        {
            return Repository.Table<SystemChore>().ToList();
        }

        /// <summary>
        /// Gets the system chores by age range.
        /// </summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="skipCount">Skip count</param>
        /// <param name="takeCount">Take count</param>
        /// <returns>System Chores.</returns>
        public SuggestedChores GetSystemChoresByAgeRange(int memberId, int skipCount, int takeCount = 10)
        {
            var suggestedChores = new SuggestedChores();
            // Get family member by member identifier
            var member = Repository.Table<FamilyMember>().Where(m => m.Id.Equals(memberId) && !m.IsDeleted).SingleOrDefault();
            if (member == null)
                return suggestedChores;

            // Calculate member age range
            int age = DateTime.Today.Year - member.DateOfBirth.Value.Year;

            // Get system chore by child age
            var systemChores = Repository.Table<SystemChore>().Where(p => (age >= p.StartAge && age <= p.EndAge) && (p.ChildId == null || p.ChildId == memberId));
            // Get customized system chores from system chores
            var customSystemChores = systemChores.Where(p => p.ParentSystemChoreId != null);

            // Update system chore values from customized chore values
            foreach (var customSystemChore in customSystemChores)
            {
                var systemChore = systemChores.Where(p => p.Id == customSystemChore.ParentSystemChoreId).SingleOrDefault();
                if (systemChore != null)
                {
                    systemChore.Name = customSystemChore.Name;
                    systemChore.Value = customSystemChore.Value;
                    systemChore.FrequencyType = customSystemChore.FrequencyType;
                }
            }

            // Get open system chores of child
            var activeSystemChores = Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId && p.ChoreStatus == ChoreStatus.Active && !p.IsDeleted).Select(n => n.SystemChoreID).Distinct();
            // Get total count of system chores by open system chores of child
            suggestedChores.TotalCount = systemChores.Count(p => p.ParentSystemChoreId == null && !activeSystemChores.Contains(p.Id));
            // Eliminate customized chores and Get system chores by open system chores of child
            suggestedChores.SystemChores = !takeCount.Equals(0)
                ? systemChores.Where(p => p.ParentSystemChoreId == null && !activeSystemChores.Contains(p.Id)).OrderBy(p => p.Id).Skip(skipCount).Take(takeCount).ToList()
                : systemChores.Where(p => p.ParentSystemChoreId == null && !activeSystemChores.Contains(p.Id)).OrderBy(p => p.Id).ToList();
            return suggestedChores;
        }

        /// <summary>
        /// Searches on the chores by keyword given.
        /// </summary>
        /// <param name="familyMemberId">Family member identifier.</param>
        /// <param name="keyWord">The keyword to find.</param>
        /// <returns>Suggessted Chores.</returns>
        public List<SuggestedChore> SearchChores(int familyMemberId, string keyWord)
        {
            var suggestedChoresByAge = GetSystemChoresByAgeRange(familyMemberId, 0, 0);

            suggestedChoresByAge.SystemChores = suggestedChoresByAge.SystemChores.Where(p => p.Name.ToLower().StartsWith(keyWord.ToLower())).ToList();

            var chores = GetChoresByFamilyMemberId(familyMemberId).Where(p => p.SystemChoreID == null && p.Name.ToLower().StartsWith(keyWord.ToLower())).ToList();

            var suggesstedChores = new List<SuggestedChore>();
            // Assign system chores to suggessted chores
            foreach (var systemChore in suggestedChoresByAge.SystemChores)
            {
                suggesstedChores.Add(new SuggestedChore
                {
                    SystemChoreId = systemChore.Id,
                    NameofChore = systemChore.Name,
                    Value = systemChore.Value,
                    ImageUrl = systemChore.ImageUrl,
                    FrequencyType = systemChore.FrequencyType
                });
            }

            // Assign own chores to suggessted chores
            foreach (var chore in chores)
            {
                suggesstedChores.Add(new SuggestedChore
                {
                    SystemChoreId = chore.SystemChoreID,
                    NameofChore = chore.Name,
                    Value = chore.Value,
                    ImageUrl = chore.ImageUrl,
                    FrequencyType = chore.FrequencyType
                });
            }

            return suggesstedChores;
        }

        /// <summary>
        /// Gets the chore count.
        /// </summary>
        /// <param name="choreDueType">The chore type</param>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="choreStatus">Chore status</param>
        /// <returns>The chore count.</returns>
        public int GetChoreCount(ChoreDueType choreDueType, int? familyMemberId = null, ChoreStatus choreStatus = ChoreStatus.Active)
        {
            familyMemberId = familyMemberId.HasValue ? familyMemberId : _currentUserService.MemberID;
            var choresQuery = Repository.Table<Chore>().Where(m => m.FamilyMemberID == familyMemberId && !m.IsDeleted && !m.FamilyMember.IsDeleted);
            var todayDate = DateTime.UtcNow.Date;

            switch (choreDueType)
            {
                case ChoreDueType.Overdue:
                    choresQuery = choresQuery.Where(m => m.ChoreStatus == choreStatus && DbFunctions.TruncateTime(m.DueDate) < todayDate);
                    break;
                case ChoreDueType.Today:
                    choresQuery = choresQuery.Where(m => m.ChoreStatus == choreStatus && DbFunctions.TruncateTime(m.DueDate) == todayDate);
                    break;
                case ChoreDueType.Upcoming:
                    choresQuery = choresQuery.Where(m => m.ChoreStatus == choreStatus && DbFunctions.TruncateTime(m.DueDate) > todayDate);
                    break;
            }

            return choresQuery.Count();
        }

        /// <summary>
        /// Gets the chores by family member identifier.
        /// </summary>
        /// <param name="familyMemberId">Family member identifier.</param>
        /// <returns>Chores.</returns>
        public List<Chore> GetChoresByFamilyMemberId(int? familyMemberId = null)
        {
            var memberId = familyMemberId.HasValue ? familyMemberId.Value : _currentUserService.MemberID;
            return Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId
                                                      && p.ChoreStatus == ChoreStatus.Active
                                                      && p.RecurringChoreID == null
                                                      && !p.IsDeleted).ToList();
        }

        /// <summary>
        /// Get chores by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="familyMemberId">The family member Identifier.</param>
        /// <returns>The list of chores</returns>
        public IList<Chore> GetChoresByDate(DateTime date, int? familyMemberId)
        {
            var memberId = familyMemberId.HasValue ? familyMemberId.Value : _currentUserService.MemberID;
            return Repository.Table<Chore>().Where(p => p.FamilyMemberID == memberId && DbFunctions.TruncateTime(p.DueDate.Value) == date.Date
                                                        && p.ChoreStatus == ChoreStatus.Active && !p.IsDeleted
                                                        && ((p.FrequencyType == FrequencyType.Once) || p.RecurringChoreID != null)).Include(p => p.FamilyMember).OrderByDescending(p => p.CreatedTime).ToList();
        }

        /// <summary>
        /// Updates the chore.
        /// </summary>
        /// <param name="chore">The chore.</param>
        /// <param name="weekDayName">The week day name.</param>
        /// <returns>The updated chore.</returns>
        public Chore Update(Chore chore, DayOfWeek? weekDayName = null)
        {
            var currentChore = Repository.Table<Chore>().SingleOrDefault(p => p.Id.Equals(chore.Id));
            if (currentChore == null)
                throw new InvalidParameterException("Invalid Chore!");

            DeleteRecurringChores(chore.Id);

            var completedOn = currentChore.CompletedOn == null ? DateTime.UtcNow : currentChore.CompletedOn;
            if (!chore.IsCompleted)
                completedOn = null;

            if (weekDayName.HasValue && chore.FrequencyType == FrequencyType.Once)
                chore.DueDate = DateTime.UtcNow.AddDays(-1 * (DateTime.UtcNow.DayOfWeek - weekDayName.Value)).ToUniversalTime();

            currentChore.Name = chore.Name;
            currentChore.Value = chore.Value;
            currentChore.ImageUrl = chore.ImageUrl;
            currentChore.DueDate = chore.DueDate;
            currentChore.FamilyMemberID = chore.FamilyMemberID;
            currentChore.FrequencyRange = chore.FrequencyRange;
            currentChore.FrequencyType = chore.FrequencyType;
            currentChore.IsDeleted = chore.IsDeleted;
            currentChore.SystemChoreID = chore.SystemChoreID;
            currentChore.RecurringChoreID = chore.RecurringChoreID;
            currentChore.IsCompleted = chore.IsCompleted;
            currentChore.ChoreStatus = chore.ChoreStatus;
            currentChore.CompletedOn = completedOn;

            Repository.Update(currentChore);
            return currentChore;
        }

        /// <summary>
        /// Disapprove today completed chores
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        public void DisapproveTodayChores(int familyMemberId)
        {
            var todayDate = DateTime.UtcNow.Date;
            var todayChores = Repository.Table<Chore>().Where(m => m.CompletedOn.HasValue && DbFunctions.TruncateTime(m.CompletedOn.Value) == todayDate
                                && m.FamilyMemberID == familyMemberId).ToList();

            foreach (var chore in todayChores)
            {
                chore.ChoreStatus = ChoreStatus.DisApproved;
                chore.DisapprovedOn = DateTime.UtcNow;

                Repository.Update(chore);
            }
        }

        /// <summary>
        /// Get childrens completed chores detail //Todo method name
        /// </summary>
        /// <returns>Result (List<ParentPhone, ChildName, ChoresCount>)</returns>
        public IList<Tuple<string, string, int>> GetTodayHeros()
        {
            var todayDate = DateTime.UtcNow.Date;

            // Get today completed chores grouped by family
            var groupedCompletedChores = Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                .Where(m => m.ChoreStatus == ChoreStatus.Completed && DbFunctions.TruncateTime(m.CompletedOn) == todayDate && !m.IsDeleted && !m.FamilyMember.IsDeleted)
                .ToList().GroupBy(m => m.FamilyMember.User.FamilyID);

            var result = new List<Tuple<string, string, int>>();
            foreach (var familyChores in groupedCompletedChores)
            {
                var parentMember = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == familyChores.Key && m.MemberType == MemberType.Admin);

                // Get grouped child chores
                var groupedChildChores = familyChores.GroupBy(m => m.FamilyMember.Firstname);

                foreach (var childChores in groupedChildChores)
                {
                    // Add (parent phone, child name, chores count)
                    result.Add(new Tuple<string, string, int>(parentMember.PhoneNumber.AppendCountyCode(), childChores.Key, childChores.Count()));
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the chore.
        /// </summary>
        /// <param name="choreId">The chore identifier.</param>
        public void Delete(int choreId)
        {
            var chore = Repository.Table<Chore>().SingleOrDefault(p => p.Id.Equals(choreId) && p.ChoreStatus == ChoreStatus.Active);
            if (chore == null)
                throw new InvalidParameterException("Invalid Chore!");

            if (chore.FrequencyType == FrequencyType.Once)
            {
                Repository.Delete(chore);
                return;
            }

            chore.IsDeleted = true;
            chore = Update(chore);

            DeleteRecurringChores(chore.Id);
        }

        /// <summary>
        /// Deleted the recurring chores if any.
        /// </summary>
        /// <param name="choreId">The master recurring chore identifier.</param>
        private void DeleteRecurringChores(int choreId)
        {
            // Delete the recurring chores
            var recurringChores = Repository.Table<Chore>().Where(p => p.RecurringChoreID == choreId && p.ChoreStatus == ChoreStatus.Active).ToList();
            foreach (var recurringChore in recurringChores)
            {
                Repository.Delete(recurringChore);
            }
        }

        private void UpdateSystemChore(int childId, int systemChoreId, string choreName, decimal choreValue, FrequencyType frequencyType)
        {
            var systemChore = Repository.Table<SystemChore>().Where(p => p.Id.Equals(systemChoreId)).SingleOrDefault();
            if (!systemChore.Name.Equals(choreName) || !systemChore.Value.Equals(choreValue) || !systemChore.FrequencyType.Equals(frequencyType))
            {
                var existingSystemChore = Repository.Table<SystemChore>().Where(p => p.ParentSystemChoreId == systemChoreId && p.ChildId == childId).SingleOrDefault();
                if (existingSystemChore != null)
                {
                    existingSystemChore.Name = choreName;
                    existingSystemChore.Value = choreValue;
                    existingSystemChore.FrequencyType = frequencyType;
                    Repository.Update(existingSystemChore);
                }
                else
                {
                    var chore = new SystemChore();
                    chore.Name = choreName;
                    chore.Value = choreValue;
                    chore.StartAge = systemChore.StartAge;
                    chore.EndAge = systemChore.EndAge;
                    chore.ImageUrl = systemChore.ImageUrl;
                    chore.FrequencyType = frequencyType;
                    chore.ChildId = childId;
                    chore.ParentSystemChoreId = systemChoreId;
                    Repository.Insert(chore);
                }
            }
        }

        /// <summary>
        /// Approves the payment for the payday.
        /// </summary>
        /// <param name="familyId">The family identifier</param>
        /// <returns></returns>
        public void ApprovePayDayPayment(int familyId)
        {
            var dtTodayUtc = DateTime.UtcNow;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;

            var nextPayDate = cstTimeZoneTime.GetNextPayDay().Date.AddHours(15);

            var startDate = nextPayDate.AddDays(-7);

            var Previousthirtyday = startDate.AddDays(-30);

            var chores = Repository.Table<Chore>().Where(m => m.FamilyMember.User.FamilyID == familyId && m.ChoreStatus == ChoreStatus.Completed
                                         && m.CompletedOn >= Previousthirtyday && m.CompletedOn <= nextPayDate && m.IncludedFlag == true
                                         && !m.BankTransactionID.HasValue && !m.FamilyMember.IsDeleted && !m.IsDeleted).ToList();

            foreach (var chore in chores)
            {
                chore.ChoreStatus = ChoreStatus.CompletedAndApproved;
                Repository.Update(chore);
            }


            _earningServices.PayDayChanges(familyId);
        }

        /// <summary>
        /// Disapproves the payment for the payday.
        /// </summary>
        /// <param name="familyId">The family identifier</param>
        /// <param name="isJob">The is job.</param>
        /// <returns></returns>
        public void DisapprovePayDayPayment(int familyId, bool isJob = false)
        {
            var dtTodayUtc = DateTime.UtcNow;
            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);
            DateTime cstTimeZoneTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DateTime;

            var nextPayDate = cstTimeZoneTime.GetNextPayDay().Date.AddHours(15);
            var startDate = nextPayDate.AddDays(-7);

            var chores = Repository.Table<Chore>().Where(m => m.FamilyMember.User.FamilyID == familyId && m.ChoreStatus == ChoreStatus.Completed
                                         && m.CompletedOn <= startDate
                                         && !m.BankTransactionID.HasValue && !m.FamilyMember.IsDeleted && !m.IsDeleted).ToList();

            foreach (var chore in chores)
            {
                chore.ChoreStatus = ChoreStatus.DisApproved;
                chore.IncludedFlag = false;
                Repository.Update(chore);
            }
        }

        public void CancelChore()
        {
            var dtTodayUtc = DateTime.UtcNow;
            var cutOffDateTime = DateTime.UtcNow;
            var lastCutOffTime = DateTime.UtcNow;
            // Console.WriteLine(dtTodayUtc.ToLocalTime());

            var timeZoneCST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var utcOffset = new DateTimeOffset(dtTodayUtc, TimeSpan.Zero);

            int timeToday = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).TimeOfDay.Hours;

            if (utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).DayOfWeek == DayOfWeek.Friday && timeToday == 10)
            {
                // Get payDay cutoff time
                cutOffDateTime = utcOffset.ToOffset(timeZoneCST.GetUtcOffset(utcOffset)).Date.AddDays(-1).AddHours(10);
                lastCutOffTime = cutOffDateTime.AddDays(-7);

                var chores = Repository.Table<Chore>().Where(m => m.ChoreStatus == ChoreStatus.Completed
                                         && m.CompletedOn <= cutOffDateTime && m.CompletedOn >= lastCutOffTime && m.IsCompleted == true
                                         && !m.BankTransactionID.HasValue && !m.FamilyMember.IsDeleted && !m.IsDeleted).ToList();

                foreach (var chore in chores)
                {
                    chore.ChoreStatus = ChoreStatus.Cancel;
                    Repository.Update(chore);
                }
            }
        }

        /// <summary>
        /// Pay chores payment
        /// </summary>


        /// <summary>
        /// Get admins with no activity by kids
        /// </summary>
        /// <param name="lastActivityDate">Last activity date</param>
        /// <returns>Family members</returns>
        public List<FamilyMember> GetAdminsWithNoActivityByKids(DateTime lastActivityDate)
        {
            // Get incomplete chore by completed chore date
            var completeChoresBeforeNDays = Repository.Table<Chore>().Where(p => DbFunctions.TruncateTime(p.CompletedOn) == lastActivityDate && !p.IsDeleted && !p.FamilyMember.IsDeleted).Select(m => m.FamilyMember).Distinct();

            // Get incomplete chore by after completed chore date
            var recentlyCompleteChores = Repository.Table<Chore>().Where(p => DbFunctions.TruncateTime(p.CompletedOn) > lastActivityDate && !p.IsDeleted && !p.FamilyMember.IsDeleted).Select(m => m.FamilyMember).Distinct();

            // Get members by chore except after completed chore date
            var noActivityChildrens = completeChoresBeforeNDays.Where(p => !recentlyCompleteChores.Any(s => s.Id == p.Id)).Select(p => p).Include(a => a.User);

            var admins = new List<FamilyMember>();
            foreach (var member in noActivityChildrens)
            {
                var parentMember = Repository.Table<FamilyMember>().Include(m => m.User).FirstOrDefault(m => m.User.FamilyID == member.User.FamilyID && m.MemberType == MemberType.Admin && m.User.Family.FamilySubscription.Status == SubscriptionStatus.Active);
                admins.Add(parentMember);
            }

            return admins;
        }

        /// <summary>
        /// Get continuous child activity
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Family members</returns>
        public List<FamilyMember> GetContinuousChildActivity(DateTime startDate, DateTime endDate)
        {
            var chores = Repository.Table<Chore>().Where(p => p.ChoreStatus == ChoreStatus.Completed && DbFunctions.TruncateTime(p.CompletedOn) > startDate && DbFunctions.TruncateTime(p.CompletedOn) <= endDate
                        && !p.IsDeleted && !p.FamilyMember.IsDeleted).Include(m => m.FamilyMember).Include(m => m.FamilyMember.User).ToList().GroupBy(p => p.FamilyMemberID);

            var activityDaysCount = (endDate.Date - startDate.Date).Days;
            var admins = new List<FamilyMember>();
            foreach (var chore in chores)
            {
                var choreCount = chore.Select(p => p.CompletedOn).Distinct().Count();
                if (choreCount != activityDaysCount)
                    continue;

                var familyId = chore.FirstOrDefault().FamilyMember.User.FamilyID;
                var parentMember = Repository.Table<FamilyMember>().Include(m => m.User).FirstOrDefault(m => m.User.FamilyID == familyId && m.MemberType == MemberType.Admin);
                admins.Add(parentMember);
            }
            return admins;
        }

        /// <summary>
        /// Gets the completed chores grouped by family.
        /// </summary>
        /// <param name="startDate">The start date of the week</param>
        /// <param name="endDate">The end date of the week (Friday is the payday)</param>
        /// <returns>The grouped chores list.</returns>
        public IEnumerable<IGrouping<int, Chore>> GetCompletedChoresByFamily(DateTime startDate, DateTime endDate)
        {
            var Previousthirtyday = startDate.AddDays(-30);

            return Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                                    .Where(m => m.ChoreStatus == ChoreStatus.Completed
                                    && m.CompletedOn >= Previousthirtyday.Date
                                    && m.CompletedOn <= endDate
                                    && !m.IsDeleted && !m.FamilyMember.IsDeleted)
                                    .ToList().GroupBy(m => m.FamilyMember.User.FamilyID);
        }


    }
}
