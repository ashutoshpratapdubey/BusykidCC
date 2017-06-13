using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Dto.Spend;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using System.Data.Entity;
using LeapSpring.MJC.BusinessLogic.Services.Settings;

namespace LeapSpring.MJC.BusinessLogic.Services.Spend
{
    /// <summary>
    /// Represents a spend service
    /// </summary>
    public class SpendService : ServiceBase, ISpendService
    {
        private IGyftService _gyftService;
        private ICurrentUserService _currentUserService;
        private IFamilyService _familyService;
        private IEarningsService _earningsService;
        private ITextMessageService _textMessageService;
        private ITransactionService _transactionService;
        private IBankService _bankService;
        private ISMSApprovalHistory _smsApprovalHistory;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public SpendService(IRepository repository, IGyftService gyftService, ICurrentUserService currentUserService,
            IFamilyService familyService, IEarningsService earningsService, ITextMessageService textMessageService,
            ITransactionService transactionService, IBankService bankService, ISMSApprovalHistory smsApprovalHistory) : base(repository)
        {
            _gyftService = gyftService;
            _currentUserService = currentUserService;
            _familyService = familyService;
            _earningsService = earningsService;
            _textMessageService = textMessageService;
            _transactionService = transactionService;
            _bankService = bankService;
            _smsApprovalHistory = smsApprovalHistory;
        }

        #region Methods

        /// <summary>
        /// Add purchased gift card
        /// </summary>
        /// <param name="purchasedGiftCard">Purchased gift card</param>
        public void AddPurchasedGiftCard(PurchasedGiftCard purchasedGiftCard)
        {
            Repository.Insert(purchasedGiftCard);
        }

        /// <summary>
        /// Update purchased gift card
        /// </summary>
        /// <param name="purchasedGiftCard">Purchased gift card</param>
        public void UpdatePurchasedGiftCard(PurchasedGiftCard purchasedGiftCard)
        {
            Repository.Update(purchasedGiftCard);
        }

        /// <summary>
        /// Get purchased gift card by identifier
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card by identifier</param>
        /// <returns>Purchased gift card</returns>
        public PurchasedGiftCard GetPurchasedGiftCardById(int purchasedGiftCardId)
        {
            var purchasedGiftCard = Repository.Table<PurchasedGiftCard>().SingleOrDefault(m => m.Id == purchasedGiftCardId && !m.IsDeleted);
            if (purchasedGiftCard == null)
                throw new ObjectNotFoundException("Purchased gift card not found");

            return purchasedGiftCard;
        }

        /// <summary>
        /// Delete purchased gift card by identifier
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card by identifier</param>
        public void DeletePurchasedGiftCard(int purchasedGiftCardId)
        {
            var purchasedGiftCard = GetPurchasedGiftCardById(purchasedGiftCardId);
            purchasedGiftCard.IsDeleted = true;

            Repository.Update(purchasedGiftCard);
        }

        /// <summary>
        /// Get purchased gift cards 
        /// </summary>
        /// <returns>Purchased gift cards</returns>
        public IList<PurchasedGiftCard> GetPurchasedGiftCards()
        {
            return Repository.Table<PurchasedGiftCard>().OrderByDescending(m => m.PurchasedOn).Where(m => m.FamilyMemberID == _currentUserService.MemberID && !m.IsDeleted).ToList();
        }

        /// <summary>
        /// Get gift cards
        /// </summary>
        /// <returns>Gift cards</returns>
        public IList<GiftCard> GetGiftCards()
        {
            return Repository.Table<GiftCard>().ToList();
        }

        /// <summary>
        /// Get gift cards grouped by merchant name
        /// </summary>
        /// <param name="isFeatured">Is featured</param>
        /// <returns>Gift cards</returns>
        public IList<GiftCardPreview> GetGiftCardPreviews(bool isFeatured)
        {
            var giftCardPreviews = new List<GiftCardPreview>();
            var groupedGiftCards = Repository.Table<GiftCard>().GroupBy(m => m.MerchantName);
            if (isFeatured)
                groupedGiftCards = groupedGiftCards.Where(m => m.Any(g => g.IsFeatured));

            foreach (var groupedGift in groupedGiftCards.ToList())
            {
                // Todo: Remove html p and b tag from DB.
                foreach (var giftCard in groupedGift)
                    giftCard.Description = giftCard.Description?.Replace("<p>", "").Replace("</p>", "").Replace("<b>", "").Replace("</b>", "");

                var firstGiftCard = groupedGift.FirstOrDefault();
                var giftCardPreview = new GiftCardPreview
                {
                    Name = firstGiftCard.GiftCardName ?? firstGiftCard.MerchantName,
                    Description = firstGiftCard.Description,
                    Disclosure = firstGiftCard.Disclosure,
                    GiftCards = groupedGift.OrderBy(m => m.Amount).ToList(),
                    IsFeatured = groupedGift.Any(m => m.IsFeatured),
                    MinAmount = groupedGift.Min(m => m.Amount),
                    MaxAmount = groupedGift.Max(m => m.Amount),
                    CoverImageUrl = firstGiftCard.MerchantIconImageUrl.Replace("http://", "https://")
                };

                giftCardPreviews.Add(giftCardPreview);
            }

            return giftCardPreviews;
        }

