using CorePro.SDK;
using CorePro.SDK.Models;
using LeapSpring.MJC.Core.Domain.Family;
using System.Collections.Generic;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public interface ICoreProService
    {
        /// <summary>
        /// Creates the corepro customer.
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <returns>The corepro customer.</returns>
        Customer CreateCustomer(FamilyMember adminMember);

        /// <summary>
        /// Creates the corepro account for the customer.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="accountName">The name of the account.</param>
        /// <returns>The corepro account identifier.</returns>
        int CreateAccount(int customerId, string accountName);

        /// <summary>
        ///  Creates the external bank account for the customer.
        /// </summary>
        /// <param name="customerID">The corepro customer identifier.</param>
        /// <param name="institutionName">The bank name.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="accountType">The external account type.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="isTrialDeposit">The is trial deposit.</param>
        /// <returns>The external account</returns>
        ExternalAccount CreateExternalAccount(int customerID, string institutionName, string firstName, string lastName, string accountType, string routingNumber, string accountNumber, bool isTrialDeposit = false);

        /// <summary>
        /// Creates the corepro money transfer.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="fromAccountId">The source account identifier.</param>
        /// <param name="toAccountId">The destination account identifier.</param>
        /// <param name="amount">The amount to be transferred.</param>
        /// <returns>The list of transfers created.</returns>
        IList<Transfer> CreateTransfer(int customerId, int fromAccountId, int toAccountId, decimal amount);

        /// <summary>
        /// Gets the corepro account.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="accountId">The corepro account identifier.</param>
        /// <returns>The corepro account</returns>
        CorePro.SDK.Account GetAccount(int customerId, int accountId);

        /// <summary>
        /// Gets the corepro account.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <returns>The corepro account</returns>
        ExternalAccount GetExternalAccount(int customerId, int externalAccountId);

        /// <summary>
        /// Gets the bank documents
        /// </summary>
        /// <returns>The list of the bank documents.</returns>
        List<Document> GetBankDocuments();

        /// <summary>
        /// Gets the bank documents by document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>The file content.</returns>
        FileContent GetBankDocumentById(int documentId);

        /// <summary>
        /// Disconnects the external account for corepro
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <returns></returns>
        bool ArchiveExternalAccount(int customerId, int externalAccountId);

        /// <summary>
        /// Verifies an external account.
        /// </summary>
        /// <param name="customerID">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <param name="firstAmount">The first amount.</param>
        /// <param name="secondAmount">The second amount.</param>
        /// <returns><c>True</c>, If verified. Otherwise, <c>False</c>.</returns>
        bool VerifyExternalAccount(int customerID, int externalAccountId, decimal firstAmount, decimal secondAmount);


        /// <summary>
        /// Update the corepro customer Email.
        /// </summary>
        /// <param name="cutomerid">The admin member of the family</param>
        /// <returns>The corepro customer.</returns>
        string UpdateEmail(int cutomerid, string email);
        Customer get(int cutomerid);
    }
}
