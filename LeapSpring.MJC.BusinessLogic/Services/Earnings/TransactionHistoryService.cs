using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LeapSpring.MJC.Core;


namespace LeapSpring.MJC.BusinessLogic.Services.Earnings
{
    public class TransactionHistoryService : ServiceBase, ITransactionHistoryService
    {
        #region Fields

        private readonly ICurrentUserService _currentUserService;

        private readonly IEarningsService _earningService;

        private List<TransactionHistory> _transactionHistories;

        #endregion

        #region Ctor

        public TransactionHistoryService(IRepository repository, ICurrentUserService currentUserService, IEarningsService earningServices) : base(repository)
        {
            _currentUserService = currentUserService;
            _earningService = earningServices;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets all transactions done by a child.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The list of transactions grouped by date. </returns>
        public Dictionary<DateTime, List<TransactionHistory>> GetAllTransactions(int? familyMemberId = null)
        {
            _transactionHistories = new List<TransactionHistory>();

            GetChoreTransactions(familyMemberId);
            GetBonusTransactions(familyMemberId);
            GetStockTransactions(familyMemberId);
            GetCharityTransactions(familyMemberId);
            GetGiftCardTransactions(familyMemberId);
            GetCashOutTransactions(familyMemberId);

            //GetChoreStatus(familyMemberId);

            if (!_transactionHistories.Any())
                throw new ObjectNotFoundException("No transactions found");

            return _transactionHistories.OrderByDescending(p => p.Date).GroupBy(p => p.Date.Date).ToDictionary(p => p.Key, p => p.ToList());
        }


        public Dictionary<DateTime, List<TransactionHistory>> GetAllowanceIn(int? familyMemberId = null)
        {
            _transactionHistories = new List<TransactionHistory>();

            GetChoreTransactions(familyMemberId);
            GetBonusTransactions(familyMemberId);

            if (!_transactionHistories.Any())
                throw new ObjectNotFoundException("No transactions found");

            return _transactionHistories.OrderByDescending(p => p.Date).GroupBy(p => p.Date.Date).ToDictionary(p => p.Key, p => p.ToList());
        }

        /// <summary>
        /// Gets allowance out.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The list of transactions grouped by date. </returns>
        public Dictionary<DateTime, List<TransactionHistory>> GetAllowanceOut(int? familyMemberId = null)
        {
            _transactionHistories = new List<TransactionHistory>();

            GetStockTransactions(familyMemberId);
            GetCharityTransactions(familyMemberId);
            GetGiftCardTransactions(familyMemberId);
            GetCashOutTransactions(familyMemberId);

            if (!_transactionHistories.Any())
                throw new ObjectNotFoundException("No transactions found");

            return _transactionHistories.OrderByDescending(p => p.Date).GroupBy(p => p.Date.Date).ToDictionary(p => p.Key, p => p.ToList());
        }


        #region Private Methods

        /// <summary>
        /// Get all paid chores transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetChoreTransactions(int? familyMemberId = null)
        {
            bool chorePayDayFlag = false;
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var chores = Repository.Table<Chore>().Include(m => m.BankTransaction).Where(p => p.FamilyMemberID == memberId
                && (p.ChoreStatus == ChoreStatus.Completed || p.ChoreStatus == ChoreStatus.CompletedAndApproved || p.ChoreStatus == ChoreStatus.CompletedAndPaid ||
                p.ChoreStatus == ChoreStatus.DisApproved || p.ChoreStatus == ChoreStatus.DisapprovedAndPending));
            bool thirtyDaysFlag = false;
            if (chores.Any())
            {
                foreach (var chore in chores)
                {
                    // Get transaction date, if last transaction date is empty or get last transaction updated date
                    var choreStatus = (chore.ChoreStatus == ChoreStatus.Completed || chore.ChoreStatus == ChoreStatus.CompletedAndApproved
                        || (chore.ChoreStatus == ChoreStatus.CompletedAndPaid && chore.BankTransaction.TransactionStatus == TransactionStatus.Pending))
                        ? TransactionStatus.Pending : TransactionStatus.Completed;
                    thirtyDaysFlag = _earningService.ShowThirtyDaysLink(chore.Id);
                    chorePayDayFlag = _earningService.ShowApproveDisapprovelink(chore.Id);
                    _transactionHistories.Add(new TransactionHistory
                    {
                        ChoreID = chore.Id,
                        Name = chore.Name,
                        Amount = chore.Value,
                        Date = chore.CompletedOn.Value,
                        TransactionHistoryType = TransactionHistoryType.AllowanceIn,
                        TransactionStatus = choreStatus,
                        ChrStatus = chore.ChoreStatus,
                        choreApprovalFlag = thirtyDaysFlag,
                        chorePaydayApprovalFlag = chorePayDayFlag
                    });
                }
            }
            return _transactionHistories;
        }

        //Adding a function to get Chore Status

        /// <summary>
        /// Get all paid chores transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>





        /// <summary>
        /// Gets the bonus transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetBonusTransactions(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var childBonus = Repository.Table<ChildBonus>()
                .Include(p => p.BankTransaction)
                .Where(p => p.ChildID == memberId
                && p.BankTransactionID.HasValue
                && (p.BankTransaction.TransactionStatus == TransactionStatus.Completed || p.BankTransaction.TransactionStatus == TransactionStatus.Pending));
            if (childBonus.Any())
            {
                foreach (var bonus in childBonus)
                {
                    _transactionHistories.Add(new TransactionHistory
                    {
                        Name = (string.IsNullOrEmpty(bonus.Note)) ? "Bonus" : "Bonus for " + bonus.Note.ToLower(),
                        Amount = bonus.Amount,
                        Date = bonus.Date,
                        TransactionHistoryType = TransactionHistoryType.AllowanceIn,
                        TransactionStatus = bonus.BankTransaction.TransactionStatus
                    });
                }
            }
            return _transactionHistories;
        }




