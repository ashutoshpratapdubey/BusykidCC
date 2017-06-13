using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.SubscriptionService
{
    public class SubscriptionService : ServiceBase, ISubscriptionService
    {
        #region Fields

        private ICurrentUserService _currentUserService;
        private ITransactionService _transactionService;
        private IBankAuthorizeService _bankAuthorizeService;
        private IEmailTemplateService _emailTemplateService;
        private IEmailService _emailService;
        private IFamilyService _familyService;
        private IEarningsService _earningsService;
        private IEmailHistoryService _emailHistoryService;
        private IBankService _bankService;

        #endregion

        #region Ctor

        /// <summary>
        /// Subscription Service
        /// </summary>
        /// <param name="repository">The repository</param>
        /// <param name="currentUserService">The current user service</param>
        /// <param name="transactionService">The transaction service</param>
        /// <param name="bankAuthorizeService">The bank authorization service</param>
        /// <param name="emailTemplateService">The email template service</param>
        /// <param name="emailService">The email service</param>
        public SubscriptionService(IRepository repository, ICurrentUserService currentUserService, ITransactionService transactionService,
            IBankAuthorizeService bankAuthorizeService, IEmailTemplateService emailTemplateService, IEmailService emailService,
            IFamilyService familyService, IEarningsService earningsService, IEmailHistoryService emailHistoryService, IBankService bankService) : base(repository)
        {
            _currentUserService = currentUserService;
            _transactionService = transactionService;
            _bankAuthorizeService = bankAuthorizeService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _familyService = familyService;
            _earningsService = earningsService;
            _emailHistoryService = emailHistoryService;
            _bankService = bankService;
        }

        #endregion

        /// <summary>
        /// Gets the family subscription bt id.
        /// </summary>
        /// <returns>The family subscription.</returns>
        public FamilySubscription GetById()
        {
            return _familyService.GetFamilySubscription();
        }

        /// <summary>
        /// Gets the subscription status of the family.
        /// </summary>
        /// <returns><c>The subscription status.</returns>
        public SubscriptionStatus GetSubscriptionStatus()
        {
            // Gets the familu subscription
            var familySubscription = _familyService.GetFamilySubscription();
            if (familySubscription == null) return SubscriptionStatus.NoSubscription;
            return familySubscription.Status;
        }

        /// <summary>
        /// Subscribes the family
        /// </summary>
        /// <param name="subscription">The subscription</param>
        /// <returns></returns>
        public void Subscribe(Subscription subscription)
        {
            SubscriptionPromoCode subscriptionPromoCode = null;
            var familySubscription = _familyService.GetFamilySubscription();
            //var subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
            var subscriptionPlan = new SubscriptionPlan();

            //added by promocode
            var prePromoCodeStatus = _familyService.GetPrePromoCodeStatus();

            if (!string.IsNullOrEmpty(prePromoCodeStatus.PromoCode))
            {
                subscription.SubscriptionType = SubscriptionType.PromoPlan;
                subscriptionPromoCode = ValidatePromoCode(prePromoCodeStatus.PromoCode);
                subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
            }
            else
            {

                subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
            }

            if (familySubscription != null)
            {
                switch (familySubscription.Status)
                {
                    case SubscriptionStatus.Active:
                        throw new InvalidOperationException(string.Format("You already have an active {0} subscription.", familySubscription.SubscriptionPlan.PlanName.ToLower()));
                    case SubscriptionStatus.PendingCancellation:
                    case SubscriptionStatus.Cancelled:
                        if (familySubscription.EndsOn.Date <= DateTime.UtcNow.Date)
                        {
                            subscriptionPlan = GetSubscriptionPlan(SubscriptionType.Annual);
                            familySubscription.BankTransactionID = PurchaseAnnualSubscription(subscriptionPlan.Price);
                            familySubscription.SubscriptionPlanID = subscriptionPlan.Id;
                            familySubscription.StartsOn = DateTime.UtcNow;
                            familySubscription.EndsOn = DateTime.UtcNow.AddYears(1);
                            familySubscription.Status = SubscriptionStatus.Active;
                            familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
                            Repository.Update(familySubscription);
                        }
                        else
                        {
                            if (familySubscription.SubscriptionPlanID == subscriptionPlan.Id)
                            {
                                familySubscription.Status = SubscriptionStatus.Active;
                                familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
                                Repository.Update(familySubscription);
                            }
                            else
                            {
                                ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, familySubscription, subscriptionPromoCode);
                            }
                        }

                        // Sets IsActive of the subscription cancellation request to false.
                        var admin = _familyService.GetAdmin();
                        if (admin == null)
                            return;

                        var subscriptionCancellationRequest = Repository.Table<SubscriptionCancellationRequest>()
                            .FirstOrDefault(p => p.FamilyMemberID == admin.Id && p.IsActive);
                        if (subscriptionCancellationRequest == null)
                            return;
                        subscriptionCancellationRequest.IsActive = false;
                        Repository.Update(subscriptionCancellationRequest);
                        break;
                }
            }
            else
            {
                //Handle null Promocode 
                //subscriptionPromoCode = ValidatePromoCode(subscription.PromoCode);
                ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, null, subscriptionPromoCode);
            }
        }


        //Done by STPL

        //public string AuthorizeSubscribePlan(Subscription subscription)
        //{
        //    SubscriptionPromoCode subscriptionPromoCode = null;
        //    var familySubscription = _familyService.GetFamilySubscription();
        //    //var subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
        //    var subscriptionPlan = new SubscriptionPlan();

        //    //added by promocode
        //    var prePromoCodeStatus = _familyService.GetPrePromoCodeStatus();

        //    if (!string.IsNullOrEmpty(prePromoCodeStatus.PromoCode))
        //    {
        //        subscription.SubscriptionType = SubscriptionType.PromoPlan;
        //        subscriptionPromoCode = ValidatePromoCode(prePromoCodeStatus.PromoCode);
        //        subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
        //    }
        //    else
        //    {
        //        subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);
        //    }

        //    if (familySubscription != null)
        //    {
        //        switch (familySubscription.Status)
        //        {
        //            case SubscriptionStatus.Active:
        //                throw new InvalidOperationException(string.Format("You already have an active {0} subscription.", familySubscription.SubscriptionPlan.PlanName.ToLower()));
        //            case SubscriptionStatus.PendingCancellation:
        //            case SubscriptionStatus.Cancelled:
        //                if (familySubscription.EndsOn.Date <= DateTime.UtcNow.Date)
        //                {
        //                    subscriptionPlan = GetSubscriptionPlan(SubscriptionType.Annual);
        //                    familySubscription.BankTransactionID = PurchaseAnnualSubscription(subscriptionPlan.Price);
        //                    familySubscription.SubscriptionPlanID = subscriptionPlan.Id;
        //                    familySubscription.StartsOn = DateTime.UtcNow;
        //                    familySubscription.EndsOn = DateTime.UtcNow.AddYears(1);
        //                    familySubscription.Status = SubscriptionStatus.Active;
        //                    familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
        //                    Repository.Update(familySubscription);
        //                }
        //                else
        //                {
        //                    if (familySubscription.SubscriptionPlanID == subscriptionPlan.Id)
        //                    {
        //                        familySubscription.Status = SubscriptionStatus.Active;
        //                        familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
        //                        Repository.Update(familySubscription);
        //                    }
        //                    else
        //                    {
        //                        ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, familySubscription, subscriptionPromoCode);
        //                    }
        //                }

        //                // Sets IsActive of the subscription cancellation request to false.
        //                var admin = _familyService.GetAdmin();
        //                if (admin == null)
        //                    return "";

        //                var subscriptionCancellationRequest = Repository.Table<SubscriptionCancellationRequest>()
        //                    .FirstOrDefault(p => p.FamilyMemberID == admin.Id && p.IsActive);
        //                if (subscriptionCancellationRequest == null)
        //                    return "";
        //                subscriptionCancellationRequest.IsActive = false;
        //                Repository.Update(subscriptionCancellationRequest);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, null, subscriptionPromoCode);
        //    }

        //    string strPromoCode = prePromoCodeStatus.PromoCode.ToString();
        //    return strPromoCode;
        //}
        /// <summary>
        /// Renew the subscription if expaires.
        /// </summary>
        /// <returns></returns>
        public async Task RenewSubscription()
        {
            var todayDate = DateTime.UtcNow.Date;
            var familySubscriptions = Repository.Table<Family>()
                .Include(p => p.FamilySubscription)
                .Where(p => p.FamilySubscription.Status == SubscriptionStatus.Active && DbFunctions.TruncateTime(p.FamilySubscription.EndsOn) <= todayDate).ToList();

            var subscriptionPlan = GetSubscriptionPlan(SubscriptionType.Annual);

            foreach (var familySubscription in familySubscriptions)
            {
                try
                {
                    familySubscription.FamilySubscription.BankTransactionID = PurchaseAnnualSubscription(subscriptionPlan.Price, familySubscription.Id);
                    familySubscription.FamilySubscription.Status = SubscriptionStatus.Active;
                }
                catch
                {
                    familySubscription.FamilySubscription.Status = SubscriptionStatus.Cancelled;
                }

                if (subscriptionPlan.Id != familySubscription.FamilySubscription.SubscriptionPlanID)
                    familySubscription.FamilySubscription.SubscriptionPlanID = subscriptionPlan.Id;

                familySubscription.FamilySubscription.StartsOn = DateTime.UtcNow;
                familySubscription.FamilySubscription.EndsOn = DateTime.UtcNow.AddYears(1);
                Repository.Update(familySubscription);

                if (familySubscription.FamilySubscription.Status == SubscriptionStatus.Cancelled)
                    continue;

                var adminMember = _familyService.GetAdmin(familySubscription.Id);
                if (adminMember == null)
                    continue;

                if (!adminMember.IsUnSubscribed)
                {
                    try
                    {
                        var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.OneYearRenewal);
                        var bodyContent = emailTemplate?.Content ?? "Subscription Renewal";
                        await _emailService.Send(adminMember.User.Email, emailTemplate.Subject, bodyContent);

                        _emailHistoryService.SaveEmailHistory(adminMember.Id, EmailType.OneYearRenewal);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Cancells the subscription.
        /// </summary>
        public async Task CancelSubscription()
        {
            var familySubscription = _familyService.GetFamilySubscription(_currentUserService.FamilyID);
            if (familySubscription == null)
                throw new ObjectNotFoundException("No family subscription found!");

            var totalAmountInBuckets = _earningsService.GetTotalEarningsByFamily(_currentUserService.FamilyID);
            if (totalAmountInBuckets > 0)
            {
                try
                {
                    if (!_bankService.IsBankLinked(_currentUserService.MemberID))
                        throw new InvalidOperationException("Bank is not linked or verified!");

                    // Tranfer amount to customer account
                    var transactionResult = _transactionService.Transfer(_currentUserService.MemberID, totalAmountInBuckets, PaymentType.SubscriptionCancellation, TransferType.InternalToExternalAccount);

                    // If transaction failure
                    if (!transactionResult.HasValue)
                        throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                    var subscriptionCancellationRequest = new SubscriptionCancellationRequest
                    {
                        FamilyMemberID = _currentUserService.MemberID,
                        RequestedOn = DateTime.UtcNow,
                        BankTransactionID = transactionResult,
                        IsActive = true
                    };
                    Repository.Insert(subscriptionCancellationRequest);

                    familySubscription.Status = SubscriptionStatus.PendingCancellation;
                    Repository.Update(familySubscription);
                    _earningsService.ResetChildEarningsByFamily(_currentUserService.FamilyID);
                }
                catch (Exception ex)
                {
                    _transactionService.SaveTransactionLog(_currentUserService.MemberID, ex.Message, totalAmountInBuckets);
                    throw new InvalidOperationException(ex.Message);
                }
            }
            else
            {
                //await _bankService.RemoveBank();
                familySubscription.Status = SubscriptionStatus.Cancelled;
                Repository.Update(familySubscription);
            }

            var adminMember = _familyService.GetAdmin();
            if (adminMember == null)
                throw new ObjectNotFoundException("No family member found!");

            if (!adminMember.IsUnSubscribed)
            {
                var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.AccountCancellation);
                var bodyContent = emailTemplate?.Content ?? "Account Cancelled";
                await _emailService.Send(adminMember.User.Email, emailTemplate.Subject, bodyContent);
                _emailHistoryService.SaveEmailHistory(adminMember.Id, EmailType.AccountCancellation);
            }
        }

        /// <summary>
        /// Validates the promo code.
        /// </summary>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>The promo code subscription plan.</returns>
        public SubscriptionPromoCode ValidatePromoCode(string promoCode)
        {
            var subscriptionPromoCode = Repository.Table<SubscriptionPromoCode>().SingleOrDefault(p => p.PromoCode == promoCode && p.IsActive);
            if (subscriptionPromoCode == null)
                throw new ObjectNotFoundException("Invalid promo code!");

            return subscriptionPromoCode;
        }


        /// <summary>
        /// Subscribes the Pending microtransaction account family
        /// </summary>
        /// <param name="subscriptionMicroTransaction">The subscription Micro Transaction</param>
        /// <returns></returns>
        public void SubscribeMicroTransaction(Subscription subscription)
        {
            SubscriptionPromoCode subscriptionPromoCode = null;
            var familySubscription = _familyService.GetFamilySubscription();
            var subscriptionPlan = GetSubscriptionPlan(subscription.SubscriptionType);

            if (!string.IsNullOrEmpty(subscription.PromoCode))
                subscriptionPromoCode = ValidatePromoCode(subscription.PromoCode);

            if (familySubscription != null)
            {
                switch (familySubscription.Status)
                {
                    case SubscriptionStatus.Active:
                        throw new InvalidOperationException(string.Format("You already have an active {0} subscription.", familySubscription.SubscriptionPlan.PlanName.ToLower()));
                    case SubscriptionStatus.PendingCancellation:
                    case SubscriptionStatus.Cancelled:
                        if (familySubscription.EndsOn.Date <= DateTime.UtcNow.Date)
                        {
                            subscriptionPlan = GetSubscriptionPlan(SubscriptionType.Annual);
                            familySubscription.BankTransactionID = PurchaseAnnualSubscription(subscriptionPlan.Price);
                            familySubscription.SubscriptionPlanID = subscriptionPlan.Id;
                            familySubscription.StartsOn = DateTime.UtcNow;
                            familySubscription.EndsOn = DateTime.UtcNow.AddYears(1);
                            familySubscription.Status = SubscriptionStatus.Active;
                            familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
                            Repository.Update(familySubscription);
                        }
                        else
                        {
                            if (familySubscription.SubscriptionPlanID == subscriptionPlan.Id)
                            {
                                familySubscription.Status = SubscriptionStatus.Active;
                                familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;
                                Repository.Update(familySubscription);
                            }
                            else
                            {
                                ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, familySubscription, subscriptionPromoCode);
                            }
                        }

                        // Sets IsActive of the subscription cancellation request to false.
                        var admin = _familyService.GetAdmin();
                        if (admin == null)
                            return;

                        var subscriptionCancellationRequest = Repository.Table<SubscriptionCancellationRequest>()
                            .FirstOrDefault(p => p.FamilyMemberID == admin.Id && p.IsActive);
                        if (subscriptionCancellationRequest == null)
                            return;
                        subscriptionCancellationRequest.IsActive = false;
                        Repository.Update(subscriptionCancellationRequest);
                        break;
                }
            }
            else
            {
                ActivateSubscription(subscriptionPlan, subscription.SubscriptionType, null, subscriptionPromoCode);
            }
        }


        #region Private Methods

        /// <summary>
        /// gets the subscription plan by subscription type.
        /// </summary>
        /// <param name="subscriptionType">The subscription type</param>
        /// <returns></returns>
        private SubscriptionPlan GetSubscriptionPlan(SubscriptionType subscriptionType)
        {
            // Gets the description of subscription type
            var subscriptionTypeDescription = subscriptionType.GetEnumDescriptionValue();

            // Gets the subscription plan
            var subscriptionPlan = Repository.Table<SubscriptionPlan>().SingleOrDefault(p => p.PlanName.ToLower() == subscriptionTypeDescription);
            if (subscriptionPlan == null)
                throw new ObjectNotFoundException("No subscription plan found!");
            return subscriptionPlan;
        }

        /// <summary>
        /// Activates the subscription plan
        /// </summary>
        /// <param name="subscriptionPlanId">The subscription plan identifier.</param>
        /// <param name="subscriptionType">The subscription type.</param>
        /// <param name="familySubscription">The family subscription.</param>
        /// <param name="subscriptionPromoCode">The subscription promo code.</param>
        /// <returns>The family subscription.</returns>
        private FamilySubscription ActivateSubscription(SubscriptionPlan subscriptionPlan, SubscriptionType subscriptionType, FamilySubscription familySubscription, SubscriptionPromoCode subscriptionPromoCode)
        {
            familySubscription = familySubscription ?? new FamilySubscription();
            var subscriptionEndDate = DateTime.UtcNow.AddDays(-1);
            switch (subscriptionType)
            {
                case SubscriptionType.OneMonthTrial:
                    subscriptionEndDate = DateTime.UtcNow.AddMonths(1);
                    familySubscription.IsTrialUsed = true;
                    familySubscription.TrialStartDate = DateTime.UtcNow;
                    break;
                case SubscriptionType.PromoPlan:
                    switch (subscriptionPromoCode.DurationType)
                    {
                        case DurationType.Day:
                            subscriptionEndDate = DateTime.UtcNow.AddDays(subscriptionPromoCode.Duration);
                            break;
                        case DurationType.Month:
                            subscriptionEndDate = DateTime.UtcNow.AddMonths(subscriptionPromoCode.Duration);
                            break;
                        case DurationType.Year:
                            subscriptionEndDate = DateTime.UtcNow.AddYears(subscriptionPromoCode.Duration);
                            break;
                    }
                    break;
                case SubscriptionType.Annual:
                    familySubscription.BankTransactionID = PurchaseAnnualSubscription(subscriptionPlan.Price);
                    subscriptionEndDate = DateTime.UtcNow.AddYears(1);
                    break;
                case SubscriptionType.Pendingaccount:
                    subscriptionEndDate = DateTime.UtcNow.AddMonths(1);
                    familySubscription.IsTrialUsed = true;
                    familySubscription.TrialStartDate = DateTime.UtcNow;
                    break;
            }
            familySubscription.SubscriptionPlanID = subscriptionPlan.Id;
            familySubscription.StartsOn = DateTime.UtcNow;
            familySubscription.EndsOn = subscriptionEndDate;
            familySubscription.Status = SubscriptionStatus.Active;
            familySubscription.PromoCode = subscriptionPromoCode?.PromoCode ?? string.Empty;

            return _familyService.UpdatetFamilySubscription(familySubscription);
        }

        /// <summary>
        /// Purchase the annual subscription
        /// </summary>
        /// <returns>The bank transaction identifier.</returns>
        private int PurchaseAnnualSubscription(decimal price, int? familyId = null)
        {
            var currentFamilyId = familyId ?? _currentUserService.FamilyID;
            var adminMember = _familyService.GetAdmin(currentFamilyId);
            try
            {
                if (!_bankService.IsBankLinked(adminMember.Id))
                    throw new InvalidOperationException("Bank is not linked or verified!");

                // Tranfer amount from customer's external account to internal account
                var transactionResult = _transactionService.Transfer(adminMember.Id, price, PaymentType.SubscriptionInitiated, TransferType.ExternalToInetrnalAccount);

                // If transaction failure
                if (!transactionResult.HasValue)
                    throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                return transactionResult.Value;
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, price);
                throw new InvalidOperationException(ex.Message);
            }
        }


        #endregion
    }
}
