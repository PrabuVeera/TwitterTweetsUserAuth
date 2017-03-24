using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using TweetSharp;
using TwitterTweetsUserAuth.Models;

namespace TwitterTweetsUserAuth.Utils
{
    /// <summary>
    /// Helper class to authenticate using OAuth2.0
    /// </summary>
    public static class OuthAuthenticationHelper
    {
        #region AccessTokenResult
        /// <summary>
        /// The access token result class
        /// </summary>
        public class AccessTokenResult
        {
            /// <summary>
            /// The obtained access token to be used
            /// </summary>
            public TwitterTweetsUserAuth.Models.OAuthAccessToken OAuthToken { get; set; }

            /// <summary>
            /// Error message if any
            /// </summary>
            public string ErroMessage { get; set; }

            /// <summary>
            /// Indciates if it has a valid access token
            /// </summary>
            public bool HasAuthToken
            {
                get
                {
                    return string.IsNullOrEmpty(ErroMessage) && OAuthToken != null && !string.IsNullOrEmpty(OAuthToken.Token_type)
                        && !string.IsNullOrEmpty(OAuthToken.Access_Token);
                }
            }

            public AccessTokenResult()
            {
                this.OAuthToken = null;
                this.ErroMessage = null;
            }
        }

        #endregion AccessTokenResult

        #region GetAccessToken()
        /// <summary>
        /// Gets the access token from twitter api using the credentials passed in
        /// </summary>
        /// <returns></returns>
        public static AccessTokenResult GetAccessToken()
        {
            AccessTokenResult accessTokenValue = new AccessTokenResult();

            /*
              post.Method = "POST";
            post.ContentType = "application/x-www-form-urlencoded";
            post.Headers[HttpRequestHeader.Authorization] = "Basic " + Base64Encode(credentials);
            var reqbody = Encoding.UTF8.GetBytes("grant_type=client_credentials");
             * */

            string authHeader = string.Format("Basic : {0}", GetBase64CredentialsForTwitterFromKey());

            var httpResult = HttpUtils.MakeHttpPostRequest("https://api.twitter.com/oauth2/token", authHeader, "application/x-www-form-urlencoded",
                "grant_type=client_credentials");

            //
            // If the status code is OK, parse the json and get the access token
            // 
            if (httpResult.ReplyStatusCode == System.Net.HttpStatusCode.OK)
            {
                JavaScriptSerializer serailizer = new JavaScriptSerializer();
                accessTokenValue.OAuthToken = serailizer.Deserialize<TwitterTweetsUserAuth.Models.OAuthAccessToken>(httpResult.ResponseBody);
            }
            else
            {
                accessTokenValue.ErroMessage = string.Format("Access token cannot be retrieved. Obtained error code {0} from twitter with message {1}.",
                    httpResult.ReplyStatusCode, httpResult.ResponseBody);
            }

            return accessTokenValue;
        }

        #endregion GetAccessToken()

        #region GetAuthenticatedTwitterService(HttpContext _context)
        /// <summary>
        /// Gets the authenticated twitter service
        /// </summary>
        /// <returns></returns>
        public static TwitterService GetAuthenticatedTwitterService( HttpContext _context)
        {
            var consumerKey = GetTwitterConsumerApiKey();
            var consumerSecret = GetTwitterConsumerSecretKey();
            var service = new TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(_context.Session["accesstoken"].ToString(),
                _context.Session["accesssecret"].ToString());

            return service;
        }
        #endregion GetAuthenticatedTwitterService()

        #region GetTwitterConsumerApiKey()
        /// <summary>
        /// Gets the Twitter api consumer api key from app settings
        /// </summary>
        /// <returns></returns>
        public static string GetTwitterConsumerApiKey()
        {
            return ConfigurationManager.AppSettings["ConsumerKey"].ToString(); ; 
        }
        #endregion GetTwitterConsumerApiKey()

        #region GetTwitterConsumerSecretKey()
        /// <summary>
        /// Gets the Twitter api consumer secret key from app settings
        /// </summary>
        /// <returns></returns>
        public static string GetTwitterConsumerSecretKey()
        {
            return ConfigurationManager.AppSettings["ConsumerSecret"].ToString(); ;
        }
        #endregion GetTwitterConsumerSecretKey()

        #region GetBase64CredentialsForTwitterFromKey()
        /// <summary>
        /// Helper method to get the crendtials to be put in the 
        /// authentication header to get the access token from Twitter
        /// </summary>
        /// <returns></returns>
        public static string GetBase64CredentialsForTwitterFromKey()
        {
            //
            // Key:Secret should be base 64 encoded to make it work
            //
            string credential = string.Format("{0}:{1}", GetTwitterConsumerApiKey(), 
                GetTwitterConsumerSecretKey());

            //
            // Get the 
            //
            byte[] retValInBytes = System.Text.Encoding.UTF8.GetBytes(credential);
            string base64EncodedCredential = System.Convert.ToBase64String(retValInBytes);

            return base64EncodedCredential;
        }
        #endregion GetBase64CredentialsForTwitterFromKey()
    }
}