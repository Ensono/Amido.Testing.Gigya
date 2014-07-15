using System;
using System.Collections.Generic;

using Amido.Testing.Framework;
using Amido.Testing.Gigya.Contracts;

using Gigya.Socialize.SDK;

namespace Amido.Testing.Gigya.Workflow
{
    public class GigyaRegistrationWorkflow : IRegisterUser, IUpdateGigyaUser, IFinalizeRegistration
    {
        private readonly string initializationRegToken;
        private readonly string gigyaApiKey;
        private readonly string gigyaSecretKey;
        private readonly string apiDomain;
        private string uid;
        private string regToken;

        public GigyaRegistrationWorkflow(
            string initializationRegToken, 
            string gigyaApiKey,
            string gigyaSecretKey,
            string apiDomain)
        {
            this.initializationRegToken = initializationRegToken;
            this.gigyaApiKey = gigyaApiKey;
            this.gigyaSecretKey = gigyaSecretKey;
            this.apiDomain = apiDomain;
        }

        public IUpdateGigyaUser Register(string email, string password)
        {
            return RetryHelper.Do<IUpdateGigyaUser>(
                () =>
                    {
                        var request = new GSRequest(this.gigyaApiKey, this.gigyaSecretKey, "accounts.register", true)
                                          {
                                              APIDomain
                                                  =
                                                  this
                                                  .apiDomain
                                          };
                        request.SetParam("email", email);
                        request.SetParam("password", password);
                        request.SetParam("regToken", this.initializationRegToken);

                        var response = request.Send();
                        var errorCode = response.GetInt("errorCode", 0);
                        if (errorCode != 0 && errorCode != 206001)
                        {
                            throw new Exception("Registration failed");
                        }

                        var registrationData = response.GetData();
                        this.uid = registrationData.GetString("UID");
                        this.regToken = registrationData.GetString("regToken");

                        return this;
                    },
                TimeSpan.FromSeconds(1));
        }

        public IFinalizeRegistration SetAccountInfo(Dictionary<string, object> gigyaUserParts)
        {
            return RetryHelper.Do<IFinalizeRegistration>(
                () =>
                    {
                        var request = new GSRequest(
                            this.gigyaApiKey,
                            this.gigyaSecretKey,
                            "accounts.setAccountInfo",
                            true) { APIDomain = this.apiDomain };
                        request.SetParam("UID", this.uid);

                        foreach (var gigyaUserPart in gigyaUserParts)
                        {
                            request.SetParam(gigyaUserPart.Key, JsonSerializer.ToJsonString(gigyaUserPart.Value));
                        }

                        var response = request.Send();
                        var errorCode = response.GetInt("errorCode", 0);
                        if (errorCode != 0)
                        {
                            throw new Exception("Set account info failed");
                        }

                        return this;
                    },
                TimeSpan.FromSeconds(1));
        }

        public string FinalizeRegistration()
        {
            RetryHelper.Do(
                () =>
                    {
                        var request = new GSRequest(
                            this.gigyaApiKey,
                            this.gigyaSecretKey,
                            "accounts.finalizeRegistration",
                            true) { APIDomain = this.apiDomain };

                        request.SetParam("regToken", this.regToken);
                        var finalizeResponse = request.Send();
                        var errorCode = finalizeResponse.GetInt("errorCode", 0);
                        if (errorCode != 0)
                        {
                            throw new Exception("Finalize registration failed");
                        }
                    },
                TimeSpan.FromSeconds(1));

            return uid;
        }
    }
}