        /// <summary>
        /// Purchase gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Purchased gift card</returns> 
        public PurchasedGiftCard PurchaseGiftCardRequest(int giftCardId)
        {
            var giftCard = Repository.Table<GiftCard>().FirstOrDefault(m => m.CardId == giftCardId);
            if (giftCard == null) throw new InvalidParameterException("Gift card not found!");

            var canTransact = _earningsService.CanTransact(EarningsBucketType.Spend, giftCard.Amount);
            if (giftCard.Amount == 0 || !canTransact) throw new InvalidParameterException("Insufficient balance in spend bucket!");

            var familyMember = _familyService.GetMember();
            var adminMember = _familyService.GetAdmin();

            var purchasedGiftCard = new PurchasedGiftCard
            {
                FamilyMemberID = familyMember.Id,
                CardId = giftCard.CardId,
                Name = giftCard.GiftCardName ?? giftCard.MerchantName,
                Amount = giftCard.Amount,
                CoverImageUrl = giftCard.MerchantIconImageUrl,
                PurchasedOn = DateTime.UtcNow,
                Status = ApprovalStatus.PendingApproval
            };
            AddPurchasedGiftCard(purchasedGiftCard);

            // deduct spend amount of corresponding child from child earnings
            var childEarnings = _earningsService.GetByMemberId(familyMember.Id);
            childEarnings.Spend -= giftCard.Amount;
            _earningsService.Update(childEarnings);

            // Send spent info to parent through SMS
            var message = $"{familyMember.Firstname.FirstCharToUpper()} wants to purchase a ${purchasedGiftCard.Amount:N2} {purchasedGiftCard.Name} gift card. Do you approve? Reply YES or NO.";
            _smsApprovalHistory.Add(adminMember.Id, ApprovalType.GiftPurchase, message, purchasedGiftCard.Id);

            if (adminMember != null && !string.IsNullOrEmpty(adminMember.PhoneNumber))
                _textMessageService.Send(adminMember.PhoneNumber, message);
            return purchasedGiftCard;
        }

        /// <summary>
        /// Initiates the cash out request.
        /// </summary>
        /// <param name="cashOutRequest">The cashout request.</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The cashout.</returns>
        public CashOut CashOutRequest(CashOut cashOutRequest, int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            var canAllowTransaction = _earningsService.CanTransact(EarningsBucketType.Spend, cashOutRequest.Amount);
            if (cashOutRequest.Amount == 0 || !canAllowTransaction)
                throw new InvalidOperationException("Insufficient balance in spend bucket!");

            cashOutRequest.ChildID = memberId;
            cashOutRequest.Date = DateTime.UtcNow;

            Repository.Insert(cashOutRequest);

            // Add spend amount to corresponding child
            var childEarnings = _earningsService.GetByMemberId(memberId);
            childEarnings.Spend -= cashOutRequest.Amount;
            _earningsService.Update(childEarnings);

            var child = _familyService.GetMemberById(memberId);
            var admin = _familyService.GetAdmin(child.User.FamilyID);

            var childGenderNotation = child.Gender.Value == Gender.Male ? "him" : "her";
            var message = $"{child.Firstname.FirstCharToUpper()} wants to transfer ${cashOutRequest.Amount:N2} back into your account so you can give {childGenderNotation} cash instead. Do you approve? Reply YES or NO.";

            _smsApprovalHistory.Add(admin.Id, ApprovalType.CashOut, message, cashOutRequest.Id);

            if (admin != null && !string.IsNullOrEmpty(admin.PhoneNumber))
                _textMessageService.Send(admin.PhoneNumber, message);

            return cashOutRequest;
        }