        /// <summary>
        /// Gets the purchased gift cards transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetGiftCardTransactions(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var giftCards = Repository.Table<PurchasedGiftCard>()
                .Where(p => p.FamilyMemberID == memberId && (p.Status == ApprovalStatus.Completed || p.Status == ApprovalStatus.PendingApproval));
            if (giftCards.Any())
            {
                foreach (var giftCard in giftCards)
                {
                    _transactionHistories.Add(new TransactionHistory
                    {
                        Name = giftCard.Name,
                        Amount = giftCard.Amount,
                        Date = giftCard.PurchasedOn,
                        TransactionHistoryType = TransactionHistoryType.AllowanceOut,
                        TransactionOutType = EarningsBucketType.Spend,
                        TransactionStatus = (giftCard.Status == ApprovalStatus.PendingApproval) ? TransactionStatus.Pending : TransactionStatus.Completed
                    });
                }
            }
            return _transactionHistories;
        }

        /// <summary>
        /// Gets the purchased gift card transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetCharityTransactions(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var donations = Repository.Table<Donation>().Include(p => p.Charity)
                .Include(p => p.BankTransaction)
                .Where(p => p.FamilyMemberID == memberId && p.ApprovalStatus == ApprovalStatus.Completed
                && p.BankTransactionID.HasValue
                && (p.BankTransaction.TransactionStatus == TransactionStatus.Completed || p.BankTransaction.TransactionStatus == TransactionStatus.Pending));
            if (donations.Any())
            {
                foreach (var donation in donations)
                {
                    _transactionHistories.Add(new TransactionHistory
                    {
                        Name = donation.Charity.Name,
                        Amount = donation.Amount,
                        Date = donation.Date,
                        TransactionHistoryType = TransactionHistoryType.AllowanceOut,
                        TransactionOutType = EarningsBucketType.Share,
                        TransactionStatus = donation.BankTransaction.TransactionStatus
                    });
                }
            }
            return _transactionHistories;
        }

        /// <summary>
        /// Gets the purchased stock gift card transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetStockTransactions(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var purchasedStocks = Repository.Table<StockPurchaseRequest>()
                .Include(p => p.StockItem)
                .Where(p => p.ChildID == memberId && (p.Status == ApprovalStatus.Completed || p.Status == ApprovalStatus.PendingApproval));
            if (purchasedStocks.Any())
            {
                foreach (var purchasedStock in purchasedStocks)
                {
                    _transactionHistories.Add(new TransactionHistory
                    {
                        Name = purchasedStock.StockItem.CompanyName,
                        Amount = decimal.Add(purchasedStock.Amount, purchasedStock.Fee),
                        Date = purchasedStock.DateCreated,
                        TransactionHistoryType = TransactionHistoryType.AllowanceOut,
                        TransactionOutType = EarningsBucketType.Save,
                        TransactionStatus = (purchasedStock.Status == ApprovalStatus.PendingApproval) ? TransactionStatus.Pending : TransactionStatus.Completed
                    });
                }
            }
            return _transactionHistories;
        }

        /// <summary>
        /// Gets the cash out transaction lists.
        /// </summary>
        /// <returns>The transaction history list.</returns>
        private List<TransactionHistory> GetCashOutTransactions(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var cashoutTransactions = Repository.Table<CashOut>()
                .Include(p => p.Child)
                .Include(p => p.BankTransaction)
                .Where(p => p.ChildID == memberId
                && p.ApprovalStatus == ApprovalStatus.Completed
                && p.BankTransactionID.HasValue
                && (p.BankTransaction.TransactionStatus == TransactionStatus.Completed || p.BankTransaction.TransactionStatus == TransactionStatus.Pending));
            if (cashoutTransactions.Any())
            {
                foreach (var cashoutTransaction in cashoutTransactions)
                {
                    _transactionHistories.Add(new TransactionHistory
                    {
                        Name = (string.IsNullOrEmpty(cashoutTransaction.Note)) ? "Cashout" : "Cashout for " + cashoutTransaction.Note.ToLower(),
                        Amount = cashoutTransaction.Amount,
                        Date = cashoutTransaction.Date,
                        TransactionHistoryType = TransactionHistoryType.AllowanceOut,
                        TransactionOutType = EarningsBucketType.Spend,
                        TransactionStatus = cashoutTransaction.BankTransaction.TransactionStatus
                    });
                }
            }
            return _transactionHistories;
        }





        #endregion

        #endregion
    }
}
