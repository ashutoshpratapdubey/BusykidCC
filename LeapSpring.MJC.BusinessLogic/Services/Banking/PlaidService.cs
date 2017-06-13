using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Dto.Banking;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public class PlaidService : IPlaidService
    {
        private IAppSettingsService _appSettingsService;

        public PlaidService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        async public Task<string> LifeTimeAccessToken(string publicToken)
        {
            dynamic result;
            dynamic body = new ExpandoObject();
            body.client_id = _appSettingsService.PlaidClientID;
            body.secret = _appSettingsService.PlaidClientSecret;
            body.public_token = publicToken;            

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.SendAsync(AuthenticatedRequest("POST", "exchange_token", body));
                result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            }

            return result["access_token"].Value;            
        }

        async public Task<BankInfo> GetBankInfo(string accessToken, string selectedAccountId)
        {
            BankInfo bankInfo = new BankInfo();
            dynamic body = new ExpandoObject();
            body.client_id = _appSettingsService.PlaidClientID;
            body.secret = _appSettingsService.PlaidClientSecret;
            body.access_token = accessToken;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.SendAsync(AuthenticatedRequest("POST", "auth/get", body));
                var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                foreach (var account in result["accounts"])
                {
                    if(selectedAccountId.Equals(account["_id"].ToString()))
                    {
                        bankInfo.AccountNumber = account["numbers"]["account"].Value.ToString();
                        bankInfo.RoutingNumber = account["numbers"]["routing"].Value.ToString();
                        bankInfo.AccountType = account["type"].Value.ToString();
                        bankInfo.AccountSubType = account["subtype"].Value.ToString();
                    }
                }
            }

            return bankInfo;
        }

        /// <summary>
        /// Remove the plaid bank info.
        /// </summary>
        /// <param name="accessToken">The token to be removed.</param>
        /// <returns>If removed,<c>True</c>. Otherwise <c>False</c>.</returns>
        async public Task<bool> RemoveBankInfo(string accessToken)
        {
            dynamic body = new ExpandoObject();
            body.client_id = _appSettingsService.PlaidClientID;
            body.secret = _appSettingsService.PlaidClientSecret;
            body.access_token = accessToken;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.SendAsync(AuthenticatedRequest("DELETE", "auth", body));
                return response.IsSuccessStatusCode;
            }
        }

        private HttpRequestMessage AuthenticatedRequest(string method, string path, dynamic body)
        {
            var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };

            return new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri($"{_appSettingsService.PlaidBaseUrl}/{path}"),
                Content = new StringContent(JsonConvert.SerializeObject(body, settings), Encoding.UTF8, "application/json")
            };
        }
    }
}
