using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Banking
{
    public class CreditCardAccount : BaseEntity
    {

        public CreditCardAccount()
        {
            DateAdded = DateTime.UtcNow;
            Isdeleted = false;
        }

        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the CodeFlex customer identifier.
        /// </summary>
        public string customer_vault_id { get; set; }

        /// <summary>
        /// Gets or sets the Credit card Type.
        /// </summary>
        public CreditCardType CardType { get; set; }

        /// <summary>
        /// Gets or sets the masked credit card number.
        /// </summary>
        public string MaskedCardNumber { get; set; }


        /// <summary>
        /// Gets or sets the status of the account linked.
        /// </summary>
        public CreditCardStatus CardStatus { get; set; }

        /// <summary>
        /// Gets or sets the card expiration Month.
        /// </summary>
        public int CardExpirationMonth { get; set; }

        /// <summary>
        /// Gets or sets the card expiration Year.
        /// </summary>
        public int CardExpirationYear { get; set; }

        /// <summary>
        /// Gets or sets the card is expired or active
        /// </summary>
        public bool IsCardExpired { get; set; }

        /// <summary>
        /// Gets or sets the card is removed from client
        /// </summary>
        public bool Isdeleted { get; set; }


        /// <summary>
        /// Gets or sets the card added date.
        /// </summary>
        public DateTime DateAdded { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }

    }
}
