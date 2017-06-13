namespace LeapSpring.MJC.Core.Dto.Accounts
{
    /// <summary>
    /// Represents a sign up progress details
    /// </summary>
    public class SignUpProgress
    {
        /// <summary>
        /// Gets or sets the member identifier
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether account created
        /// </summary>
        /// <value>
        /// <c>true</c> if account created; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccountCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether added child
        /// </summary>
        /// <value>
        /// <c>true</c> if added child; otherwise, <c>false</c>.
        /// </value>
        public bool IsAddedChild { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether assigned some chores
        /// </summary>
        /// <value>
        /// <c>true</c> if assigned some chores; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssignedSomeChores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether linked to bank
        /// </summary>
        /// <value>
        /// <c>true</c> if linked to bank; otherwise, <c>false</c>.
        /// </value>
        public bool IsLinkedToBank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether linked to Card
        /// </summary>
        /// <value>
        /// <c>true</c> if linked to card; otherwise, <c>false</c>.
        /// </value>
        public bool IsLinkedToCreditCard { get; set; }

        /// <summary>
        /// Gets or sets the last child identifier
        /// </summary>
        public int? LastChildId { get; set; }

        /// <summary>
        /// Gets or sets a bank status
        /// </summary>
        public string BankStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pin
        /// </summary>
        /// <value>
        /// <c>true</c> if pin; otherwise, <c>false</c>.
        /// </value>
        public bool HasPin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether phone number
        /// </summary>
        /// <value>
        /// <c>true</c> if phone number; otherwise, <c>false</c>.
        /// </value>
        public bool HasPhoneNumber { get; set; }
    }
}
