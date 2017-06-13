using LeapSpring.MJC.Core.Domain.Earnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Domain.Bonus;

namespace LeapSpring.MJC.BusinessLogic.Services.Earnings
{
    /// <summary>
    /// Represents a interface of earnings service
    /// </summary>
    public interface IEarningsService
    {
        /// <summary>
        /// Add child earnings
        /// </summary>
        /// <param name="childEarnings">Child earnings</param>
        void Add(ChildEarnings childEarnings);

        /// <summary>
        /// Update child earnings
        /// </summary>
        /// <param name="childEarnings">Child earnings</param>
        void Update(ChildEarnings childEarnings);

        /// <summary>
        /// Create new child earnings
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        void CreateNew(int familyMemberId);

        /// <summary>
        /// Get child earnings by family member identifier
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Child earnings</returns>
        ChildEarnings GetByMemberId(int familyMemberId);

        /// <summary>
        /// Pay chores payment
        /// </summary>
        void Pay();

        /// <summary>
        /// Gets the total earnigs of all childrens of the family
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns>The total amount in  buckets.</returns>
        decimal GetTotalEarningsByFamily(int familyId);

        /// <summary>
        /// Gets the financial overview of a child.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        ChildFinancialOverview GetChildFinancialOverview(DayOfWeek weekDay, int? familyMemberId = null);

        /// <summary>
        /// Move money
        /// </summary>
        /// <param name="sourceBucket">Source bucket</param>
        /// <param name="destinationBucket">Destination bucket</param>
        /// <param name="amount">Amount</param>
        /// <returns>Child earnings</returns>
        ChildEarnings MoveMoney(EarningsBucketType sourceBucket, EarningsBucketType destinationBucket, decimal amount);


        /// <summary>
        /// Show the Link upto 30 days.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        bool ShowThirtyDaysLink(int choreId);
        /// <summary>
        /// Send bonus to the child
        /// </summary>
        /// <param name="bonus">The bonus.</param>
        void SendBonus(ChildBonus bonus, int? familyMemberId = null);

        /// <summary>
        /// Can transact from earnings bucket
        /// </summary>
        /// <param name="bucketType">Earnings bucket type</param>
        /// <param name="amount">Amount</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>Result</returns>
        bool CanTransact(EarningsBucketType bucketType, decimal amount, int? familyMemberId = null);
        /// <summary>
        /// Resets the all child's buckets of the family
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        void ResetChildEarningsByFamily(int familyId);
        /// <summary>
        /// Get Child Approval Status
        /// </summary>
        /// <param name="choreId"></param>
        /// <returns></returns>
        childApprovalDetails RemoveApprovalService(int choreId);
        childApprovalDetails ApproveForPayday(int choreId);
        void PayDayChanges(int familyID);
        bool ShowApproveDisapprovelink(int choreID);
    }
}
