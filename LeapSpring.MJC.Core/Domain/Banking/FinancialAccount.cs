using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
namespace LeapSpring.MJC.Core.Domain.Banking
{
    public class FinancialAccount : BaseEntity
    {
        public FinancialAccount()
        {
            DateAdded = DateTime.UtcNow;
            isVerifyMailsent = false;

        }
        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the corepro customer identifier.
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the corepro account identifier.
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the corepro exter account identifier.
        /// </summary>
        public int? ExternalAccountID { get; set; }

        /// <summary>
        /// Gets or sets the bank name.
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Gets or sets the masked account number.
        /// </summary>
        public string MaskedAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the status of the account linked.
        /// </summary>
        public FinancialAccountStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the plaid access token.
        /// </summary>
        public string PlaidAccessToken { get; set; }

        public DateTime DateAdded { get; set; }

        public bool isVerifyMailsent { get; set; }
        /// <summary>
        /// Gets or sets the account type.
        /// </summary>
        public FundingSourceType? AccountType { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }
    }
}
