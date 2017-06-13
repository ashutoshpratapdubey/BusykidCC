using LeapSpring.MJC.BusinessLogic.Services.Charities;
using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Save;
using LeapSpring.MJC.BusinessLogic.Services.Spend;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Sms;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Sms
{
    public class SMSApprovalService : ServiceBase, ISMSApprovalService
    {
        private ISaveService _saveService;
        private ISpendService _spendService;
        private ICharityService _charityService;
        private IChoreService _choreService;
        private ISMSApprovalHistory _smsApprovalHistory;

        public SMSApprovalService(IRepository repository, ISaveService saveService, ISpendService spendService,
            ICharityService charityService, IChoreService choreService, ISMSApprovalHistory smsApprovalHistory) : base(repository)
        {
            _saveService = saveService;
            _spendService = spendService;
            _charityService = charityService;
            _choreService = choreService;
            _smsApprovalHistory = smsApprovalHistory;
        }

        /// <summary>
        /// Approves the request.
        /// </summary>
        /// <param name="adminMember">The admin member</param>
        /// <returns>The message</returns>
        public async Task<string> Approve(FamilyMember adminMember)
        {
            var responseMessage = string.Empty;
            var currentApprovalRequest = _smsApprovalHistory.GetRecentApprovalRequest(adminMember.Id);
            if (currentApprovalRequest == null)
                return string.Empty;

            switch (currentApprovalRequest.ApprovalType)
            {
                case ApprovalType.GiftPurchase:
                    if (currentApprovalRequest.PurchasedGiftCardID.HasValue)
                    {
                        await _spendService.ApprovePurchaseGiftCard(adminMember, currentApprovalRequest.PurchasedGiftCardID.Value);
                        responseMessage = "Ok, consider it done.";
                    }
                    break;
                case ApprovalType.StockPurchase:
                    if (currentApprovalRequest.StockPurchaseRequestID.HasValue)
                    {
                        await _saveService.ApproveStockPurchase(adminMember, currentApprovalRequest.StockPurchaseRequestID.Value);
                        responseMessage = "Ok, great. Check your inbox to redeem the stock!";
                    }
                    break;
                case ApprovalType.CharityDonation:
                    if (currentApprovalRequest.DonationID.HasValue)
                    {
                        var approvedDonation = _charityService.ApproveDonation(adminMember, currentApprovalRequest.DonationID.Value);
                        responseMessage = $"Ok, great. We have transfered $ {approvedDonation.Amount:N2} back into your account in 1-2 days so you can donate: {approvedDonation.Charity.CharityUrl}";
                    }
                    break;
                case ApprovalType.CashOut:
                    if (currentApprovalRequest.CashOutID.HasValue)
                    {
                        _spendService.ApproveCashOut(adminMember, currentApprovalRequest.CashOutID.Value);
                        responseMessage = "OK, consider it done.";
                    }
                    break;
                case ApprovalType.ChorePayment:
                    responseMessage = ApproveChores(adminMember);
                    break;
            }
            _smsApprovalHistory.MarkAsNotActive(currentApprovalRequest.Id);
            return responseMessage;
        }

        /// <summary>
        /// Disapproves the request.
        /// </summary>
        /// <param name="adminMember">The admin member</param>
        /// <returns>The message</returns>
        public string Disapprove(FamilyMember adminMember)
        {
            var responseMessage = string.Empty;
            var currentApprovalRequest = _smsApprovalHistory.GetRecentApprovalRequest(adminMember.Id);
            if (currentApprovalRequest == null)
                return string.Empty;

            switch (currentApprovalRequest.ApprovalType)
            {
                case ApprovalType.GiftPurchase:
                    if (currentApprovalRequest.PurchasedGiftCardID.HasValue)
                        _spendService.DisapprovePurchasedGiftCard(currentApprovalRequest.PurchasedGiftCardID.Value);
                    break;
                case ApprovalType.StockPurchase:
                    if (currentApprovalRequest.StockPurchaseRequestID.HasValue)
                        _saveService.DisapprovePurchasedStock(currentApprovalRequest.StockPurchaseRequestID.Value);
                    break;
                case ApprovalType.CharityDonation:
                    if (currentApprovalRequest.DonationID.HasValue)
                        _charityService.DisapproveDonation(currentApprovalRequest.DonationID.Value);
                    break;
                case ApprovalType.CashOut:
                    if (currentApprovalRequest.CashOutID.HasValue)
                        _spendService.DisapproveCashOut(currentApprovalRequest.CashOutID.Value);
                    break;
                case ApprovalType.ChorePayment:
                    _choreService.DisapprovePayDayPayment(adminMember.User.FamilyID);
                    break;
            }

            _smsApprovalHistory.MarkAsNotActive(currentApprovalRequest.Id);
            return "Ok, got it. No allowance will be paid.";
        }

        /// <summary>
        /// Approves the chores
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <returns>The message</returns>
        public string ApproveChores(FamilyMember adminMember)
        {
            if (adminMember == null)
                return string.Empty;

            //Update true to sms response
            _smsApprovalHistory.UpdateSMSReasponse(adminMember.Id, true);

            _choreService.ApprovePayDayPayment(adminMember.User.FamilyID);
            return "Ok, consider it done! ;-)\n Allowance Simplified.";
        }

        /// <summary>
        /// Cancel the not responded sms approvals.
        /// </summary>
        /// <param name="isChorePayment">This is chore payment. Default value is <c>False</c>. </param>
        /// <returns></returns>
        public void CancelNotRespondedSMS(bool isChorePayment = false)
        {
            var notRespondedSMSApprovals = _smsApprovalHistory.GetNotRespondedSMSApprovals(isChorePayment);
            if (notRespondedSMSApprovals == null)
                return;

            foreach (var smsApproval in notRespondedSMSApprovals)
            {
                switch (smsApproval.ApprovalType)
                {
                    case ApprovalType.GiftPurchase:
                        if (smsApproval.PurchasedGiftCardID.HasValue)
                            _spendService.DisapprovePurchasedGiftCard(smsApproval.PurchasedGiftCardID.Value);
                        break;
                    case ApprovalType.StockPurchase:
                        if (smsApproval.StockPurchaseRequestID.HasValue)
                            _saveService.DisapprovePurchasedStock(smsApproval.StockPurchaseRequestID.Value);
                        break;
                    case ApprovalType.CharityDonation:
                        if (smsApproval.DonationID.HasValue)
                            _charityService.DisapproveDonation(smsApproval.DonationID.Value);
                        break;
                    case ApprovalType.CashOut:
                        if (smsApproval.CashOutID.HasValue)
                            _spendService.DisapproveCashOut(smsApproval.CashOutID.Value);
                        break;
                    case ApprovalType.ChorePayment:
                        _choreService.DisapprovePayDayPayment(smsApproval.FamilyMember.User.FamilyID, true);
                        break;
                }
                _smsApprovalHistory.MarkAsNotActive(smsApproval.Id);
            }
        }
    }
}
