using LeapSpring.MJC.Core.Dto;
using System;
using System.Collections.Generic;

namespace LeapSpring.MJC.BusinessLogic.Services.Earnings
{
    public interface ITransactionHistoryService
    {
        /// <summary>
        /// Gets all transactions done by a child.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The list of transactions grouped by date. </returns>
        Dictionary<DateTime, List<TransactionHistory>> GetAllTransactions(int? familyMemberId = null);

        /// <summary>
        /// Gets allowance out.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The list of transactions grouped by date. </returns>
        Dictionary<DateTime, List<TransactionHistory>> GetAllowanceIn(int? familyMemberId = null);

        /// <summary>
        /// Gets allowance out.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The list of transactions grouped by date. </returns>
        Dictionary<DateTime, List<TransactionHistory>> GetAllowanceOut(int? familyMemberId = null);
    }
}
