using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using LeapSpring.MJC.Core.Domain.Earnings;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Domain.Save;
using System.Collections.Generic;
using LeapSpring.MJC.Core.Dto.Banking;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    /// <summary>
    /// Represent a transaction service
    /// </summary>
    public class TransactionService : ServiceBase, ITransactionService
    {
        private IEmailTemplateService _emailTemplateService;
        private IEmailService _emailService;
        private IAllocationSettingsService _allocationSettingsService;
        private IAppSettingsService _appSettingsService;
        private ITextMessageService _textMessageService;
        private ICoreProService _coreProService;
        private IBankService _bankService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public TransactionService(IRepository repository, IEmailTemplateService emailTemplateService,
            IEmailService emailService, IAllocationSettingsService allocationSettingsService, IAppSettingsService appSettingsService,
            ITextMessageService textMessageService, ICoreProService coreProService, IBankService bankService) : base(repository)
        {
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _allocationSettingsService = allocationSettingsService;
            _appSettingsService = appSettingsService;
            _textMessageService = textMessageService;
            _coreProService = coreProService;
            _bankService = bankService;
        }

        #region Utilities

        /// <summary>
        /// Prepare email template values
        /// </summary>
        /// <param name="bodyContent">Email body content</param>
        /// <param name="bankTransaction">Bank transaction</param>
        /// <returns>Body content</returns>
        private string PrepareTemplateValues(string bodyContent, BankTransaction bankTransaction)
        {
            var financialAccount = Repository.Table<FinancialAccount>().SingleOrDefault(m => m.FamilyMemberID == bankTransaction.FamilyMemberID);

            bodyContent = bodyContent.Replace("{{customername}}", bankTransaction.FamilyMember.Firstname.FirstCharToUpper());
            bodyContent = bodyContent.Replace("{{bankname}}", financialAccount.BankName);
            bodyContent = bodyContent.Replace("{{$x}}", string.Format("${0}", bankTransaction.Amount.ToString()));
            bodyContent = bodyContent.Replace("{{amount}}", string.Format("${0}", bankTransaction.Amount.ToString()));
            bodyContent = bodyContent.Replace("{{date}}", DateTime.Now.ToString("MM/dd/yyyy"));
            bodyContent = bodyContent.Replace("{{createdOn}}", DateTime.Now.ToString("MM/dd/yyyy"));

            // Get transaction type as string
            var paymentType = bankTransaction.PaymentType == PaymentType.Chore ? "Pay Day" : bankTransaction.PaymentType.ToString();
            bodyContent = bodyContent.Replace("{{transactiontype}}", paymentType);
            return bodyContent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get bank transaction
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <returns>Bank transaction</returns>
        public BankTransaction GetBankTransaction(string transactionId)
        {
            return Repository.Table<BankTransaction>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User)
                .SingleOrDefault(m => m.TransactionId == transactionId);
        }

        /// <summary>
        /// Save bank transaction
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        public void SaveBankTransaction(BankTransaction bankTransaction)
        {
            bankTransaction.CreatedOn = DateTime.UtcNow;
            Repository.Insert(bankTransaction);
        }

        /// <summary>
        /// Update bank transaction
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        public void UpdateBankTransaction(BankTransaction bankTransaction)
        {
            bankTransaction.LastUpdatedOn = DateTime.UtcNow;
            Repository.Update(bankTransaction);

            if (bankTransaction.TransactionStatus == TransactionStatus.Cancelled || bankTransaction.TransactionStatus == TransactionStatus.Failed)
            {
                switch (bankTransaction.PaymentType)
                {
                    case PaymentType.Chore:
                        UpdateChoreStatus(bankTransaction.Id, ChoreStatus.CompletedAndPaymentFailed);
                        break;
                    case PaymentType.Charity:
                        RevertDonation(bankTransaction.Id);
                        break;
                    case PaymentType.CashOut:
                        RevertCashOut(bankTransaction.Id);
                        break;
                    case PaymentType.SubscriptionInitiated:
                    case PaymentType.Subscription:
                        CancelAnnualSubscription(bankTransaction.Id);
                        break;
                    case PaymentType.StockPile:
                        CancelStockPurchaseRequest(bankTransaction.Id);
                        break;
                    case PaymentType.GiftCard:
                        CancelPurchasedGiftCard(bankTransaction.Id);
                        break;
                }
            }
        }

        /// <summary>
        /// updates the chore status.
        /// </summary>
        /// <param name="bankTransaction">The bank tranaction identifier</param>
        /// <param name="choreStatus">The chore status</param>
        private void UpdateChoreStatus(int bankTransactionId, ChoreStatus choreStatus)
        {
            var chores = Repository.Table<Chore>().Where(m => m.BankTransactionID == bankTransactionId).ToList();
            foreach (var chore in chores)
            {
                chore.ChoreStatus = choreStatus;
                Repository.Update(chore);
            }
        }

        /// <summary>
        /// Returns the donated amount from the bucket.
        /// </summary>
        /// <param name="bankTransaction">The bank tranaction identifier</param>
        private void RevertDonation(int bankTransactionId)
        {
            var donation = Repository.Table<Donation>().SingleOrDefault(d => d.BankTransactionID == bankTransactionId);
            if (donation == null) return;

            // Updates child earnings
            var childEarnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID == donation.FamilyMemberID);
            childEarnings.Share += donation.Amount;
            Repository.Update(childEarnings);

            donation.ApprovalStatus = ApprovalStatus.TransasctionFailed;
            Repository.Update(donation);
        }

        /// <summary>
        /// Returns the donated amount from the bucket.
        /// </summary>
        /// <param name="bankTransaction">The bank tranaction identifier</param>
        private void RevertCashOut(int bankTransactionId)
        {
            var cashOut = Repository.Table<CashOut>().SingleOrDefault(d => d.BankTransactionID == bankTransactionId);
            if (cashOut == null) return;

            // Updates child earnings
            var childEarnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID == cashOut.ChildID);
            childEarnings.Spend += cashOut.Amount;
            Repository.Update(childEarnings);

            cashOut.ApprovalStatus = ApprovalStatus.TransasctionFailed;
            Repository.Update(cashOut);
        }

        /// <summary>
        /// Cancels the annual subscription
        /// </summary>
        /// <param name="bankTransactionId">The bank transaction identifier.</param>
        private void CancelAnnualSubscription(int bankTransactionId)
        {
            var familySubscription = Repository.Table<FamilySubscription>().SingleOrDefault(d => d.BankTransactionID == bankTransactionId);
            if (familySubscription == null) return;

            // Updates child earnings
            familySubscription.EndsOn = DateTime.UtcNow.AddDays(-1);
            familySubscription.Status = SubscriptionStatus.Cancelled;
            Repository.Update(familySubscription);
        }

        /// <summary>
        /// Cancells the stock purchase request.
        /// </summary>
        /// <param name="bankTransactionId">The bank transaction identifier.</param>
        private void CancelStockPurchaseRequest(int bankTransactionId)
        {
            var purchasedStockRequest = Repository.Table<StockPurchaseRequest>().SingleOrDefault(d => d.BankTransactionID == bankTransactionId);
            if (purchasedStockRequest == null) return;

            // Updates child earnings
            var childEarnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID == purchasedStockRequest.ChildID);
            childEarnings.Save += (purchasedStockRequest.Amount + purchasedStockRequest.Fee); // Adding stock amount including Fee
            Repository.Update(childEarnings);

            purchasedStockRequest.Status = ApprovalStatus.TransasctionFailed;
            Repository.Update(purchasedStockRequest);
        }

        /// <summary>
        /// Cancells the gift purchased card.
        /// </summary>
        /// <param name="bankTransactionId">The bank transaction identifier.</param>
        private void CancelPurchasedGiftCard(int bankTransactionId)
        {
            var purchasedGiftCard = Repository.Table<PurchasedGiftCard>().SingleOrDefault(d => d.BankTransactionID == bankTransactionId);
            if (purchasedGiftCard == null) return;

            // Updates child earnings
            var childEarnings = Repository.Table<ChildEarnings>().SingleOrDefault(p => p.FamilyMemberID == purchasedGiftCard.FamilyMemberID);
            childEarnings.Spend += purchasedGiftCard.Amount;
            Repository.Update(childEarnings);

            purchasedGiftCard.Status = ApprovalStatus.TransasctionFailed;
            Repository.Update(purchasedGiftCard);
        }

        /// <summary>
        /// Update transaction status
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="transferStatus">Transaction status</param>
        public void UpdateTransactionStatus(string transactionId, TransactionStatus transferStatus)
        {
            var bankTransaction = GetBankTransaction(transactionId);

            bankTransaction.LastUpdatedOn = DateTime.UtcNow;
            bankTransaction.TransactionStatus = transferStatus;
            Repository.Update(bankTransaction);
        }

        /// <summary>
        /// Save transaction log
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="reason">Reason</param>
        /// <param name="amount">Amount</param>
        public void SaveTransactionLog(int familyMemberId, string reason, decimal amount)
        {
            var trasactionLog = new TransactionLog
            {
                FamilyMemberID = familyMemberId,
                Reason = reason,
                Amount = amount,
                CreatedOn = DateTime.UtcNow
            };

            Repository.Insert(trasactionLog);
        }

        #endregion

        #region Earnings & Payment

        /// <summary>
        /// Process chore payment
        /// </summary>
        /// <param name="bankTransactionId">bank transaction identifier</param>
        private void ProcessChorePayment(int bankTransactionId)
        {
            // Update pending chores to complete
            UpdateChoreStatus(bankTransactionId, ChoreStatus.CompletedAndPaid);
            var chores = Repository.Table<Chore>().Include(m => m.FamilyMember).Include(m => m.FamilyMember.User).Include(m => m.FamilyMember.User.Family)
                .Where(m => m.BankTransactionID == bankTransactionId && !m.IsDeleted && !m.FamilyMember.IsDeleted).ToList();
            var groupedChores = chores.GroupBy(m => m.FamilyMemberID);
            foreach (var childChores in groupedChores)
            {
                var totalChorePayment = childChores.Sum(m => m.Value);
                AllocateEarnings(childChores.Key, totalChorePayment);

                var child = childChores.FirstOrDefault().FamilyMember;
                var adminMember = Repository.Table<FamilyMember>().FirstOrDefault(m => m.User.FamilyID == child.User.FamilyID && m.MemberType == MemberType.Admin);

                // Sms child to know their balance
                if (!string.IsNullOrEmpty(child.PhoneNumber))
                {
                    var url = _appSettingsService.SiteUrl + "#/family/" + child.User?.Family?.UniqueName;
                    var message = $"Hi {child.Firstname.FirstCharToUpper()}! Your payday payment of ${totalChorePayment:N2} has been processed. {url}";
                    _textMessageService.Send(child.PhoneNumber, message);
                }
                else
                {
                    var message = $"Hi {adminMember.Firstname.FirstCharToUpper()}! {child.Firstname.FirstCharToUpper()}'s payday payment of ${totalChorePayment:N2} has been processed.";
                    if (!string.IsNullOrEmpty(adminMember.PhoneNumber))
                        _textMessageService.Send(adminMember.PhoneNumber, message);
                }
            }
        }

        /// <summary>
        /// Process bonus payment
        /// </summary>
        /// <param name="bankTransactionId">bank transaction identifier</param>
        private void ProcessBonusPayment(int bankTransactionId)
        {
            // Update pending child bonus to complete
            var childBonus = Repository.Table<ChildBonus>()
                .Include(p => p.Child)
                .Include(p => p.Contributor)
                .Include(p => p.Contributor.User)
                .Include(p => p.Contributor.User.Family)
                .SingleOrDefault(m => m.BankTransactionID == bankTransactionId);
            if (childBonus != null)
            {
                AllocateEarnings(childBonus.ChildID, childBonus.Amount);

                if (!string.IsNullOrEmpty(childBonus.Child.PhoneNumber))
                {
                    var message = string.Format(_appSettingsService.TransactionCompletedChildHasPhoneMessage, childBonus.Child.Firstname, childBonus.Amount, _appSettingsService.SiteUrl + "#/family/" + childBonus.Contributor.User.Family.UniqueName);
                    _textMessageService.Send(childBonus.Child.PhoneNumber, message);
                }
                else if (!string.IsNullOrEmpty(childBonus.Contributor.PhoneNumber))
                {
                    var message = string.Format(_appSettingsService.TransactionCompletedChildHasNoPhoneMessage, childBonus.Contributor.Firstname, childBonus.Child.Firstname, childBonus.Amount);
                    _textMessageService.Send(childBonus.Contributor.PhoneNumber, message);
                }
            }
        }

        /// <summary>
        /// Cancels the family subcription and removes the bank account
        /// </summary>
        /// <param name="bankTransactionId">The bank transaction identifier.</param>
        /// <returns></returns>
        private void CancelSubscription(int bankTransactionId)
        {
            // Gets the subscription cancellation request by bank transaction identifier
            var subscriptionCancellationRequest = Repository.Table<SubscriptionCancellationRequest>()
               .Include(p => p.FamilyMember)
               .Include(p => p.FamilyMember.User)
               .Include(p => p.FamilyMember.User.Family)
               .SingleOrDefault(p => p.BankTransactionID == bankTransactionId);
            if (subscriptionCancellationRequest == null || (subscriptionCancellationRequest != null && !subscriptionCancellationRequest.IsActive))
                return;

            subscriptionCancellationRequest.IsActive = false;
            Repository.Update(subscriptionCancellationRequest);
            // Cancels the family subscription
            var familySubscription = Repository.Table<FamilySubscription>().
                SingleOrDefault(p => p.Id == subscriptionCancellationRequest.FamilyMember.User.Family.FamilySubscriptionID);
            if (familySubscription == null)
                return;

            //// Removes the bank
            //await _bankService.RemoveBank();

            familySubscription.Status = SubscriptionStatus.Cancelled;
            Repository.Update(familySubscription);
        }

        /// <summary>
        /// Process all pending payment
        /// </summary>
        /// <param name="bankTransaction">Bank transaction</param>
        public void ProcessPayment(BankTransaction bankTransaction)
        {
            switch (bankTransaction.PaymentType)
            {
                case PaymentType.Chore:
                    ProcessChorePayment(bankTransaction.Id);
                    break;
                case PaymentType.Bonus:
                    ProcessBonusPayment(bankTransaction.Id);
                    break;
                case PaymentType.SubscriptionCancellation:
                    CancelSubscription(bankTransaction.Id);
                    break;
                case PaymentType.SubscriptionInitiated:
                    ProcessSubscriptionPayment(bankTransaction.Id);
                    break;
            }
        }

        /// <summary>
        /// Allocate amount to earnings bucket
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <param name="totalAmount">Total amount</param>
        public void AllocateEarnings(int familyMemberId, decimal totalAmount)
        {
            var allocationSettings = _allocationSettingsService.GetByMemberId(familyMemberId);

            // Calculate chore payments, then split
            decimal saveAmount = Math.Round(totalAmount * (allocationSettings.Save / 100), 3);
            decimal shareAmount = Math.Round(totalAmount * (allocationSettings.Share / 100), 3);

            string[] amountArray = saveAmount.ToString().Split(new char[] { '.' });
            int afterDecimalRoundAmount = Convert.ToInt32(Convert.ToDecimal(amountArray[1]) % 1000);
            int counter = 0;
            if (afterDecimalRoundAmount >= 5)
            {
                saveAmount = Math.Round(totalAmount * (allocationSettings.Save / 100), 2);
                counter++;
            }
            else
                saveAmount = Math.Round(totalAmount * (allocationSettings.Save / 100), 2);

            amountArray = shareAmount.ToString().Split(new char[] { '.' });
            afterDecimalRoundAmount = Convert.ToInt32(Convert.ToDecimal(amountArray[1]) % 1000);

            if (afterDecimalRoundAmount >= 5 && counter == 0)
                shareAmount = Math.Round(totalAmount * (allocationSettings.Share / 100), 2);
            else
                shareAmount = Math.Truncate(100 * (totalAmount * (allocationSettings.Share / 100))) / 100;

            //Client Suggested
            var spendAmount = Math.Round(totalAmount, 2) - saveAmount - shareAmount;
            // Get child earnings
            var childEarnings = Repository.Table<ChildEarnings>().SingleOrDefault(m => m.FamilyMemberID == familyMemberId);
            var hasChildEarnings = (childEarnings != null);
            childEarnings = childEarnings ?? new ChildEarnings { FamilyMemberID = familyMemberId };

            // Update child earnings
            childEarnings.Save += saveAmount;
            childEarnings.Share += shareAmount;
            childEarnings.Spend += spendAmount;
            if (hasChildEarnings)
                Repository.Update(childEarnings);
            else
                Repository.Insert(childEarnings);
        }

        /// <summary>
        /// Transfers the subscription amount from customer account to corepro's program reserve account.
        /// </summary>
        /// <param name="bankTransactionId">The bank transaction identifier.</param>
        private void ProcessSubscriptionPayment(int bankTransactionId)
        {
            var bankTransaction = Repository.Table<BankTransaction>().SingleOrDefault(m => m.Id == bankTransactionId);

            try
            {
                if (!_bankService.IsBankLinked(bankTransaction.FamilyMemberID))
                    throw new InvalidOperationException("Bank is not linked or verified!");

                // Tranfer amount from customer's internal account to program's reserve account
                var transactionResult = Transfer(bankTransaction.FamilyMemberID, bankTransaction.Amount, PaymentType.Subscription, TransferType.InternalToBusyKidInternalAccount);

                // If transaction failure
                if (!transactionResult.HasValue)
                    throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                var familySubscription = Repository.Table<FamilySubscription>().SingleOrDefault(m => m.BankTransactionID == bankTransaction.Id);
                familySubscription.BankTransactionID = transactionResult;
                Repository.Update(familySubscription);
            }
            catch (Exception ex)
            {
                SaveTransactionLog(bankTransaction.FamilyMemberID, ex.Message, bankTransaction.Amount);
                throw new InvalidOperationException(ex.Message);
            }
        }

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
        public int? Transfer(int adminMemberId, decimal amount, PaymentType paymentType, TransferType transferType)
        {
            // Gets the financial account
            var financialAccount = _bankService.GetFinancialAccount(adminMemberId);
            IList<CorePro.SDK.Transfer> transfers = null;

            // Transfers the amount based on transfer type
            switch (transferType)
            {
                case TransferType.ExternalToInetrnalAccount:
                    transfers = _coreProService.CreateTransfer(financialAccount.CustomerID, financialAccount.ExternalAccountID.Value, financialAccount.AccountID, amount);
                    break;
                case TransferType.InternalToExternalAccount:
                    transfers = _coreProService.CreateTransfer(financialAccount.CustomerID, financialAccount.AccountID, financialAccount.ExternalAccountID.Value, amount);
                    break;
                case TransferType.InternalToBusyKidInternalAccount:
                    var programAccountId = GetProgramAccountId(paymentType);
                    transfers = _coreProService.CreateTransfer(financialAccount.CustomerID, financialAccount.AccountID, programAccountId.Value, amount);
                    break;
            }

            if (!transfers.Any())
                return null;

            // Creates the bank transaction for successfull transfer.
            var currentTransfer = transfers.FirstOrDefault();

            var bankTransaction = new BankTransaction
            {
                FamilyMemberID = adminMemberId,
                Amount = amount,
                TransactionId = currentTransfer.TransactionId.ToString(),
                TransactionStatus = transferType == TransferType.InternalToBusyKidInternalAccount ? TransactionStatus.Completed : TransactionStatus.Pending,
                PaymentType = paymentType
            };
            SaveBankTransaction(bankTransaction);

            return bankTransaction.Id;
        }

        /// <summary>
        /// Gets the corepro program's account identifier.
        /// </summary>
        /// <param name="paymentType">The payment type.</param>
        /// <returns>The internal account identifier.</returns>
        private int? GetProgramAccountId(PaymentType paymentType)
        {
            var coreproSettings = Repository.Table<CoreproSettings>().FirstOrDefault();

            if (paymentType == PaymentType.StockPile)
                return coreproSettings?.StockPileClearingAccountId;
            else if (paymentType == PaymentType.GiftCard)
                return coreproSettings?.GiftCardClearingAccountId;
            else if (paymentType == PaymentType.Subscription)
                return coreproSettings?.SubscriptionClearingAccountId;
            return null;
        }
        #endregion

        #region Webhooks Methods

        /// <summary>
        /// Mark as transaction created
        /// </summary>
        /// <param name="dwollaWebhooksResponse">Dwolla webhooks response</param>
        public void MarkAsTransferCreated(MessageEvent messageEvent)
        {
            var bankTransaction = GetBankTransaction(messageEvent.TransactionId.ToString());
            if (bankTransaction == null || bankTransaction.TransactionStatus != TransactionStatus.Pending) return;

            //if (bankTransaction.FamilyMember.IsUnSubscribed)
            //    return;

            //var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.BankTransferCreated);
            //var bodyContent = emailTemplate?.Content ?? "Transaction created from busy kid ${{amount}} on {{createdOn}}";

            //bodyContent = PrepareTemplateValues(bodyContent, bankTransaction);
            //_emailService.Send(bankTransaction.FamilyMember.User.Email, emailTemplate.Subject, bodyContent);
        }

        /// <summary>
        /// Marks the transaction as completed.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        public void MarkAsCompleted(MessageEvent messageEvent)
        {
            var bankTransaction = GetBankTransaction(messageEvent.TransactionId.ToString());
            if (bankTransaction == null || bankTransaction.TransactionStatus == TransactionStatus.Completed) return;
            bankTransaction.TransactionStatus = TransactionStatus.Completed;
            UpdateBankTransaction(bankTransaction);

            // Process all pending payment
            ProcessPayment(bankTransaction);

            //if (bankTransaction.FamilyMember.IsUnSubscribed)
            //    return;

            //var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.BankTransferCompleted);
            //var bodyContent = emailTemplate?.Content ?? "Transaction completed from busykid ${{amount}} on {{createdOn}}";

            //bodyContent = PrepareTemplateValues(bodyContent, bankTransaction);
            //await _emailService.Send(bankTransaction.FamilyMember.User.Email, emailTemplate.Subject, bodyContent);
        }

        /// <summary>
        /// Marks the transaction as settled.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        public void MarkAsSettled(MessageEvent messageEvent)
        {
            var bankTransaction = GetBankTransaction(messageEvent.TransactionId.ToString());
            if (bankTransaction == null || bankTransaction.TransactionStatus == TransactionStatus.Settled) return;
            bankTransaction.TransactionStatus = TransactionStatus.Settled;
            UpdateBankTransaction(bankTransaction);
        }

        /// <summary>
        /// Marks the transaction as failed.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        public void MarkAsFalied(MessageEvent messageEvent)
        {
            var bankTransaction = GetBankTransaction(messageEvent.TransactionId.ToString());
            if (bankTransaction == null || bankTransaction.TransactionStatus == TransactionStatus.Failed) return;
            bankTransaction.TransactionStatus = TransactionStatus.Failed;
            UpdateBankTransaction(bankTransaction);

            //if (bankTransaction.FamilyMember.IsUnSubscribed)
            //    return;

            //var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.BankTransferFailed);
            //var bodyContent = emailTemplate?.Content ?? "Transaction failed from busykid ${{amount}} on {{createdOn}}";

            //bodyContent = PrepareTemplateValues(bodyContent, bankTransaction);
            //await _emailService.Send(bankTransaction.FamilyMember.User.Email, emailTemplate.Subject, bodyContent);
        }

        /// <summary>
        /// Marks the transaction as cancelled.
        /// </summary>
        /// <param name="messageEvent">The message event received.</param>
        public void MarkAsCancelled(MessageEvent messageEvent)
        {
            var bankTransaction = GetBankTransaction(messageEvent.TransactionId.ToString());
            if (bankTransaction == null || bankTransaction.TransactionStatus == TransactionStatus.Cancelled) return;
            bankTransaction.TransactionStatus = TransactionStatus.Cancelled;
            UpdateBankTransaction(bankTransaction);

            //if (bankTransaction.FamilyMember.IsUnSubscribed)
            //    return;

            //var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.BankTransferCancelled);
            //var bodyContent = emailTemplate?.Content ?? "Transaction cancelled from busykid ${{amount}} on {{createdOn}}";

            //bodyContent = PrepareTemplateValues(bodyContent, bankTransaction);
            //await _emailService.Send(bankTransaction.FamilyMember.User.Email, emailTemplate.Subject, bodyContent);
        }
        public async Task TestEvent(MessageEvent messageEvent)
        {
            var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.BankTransferFailed);
            var bodyContent = emailTemplate?.Content ?? "Transaction failed from busykid ${{amount}} on {{createdOn}}";

            await _emailService.Send("", emailTemplate.Subject, bodyContent);
        }
        #endregion

    }
}
