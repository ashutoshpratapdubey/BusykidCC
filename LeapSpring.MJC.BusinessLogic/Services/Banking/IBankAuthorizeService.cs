using CorePro.SDK;
using CorePro.SDK.Models;
using LeapSpring.MJC.Core.Dto.Banking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    /// <summary>
    /// Represents a interface bank authorize service
    /// </summary>
    public interface IBankAuthorizeService
    {
        #region CorePro

        /// <summary>
        /// Creates the corepro customer and account.
        /// </summary>
        void CreateCustomer();

        /// <summary>
        /// Links the bank account.
        /// </summary>
        /// <param name="publicToken">The public key token.</param>
        /// <param name="institutionName">Name of the institution.</param>
        /// <param name="selectedAccountId">Selected account identifier</param>
        Task LinkBankAccount(string publicToken, string institutionName, string selectedAccountId);

        /// <summary>
        /// Links the micro deposit account.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountType">The account type.</param>
        Task LinkMicroDepositAccount(string accountNumber, string routingNumber, string accountType);

        /// <summary>
        /// Verifies the bank account.
        /// </summary>
        /// <param name="firstAmount">The first amount received.</param>
        /// <param name="secondAmount">The second amoount received.</param>
        Task VerifyBankAccount(decimal firstAmount, decimal secondAmount);

        /// <summary>
        /// Gets the bank documents.
        /// </summary>
        /// <returns>The list of bank documents to be displayed.</returns>
        List<Document> GetBankDocuments();

        /// <summary>
        /// Gets the bank documents by document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>The file content.</returns>
        FileContent GetBankDocumentById(int documentId);

        #endregion
    }
}