        /// <summary>
        /// Approves the cash out request.
        /// </summary>
        /// <param name="adminMember">The admin member of the family.</param>
        /// <param name="pendingCashOutId">The pending cash out identifier.</param>
        /// <returns></returns>
        public void ApproveCashOut(FamilyMember adminMember, int pendingCashOutId)
        {
            var cashOut = Repository.Table<CashOut>().Include(p => p.Child).SingleOrDefault(p => p.Id == pendingCashOutId);
            try
            {
                if (!_bankService.IsBankLinked(adminMember.Id))
                    throw new InvalidOperationException("Bank is not linked or verified!");

                // Tranfer amount to customer account
                var transactionResult = _transactionService.Transfer(adminMember.Id, cashOut.Amount, PaymentType.CashOut, TransferType.InternalToExternalAccount);

                // If transaction failure, continue to next family
                if (!transactionResult.HasValue)
                    throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                // Update cashout
                cashOut.BankTransactionID = transactionResult;
                cashOut.ApprovalStatus = ApprovalStatus.Completed;
                Repository.Update(cashOut);
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, cashOut.Amount);
                throw ex;
            }
        }

        /// <summary>
        /// Disapproves the pending cash out.
        /// </summary>
        /// <param name="pendingCashOutId">The pending cash out identifier.</param>
        /// <returns></returns>
        public void DisapproveCashOut(int pendingCashOutId)
        {
            var cashOut = Repository.Table<CashOut>().SingleOrDefault(p => p.Id == pendingCashOutId);
            if (cashOut == null)
                throw new ObjectNotFoundException("No cash out request found!");

            // Updates child earnings
            var childEarnings = _earningsService.GetByMemberId(cashOut.ChildID);
            childEarnings.Spend += cashOut.Amount;
            Repository.Update(childEarnings);

            cashOut.ApprovalStatus = ApprovalStatus.Rejected;
            Repository.Update(cashOut);
        }

        /// <summary>
        /// Approve purchase gift card 
        /// </summary>
        /// <param name="adminMember">Admin member</param>
        /// <param name="purchasedGiftCardId">Purchased gift card identifier</param>
        /// <returns></returns>
        async public Task ApprovePurchaseGiftCard(FamilyMember adminMember, int purchasedGiftCardId)
        {
            var purchasedGiftCard = Repository.Table<PurchasedGiftCard>().Include(p => p.FamilyMember).SingleOrDefault(p => p.Id == purchasedGiftCardId);
            var giftPurchaseRequest = new GyftPurchaseRequest
            {
                ShopCardId = purchasedGiftCard.CardId,
                ToEmail = adminMember.User.Email,
                ResellerReference = "",
                Notes = "Child purchase",
                FirstName = purchasedGiftCard.FamilyMember.Firstname,
                LastName = purchasedGiftCard.FamilyMember.Lastname,
                Gender = purchasedGiftCard.FamilyMember.Gender?.ToString() ?? string.Empty,
                Birthday = purchasedGiftCard.FamilyMember.DateOfBirth?.ToString("dd/MM/yyyy") ?? string.Empty
            };

            try
            {
                if (!purchasedGiftCard.BankTransactionID.HasValue)
                {
                    if (!_bankService.IsBankLinked(adminMember.Id))
                        throw new InvalidOperationException("Bank is not linked or verified!");

                    // Tranfer amount from customer account to program account
                    var transactionResult = _transactionService.Transfer(adminMember.Id, purchasedGiftCard.Amount, PaymentType.GiftCard, TransferType.InternalToBusyKidInternalAccount);

                    if (!transactionResult.HasValue)
                        throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                    // Update purchased gift card
                    purchasedGiftCard.BankTransactionID = transactionResult;
                    UpdatePurchasedGiftCard(purchasedGiftCard);
                }

                // Purchase gift card from gyft api, then update status
                var giftCardUrl = await _gyftService.PurchaseGiftCard(giftPurchaseRequest);
                purchasedGiftCard.GiftCardUrl = giftCardUrl;
                purchasedGiftCard.Status = ApprovalStatus.Completed;
                UpdatePurchasedGiftCard(purchasedGiftCard);
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, purchasedGiftCard.Amount);
                throw ex;
            }
        }

        /// <summary>
        /// Disapproves the purchased gift card 
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card identifier</param>
        /// <returns></returns>
        public void DisapprovePurchasedGiftCard(int purchasedGiftCardId)
        {
            var giftCard = Repository.Table<PurchasedGiftCard>().SingleOrDefault(p => p.Id == purchasedGiftCardId);
            if (giftCard == null)
                throw new ObjectNotFoundException("No gift cards purchased!");

            // Updates child earnings
            var childEarnings = _earningsService.GetByMemberId(giftCard.FamilyMemberID);
            childEarnings.Spend += giftCard.Amount;
            _earningsService.Update(childEarnings);

            giftCard.Status = ApprovalStatus.Rejected;
            Repository.Update(giftCard);
        }

        #endregion
    }
}
