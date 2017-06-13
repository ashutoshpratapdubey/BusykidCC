using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Domain.Sms;
using LeapSpring.MJC.Core.Dto.Save;
using LeapSpring.MJC.Core.Dto.Save.StockPilePurchase;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Save
{
    public class SaveService : ServiceBase, ISaveService
    {
        private readonly string _institutionId = "BusyKid";
        private const decimal StockFee = 2.99M;

        private IStockPileService _stockPileService;
        private ICurrentUserService _currentUserService;
        private IFamilyService _familyService;
        private ITextMessageService _textMessageService;
        private IEarningsService _earningsService;
        private ISMSApprovalHistory _smsApprovalHistory;
        private ITransactionService _transactionService;
        private IBankService _bankService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="stockPileService"></param>
        /// <param name="currentUserService"></param>
        /// <param name="familyService"></param>
        /// <param name="textMessageService"></param>
        /// <param name="transactionService"></param>
        /// <param name="earningsService"></param>
        public SaveService(IRepository repository, IStockPileService stockPileService, ICurrentUserService currentUserService,
            IFamilyService familyService, ITextMessageService textMessageService, IEarningsService earningsService,
            ISMSApprovalHistory smsApprovalHistory, ITransactionService transactionService, IBankService bankService) : base(repository)
        {
            _stockPileService = stockPileService;
            _currentUserService = currentUserService;
            _familyService = familyService;
            _textMessageService = textMessageService;
            _earningsService = earningsService;
            _smsApprovalHistory = smsApprovalHistory;
            _transactionService = transactionService;
            _bankService = bankService;
        }

        /// <summary>
        /// Gets the stock gift card.
        /// </summary>
        /// <returns>The stock gift card.</returns>
        public StockItem GetById(int id)
        {
            // Gets stock items from stockpile
            var stockGiftCard = Repository.Table<StockItem>().FirstOrDefault(p => p.Id == id);
            if (stockGiftCard == null)
                throw new ObjectNotFoundException("No stock items found!");
            return stockGiftCard;
        }

        /// <summary>
        /// Gets the stock gift cards.
        /// </summary>
        /// <param name="isFeaturedStock">The is featured stock.</param>
        /// <returns>The list of stock gift cards.</returns>
        public IList<StockItem> GetStockGiftCards(bool isFeaturedStock)
        {
            var stockGiftCards = isFeaturedStock ? Repository.Table<StockItem>().Where(p => p.IsFeaturedStock).ToList() : Repository.Table<StockItem>().ToList();
            if (stockGiftCards == null)
                throw new ObjectNotFoundException("No stock items found!");
            return stockGiftCards;
        }

        /// <summary>
        /// Gets the purchased stock gift cards
        /// </summary>
        /// <returns>The purchased stocks</returns>
        public IQueryable<StockPurchaseRequest> GetPurchasedStockGiftCards()
        {
            return Repository.Table<StockPurchaseRequest>().Include(p => p.StockItem)
                .Where(p => p.ChildID == _currentUserService.MemberID && (p.Status == ApprovalStatus.PendingApproval || p.Status == ApprovalStatus.Completed))
                .OrderByDescending(p => p.DateCreated);
        }

        /// <summary>
        /// Gets the disapproved stock gift cards
        /// </summary>
        /// <returns>The disapproved stocks</returns>
        public IQueryable<StockPurchaseRequest> GetDisapprovedStockGiftCards()
        {
            var startDate = DateTime.Now.AddDays(-6).Date;
            var endDate = DateTime.Now.Date;
            return Repository.Table<StockPurchaseRequest>().Include(p => p.StockItem)
                .Where(p => p.ChildID == _currentUserService.MemberID && p.Status == ApprovalStatus.Rejected
                && (DbFunctions.TruncateTime(p.DateCreated) >= startDate && DbFunctions.TruncateTime(p.DateCreated) <= endDate))
                .OrderByDescending(p => p.DateCreated);
        }

        /// <summary>
        /// Gets and updates the stock quotes.
        /// </summary>
        /// <returns>The list of stock quotes.</returns>
        public async Task<IList<GiftStockQuote>> GetStockGiftQuotes()
        {
            var stockGiftQuotes = await _stockPileService.GetStockGiftQuotes();
            if (stockGiftQuotes == null)
                throw new ObjectNotFoundException("No stock quotes found!");
            return stockGiftQuotes;
        }

        /// <summary>
        /// Update the stock quotes.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateStockGiftQuotes()
        {
            var stockGiftQuotes = await GetStockGiftQuotes();
            if (stockGiftQuotes == null)
                throw new ObjectNotFoundException("No stock quotes found!");

            var stockGiftCards = Repository.Table<StockItem>().ToList();
            if (stockGiftCards == null)
                throw new ObjectNotFoundException("No stock items found!");

            // Assigning quote to the corresponding stock gift card
            foreach (var stockGiftCard in stockGiftCards)
            {
                var stockGiftQuote = stockGiftQuotes.SingleOrDefault(p => p.Symbol == stockGiftCard.StockSymbol);
                if (stockGiftQuote == null)
                    throw new ObjectNotFoundException("No quote found!");

                stockGiftCard.StockPrice = stockGiftQuote.ClosingPrice;
                stockGiftCard.StockPriceRetrievedAt = stockGiftQuote.RetrievedAt;
                Repository.Update(stockGiftCard);
            }
        }

        /// <summary>
        /// initiates the stock purchase.
        /// </summary>
        /// <param name="stockPurchaseRequest">The stock purchase request.</param>
        /// <returns>The stock purchase request.</returns>
        public StockPurchaseRequest InitiateStockPurchase(StockPurchaseRequest stockPurchaseRequest)
        {
            stockPurchaseRequest.Fee = StockFee;
            var canAllowTransaction = _earningsService.CanTransact(EarningsBucketType.Save, (stockPurchaseRequest.Amount + stockPurchaseRequest.Fee));
            if (stockPurchaseRequest.Amount == 0 || !canAllowTransaction)
                throw new InvalidOperationException("Insufficient balance in save bucket!");

            stockPurchaseRequest.LineItemID = Guid.NewGuid();
            stockPurchaseRequest.TransactionID = Guid.NewGuid();
            stockPurchaseRequest.DateCreated = DateTime.UtcNow;
            stockPurchaseRequest.ChildID = _currentUserService.MemberID;

            Repository.Insert(stockPurchaseRequest);

            // deduct save amount of corresponding child from child earnings
            var childEarnings = _earningsService.GetByMemberId(_currentUserService.MemberID);
            childEarnings.Save -= (stockPurchaseRequest.Amount + stockPurchaseRequest.Fee); // Deducting stock amount including Fee
            Repository.Update(childEarnings);

            var admin = _familyService.GetAdmin();
            var child = _familyService.GetMember();
            var stock = GetById(stockPurchaseRequest.StockItemID);
            var stockName = string.IsNullOrEmpty(stock.BrandName) ? stock.CompanyPopularName : stock.BrandName;

            var message = $"{child.Firstname.FirstCharToUpper()} would like to buy ${stockPurchaseRequest.Amount:N2} of {stockName} stock. Are you OK with this? Reply YES or NO.";
            _smsApprovalHistory.Add(admin.Id, ApprovalType.StockPurchase, message, stockPurchaseRequest.Id);

            if (admin != null && !string.IsNullOrEmpty(admin.PhoneNumber))
                _textMessageService.Send(admin.PhoneNumber, message);

            return stockPurchaseRequest;
        }

        /// <summary>
        /// Aproves the stock purchase.
        /// </summary>
        /// <param name="adminMember">Admin member</param>
        /// <param name="pendingStockRequestId">Pending stock request identifier.</param>
        /// <returns></returns>
        public async Task ApproveStockPurchase(FamilyMember adminMember, int pendingStockRequestId)
        {
            var pendingStockRequest = Repository.Table<StockPurchaseRequest>()
                .Include(p => p.Child)
                .Include(p => p.StockItem)
                .SingleOrDefault(p => p.Id == pendingStockRequestId);

            // Prepare stock purchase request data
            var purchaser = new Purchaser
            {
                Email = adminMember.User.Email,
                //Email = "gobinath.b@web3mavens.com",
                FirstName = adminMember.Firstname,
                LastName = adminMember.Lastname,
            };

            var recipient = new Purchaser
            {
                Email = adminMember.User.Email,
                //Email = "gobinath.b@web3mavens.com",
                FirstName = adminMember.Firstname,
                LastName = adminMember.Lastname,
            };

            var stockData = new PurchaseStockRequest
            {
                Preamble = new Preamble
                {
                    TransactionID = pendingStockRequest.TransactionID,
                    InstitutionID = _institutionId,
                    PartnerProgram = "RewardPointsToStock"
                },
                Purchaser = purchaser,
                PaymentDetails = new PaymentDetails
                {
                    PaymentType = "PREFUNDED"
                },
                PrepaidValueItemRequests = new List<PrepaidValueItemRequest>() {
                    new PrepaidValueItemRequest {
                        ItemCode = pendingStockRequest.StockItem.ItemCode ,
                        ItemType = "SECURITY",
                        DeliveryOption = "EMAIL",
                        CurrencyCode = "USD",
                        Value = pendingStockRequest.Amount,
                        LineItemID = pendingStockRequest.LineItemID,
                        Recipient = recipient
                    }
                }
            };

            try
            {
                if (!pendingStockRequest.BankTransactionID.HasValue)
                {
                    if (!_bankService.IsBankLinked(adminMember.Id))
                        throw new InvalidOperationException("Bank is not linked or verified!");

                    var stockAmount = pendingStockRequest.Amount + pendingStockRequest.Fee; // Transferring stock amount including Fee

                    // Tranfer amount from customer account to program account
                    var transactionResult = _transactionService.Transfer(adminMember.Id, stockAmount, PaymentType.StockPile, TransferType.InternalToBusyKidInternalAccount);

                    if (!transactionResult.HasValue)
                        throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                    // Update pending stock request
                    pendingStockRequest.BankTransactionID = transactionResult;
                    Repository.Update(pendingStockRequest);
                }

                // Purchase the stock.
                var stockPurchaseResponse = await _stockPileService.PurchaseStock(stockData);
                // Make order request
                var orderResponse = await _stockPileService.Order(stockPurchaseResponse.Preamble.TransactionID.ToString());

                // Update the stock purchase request
                pendingStockRequest.Status = ApprovalStatus.Completed;
                Repository.Update(pendingStockRequest);

                // send a custom stock purchase email
            }
            catch (Exception ex)
            {
                var stockAmount = pendingStockRequest.Amount + pendingStockRequest.Fee; // Transferring stock amount including Fee 
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, stockAmount);
                throw ex;
            }
        }

        /// <summary>
        /// Disapproves the purchased stock.
        /// </summary>
        /// <param name="purchasedStockId">The purchased stock identifier.</param>
        /// <returns></returns>
        public void DisapprovePurchasedStock(int purchasedStockId)
        {
            var purchasedStock = Repository.Table<StockPurchaseRequest>().Include(p => p.StockItem).SingleOrDefault(p => p.Id == purchasedStockId);
            if (purchasedStock == null)
                throw new ObjectNotFoundException("No stock purchase request found!");

            // Updates child earnings
            var childEarnings = _earningsService.GetByMemberId(purchasedStock.ChildID);
            childEarnings.Save += (purchasedStock.Amount + purchasedStock.Fee); // Adding stock amount including Fee
            Repository.Update(childEarnings);

            purchasedStock.Status = ApprovalStatus.Rejected;
            Repository.Update(purchasedStock);
        }
    }
}
