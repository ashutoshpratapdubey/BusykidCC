using System.Collections.Generic;
using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Data.Repository;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using System.Linq;
using System;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using System.Threading.Tasks;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using System.Data.Entity;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Filters;

namespace LeapSpring.MJC.BusinessLogic.Services.Charities
{
    /// <summary>
    /// Represents a charity service.
    /// </summary>
    public class CharityService : ServiceBase, ICharityService
    {
        private ICurrentUserService _currentUserService;
        private ITransactionService _transactionService;
        private IEarningsService _earningsService;
        private IBankService _bankService;
        private ITextMessageService _textMessageService;
        private IAppSettingsService _appSettingsService;
        private ISMSApprovalHistory _smsApprovalHistory;
        private IFamilyService _familyService;

        public CharityService(IRepository repository, ICurrentUserService currentUserService, ITransactionService transactionService,
            IEarningsService earningsService, IBankService bankService, ITextMessageService textMessageService,
            IAppSettingsService appSettingsService, ISMSApprovalHistory smsApprovalHistory, IFamilyService familyService) : base(repository)
        {
            _currentUserService = currentUserService;
            _transactionService = transactionService;
            _earningsService = earningsService;
            _bankService = bankService;
            _textMessageService = textMessageService;
            _appSettingsService = appSettingsService;
            _smsApprovalHistory = smsApprovalHistory;
            _familyService = familyService;
        }

        /// <summary>
        /// Make a donation approval request to the parent
        /// </summary>
        /// <param name="donation">The donation</param>
        /// <returns>The donation</returns>
        public Donation Donate(Donation donation)
        {
            var canAllowTransaction = _earningsService.CanTransact(EarningsBucketType.Share, donation.Amount);
            if (donation.Amount == 0 || !canAllowTransaction)
                throw new InvalidOperationException($"Insufficient balance in share bucket!");

            donation.FamilyMemberID = _currentUserService.MemberID;
            donation.Date = DateTime.UtcNow;
            Repository.Insert(donation);

            // Updates child earnings
            var childEarnings = _earningsService.GetByMemberId(_currentUserService.MemberID);
            childEarnings.Share -= donation.Amount;
            Repository.Update(childEarnings);

            var child = _familyService.GetMemberById(donation.FamilyMemberID);
            var admin = _familyService.GetAdmin();
            var charity = Repository.Table<Charity>().SingleOrDefault(p => p.Id == donation.CharityID);

            var message = $"{child.Firstname.FirstCharToUpper()} has decided to donate ${donation.Amount:N2} to {charity.Name}."
                + $" Are you OK with transfering ${donation.Amount:N2} back into your account so you can make the donation? Reply YES or NO.";

            _smsApprovalHistory.Add(admin.Id, ApprovalType.CharityDonation, message, donation.Id);

            if (admin != null && !string.IsNullOrEmpty(admin.PhoneNumber))
                _textMessageService.Send(admin.PhoneNumber, message);
            return donation;
        }

        /// <summary>
        /// Approves the donation.
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <param name="donationId">The donation identifier.</param>
        /// <returns>The approved donation.</returns>
         public Donation ApproveDonation(FamilyMember adminMember, int donationId)
        {
            var donation = Repository.Table<Donation>()
                .Include(p => p.Charity)
                .Include(p => p.FamilyMember)
                .SingleOrDefault(p => p.Id == donationId);

            try
            {
                if (!_bankService.IsBankLinked(adminMember.Id))
                    throw new InvalidOperationException("Bank is not linked or verified!");

                // Tranfer amount to customer account
                var transactionResult =  _transactionService.Transfer(adminMember.Id, donation.Amount, PaymentType.Charity, TransferType.InternalToExternalAccount);

                // If transaction failure, continue to next family
                if (!transactionResult.HasValue)
                    throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                donation.ApprovalStatus = ApprovalStatus.Completed;
                donation.BankTransactionID = transactionResult;
                Repository.Update(donation);
            }
            catch (Exception ex)
            {
                _transactionService.SaveTransactionLog(adminMember.Id, ex.Message, donation.Amount);
                throw new InvalidOperationException(ex.Message); ;
            }
            return donation;
        }

        /// <summary>
        /// Disapproves the donation
        /// </summary>
        /// <param name="donationId">The donation identifier</param>
        /// <returns></returns>
        public void DisapproveDonation(int donationId)
        {
            var donation = Repository.Table<Donation>().SingleOrDefault(p => p.Id == donationId);
            if (donation == null)
                throw new ObjectNotFoundException("No donation request found!");

            // Updates child earnings
            var childEarnings = _earningsService.GetByMemberId(donation.FamilyMemberID);
            childEarnings.Share += donation.Amount;
            Repository.Update(childEarnings);

            donation.ApprovalStatus = ApprovalStatus.Rejected;
            Repository.Update(donation);
        }

        /// <summary>
        /// Gets the list of charities.
        /// </summary>
        /// <returns>The charities list.</returns>
        public IList<Charity> GetCharities()
        {
            return Repository.Table<Charity>().ToList();
        }
    }
}
