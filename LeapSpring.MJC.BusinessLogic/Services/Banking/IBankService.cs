using LeapSpring.MJC.Core.Domain.Banking;
using System.Threading.Tasks;
using System.Xml;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public interface IBankService
    {
        /// <summary>
        /// Is bank account linked
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Result [IsLinked]</returns>
        bool IsBankLinked(int? familyMemberId = null);

        /// <summary>
        /// Gets the financial account.
        /// </summary>
        /// <param name="adminMemberId">The admin member identifier of the family.</param>
        /// <returns>The financial account.</returns>
        FinancialAccount GetFinancialAccount(int? adminMemberId = null);

        /// <summary>
        /// checks the member has financial account or not. 
        /// </summary>
        /// <param name="adminMemberId">The admin member identifier of the family.</param>
        /// <returns><c>True</c> if not finance account present. otherwise, <c>False</c>.</returns>
        bool HasFinancialAccount(int? adminMemberId = null);

        /// <summary>
        /// Removes the bank account
        /// </summary>
        Task RemoveBank();

        CreditCardAccount GetCreditCard();

        CreditCardAccount GetCreditCardInfo();

        CreditCardAccount UpdateCardStatus();
        string sendXMLRequest(XmlDocument xmlRequest);

        void AddCreditCardDetails(string customerVaultId, string ccexp, string ccnumber,string cardtype);
    }
}
