using System;

using Amido.Testing.Framework;
using Amido.Testing.Gigya.Contracts;
using Amido.Testing.Gigya.Workflow;

using Gigya.Socialize.SDK;

namespace Amido.Testing.Gigya
{
    public class GigyaRegistrationService
    {
        public IRegisterUser Initialize(
           string gigyaApiKey,
           string gigyaSecretKey,
           string apiDomain)
        {
            var regToken = RetryHelper.Do(() => InitializeRegistration(gigyaApiKey, gigyaSecretKey, apiDomain), TimeSpan.FromSeconds(1));
            return new GigyaRegistrationWorkflow(regToken, gigyaApiKey, gigyaSecretKey, apiDomain);
        }

        public string InitializeRegistration(
            string gigyaApiKey,
           string gigyaSecretKey,
           string apiDomain)
        {
            var request = new GSRequest(gigyaApiKey, gigyaSecretKey, "accounts.initRegistration", false)
            {
                APIDomain = apiDomain
            };

            var response = request.Send();

            return response.GetData().GetString("regToken");
        }
    }
}