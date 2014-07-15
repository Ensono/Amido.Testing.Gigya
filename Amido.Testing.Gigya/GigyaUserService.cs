using System;
using System.Linq;

using Amido.Testing.Framework;

using Gigya.Socialize.SDK;

using Newtonsoft.Json.Linq;

namespace Amido.Testing.Gigya
{
    public class GigyaUserService
    {
        public void DeleteByUserId(string gigyaApiKey, string gigyaSecretKey, string apiDomain, string userId)
        {
            var request = new GSRequest(gigyaApiKey, gigyaSecretKey, "accounts.deleteAccount", false);
            request.SetParam("UID", userId);
            request.APIDomain = apiDomain;
            request.Send();
        }

        public void DeleteByEmail(string gigyaApiKey, string gigyaSecretKey, string apiDomain, string email)
        {
            var request = new GSRequest(gigyaApiKey, gigyaSecretKey, "accounts.search", false);
            request.SetParam("query", "select * from accounts where profile.email =\"" + email + "\"");
            request.APIDomain = apiDomain;

            var response = request.Send();

            if (response.GetErrorCode() != 0)
            {
                throw new Exception("Unable to find user by email");
            }

            var searchResults = response.GetResponseText();

            var searchResultsObject = JObject.Parse(searchResults);
            var results = searchResultsObject["results"];

            if (results == null || results.Count() != 1)
            {
                return;
            }

            var resultsArray = new JArray(searchResultsObject["results"]);
            var uid = resultsArray[0][0]["UID"].Value<string>();
            this.DeleteByUserId(gigyaApiKey, gigyaSecretKey, apiDomain, uid);
        }

        public TUser GetUserByEmail<TUser>(
            string gigyaApiKey,
            string gigyaSecretKey,
            string apiDomain,
            string email,
            int maxRetries,
            int intervalInMilliseconds,
            Func<TUser, bool> retryFunction)
            where TUser : class
        {
            return RetryHelper.Do(() => GetUserByEmail(gigyaApiKey, gigyaSecretKey, apiDomain, email, retryFunction), TimeSpan.FromMilliseconds(intervalInMilliseconds), maxRetries);
        }

        public TUser GetUserByEmail<TUser>(string gigyaApiKey, string gigyaSecretKey, string apiDomain, string email, Func<TUser, bool> retryFunction)
            where TUser : class
        {
            var request = new GSRequest(gigyaApiKey, gigyaSecretKey, "accounts.search", false);
            request.SetParam("query", "select * from accounts where profile.email =\"" + email + "\"");
            request.APIDomain = apiDomain;

            var response = request.Send();

            if (response.GetErrorCode() != 0)
            {
                throw new Exception("Unable to find user by email");
            }

            var searchResults = response.GetResponseText();

            var searchResultsObject = JObject.Parse(searchResults);
            var results = searchResultsObject["results"];

            if (results == null || results.Count() != 1)
            {
                return null;
            }

            var user = JsonSerializer.FromJsonString<TUser>(results[0].ToString());

            if (!retryFunction(user))
            {
                throw new Exception("Retry condition failed: retry required");
            }

            return user;
        } 
    }
}