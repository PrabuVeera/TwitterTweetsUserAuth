using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterTweetsUserAuth.Utils
{
    public class TwitterClient : OAuthClient
    {

        /// <summary>
        /// The description of Twitter's OAuth protocol URIs for use with their "Sign in with Twitter" feature.
        /// </summary>
        public static readonly ServiceProviderDescription TwitterServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint =
                new MessageReceivingEndpoint(
                    "https://api.twitter.com/oauth/request_token",
                    HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint =
                new MessageReceivingEndpoint(
                    "https://api.twitter.com/oauth/authenticate",
                    HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint =
                new MessageReceivingEndpoint(
                    "https://api.twitter.com/oauth/access_token",
                    HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        public TwitterClient(string consumerKey, string consumerSecret) :
            base("twitter", TwitterServiceDescription, consumerKey, consumerSecret) { }

        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// The response token returned from service provider authentication result. 
        protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
        {
            string accessToken = response.AccessToken;
            string accessSecret = (response as ITokenSecretContainingMessage).TokenSecret;
            string userId = response.ExtraData["user_id"];
            string userName = response.ExtraData["screen_name"];

            var extraData = new Dictionary<string, string>()
                            {
                                {"accesstoken", accessToken},
                                {"accesssecret", accessSecret}
                            };
            return new AuthenticationResult(
                isSuccessful: true,
                provider: ProviderName,
                providerUserId: userId,
                userName: userName,
                extraData: extraData);
        }
    }
}