using LeapSpring.MJC.Core.Dto.Banking;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public interface IPlaidService
    {
        Task<string> LifeTimeAccessToken(string publicToken);

        Task<BankInfo> GetBankInfo(string accessToken, string selectedAccountId);

        /// <summary>
        /// Remove the plaid bank info.
        /// </summary>
        /// <param name="accessToken">The token to be removed.</param>
        /// <returns>If removed,<c>True</c>. Otherwise <c>False</c>.</returns>
        Task<bool> RemoveBankInfo(string accessToken);
    }
}
