using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Dto.Banking;
using LeapSpring.MJC.Core.Enums;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    /// <summary>
    /// Represents a interface of transaction service
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Save bank transaction
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        void SaveBankTransaction(BankTransaction bankTransaction);
        
        /// <summary>
        /// Update bank transaction
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        void UpdateBankTransaction(BankTransaction bankTransaction);

        /// <summary>
        /// Update transaction status
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="transferStatus">Transaction status</param>
        void UpdateTransactionStatus(string transactionId, TransactionStatus transferStatus);

        /// <summary>
        /// Save transaction log
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="reason">Reason</param>
        /// <param name="amount">Amount</param>
        void SaveTransactionLog(int familyMemberId, string reason, decimal amount);

        #region Earnings & Payment

        /// <summary>
        /// Process all pending payment
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        void ProcessPayment(BankTransaction bankTransaction);
        
        /// <summary>
        /// Allocate amount to earnings bucket
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="totalAmount">Total amount</param>
        void AllocateEarnings(int familyMemberId, decimal totalAmount);

        #endregion

        #region CorePro

        /// <summary>
        /// Transfers fund between accounts.
        /// </summary>
        /// <param name="adminMemberId">The admin member identifier of the family member.</param>
        /// <param name="amount">The amount to be transfered.</param>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="transferType">The type of transfer.</param>
        /// <returns></returns>
        int? Transfer(int adminMemberId, decimal amount, PaymentType paymentType, TransferType transferType);

        /// <summary>
        /// Mark as transaction created
        /// </summary>
        /// <param name="dwollaWebhooksResponse">Dwolla webhooks response</param>
        void MarkAsTransferCreated(MessageEvent messageEvent);

        /// <summary>
        /// Marks the transaction as completed.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        void MarkAsCompleted(MessageEvent messageEvent);

        /// <summary>
        /// Marks the transaction as settled.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        void MarkAsSettled(MessageEvent messageEvent);

        /// <summary>
        /// Marks the transaction as failed.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        void MarkAsFalied(MessageEvent messageEvent);

        /// <summary>
        /// Marks the transaction as cancelled.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        void MarkAsCancelled(MessageEvent messageEvent);

        Task TestEvent(MessageEvent messageEvent);

        #endregion
    }
}
