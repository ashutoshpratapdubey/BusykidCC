namespace LeapSpring.MJC.Core.Domain.Banking
{
    public class CoreproSettings : BaseEntity
    {
        /// <summary>
        /// Gets or sets the corepro program's reserve account identifier.
        /// </summary>
        public int ReserveAccountId { get; set; }

        /// <summary>
        /// Gets or sets the corepro program's subscription clearing account identifier.
        /// </summary>
        public int SubscriptionClearingAccountId { get; set; }

        /// <summary>
        /// Gets or sets the corepro program's stockpile clearing account identifier.
        /// </summary>
        public int StockPileClearingAccountId { get; set; }

        /// <summary>
        /// Gets or sets the corepro program's giftcard clearing account identifier.
        /// </summary>
        public int GiftCardClearingAccountId { get; set; }
    }
}
