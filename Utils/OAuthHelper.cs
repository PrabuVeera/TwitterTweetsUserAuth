﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterTweetsUserAuth.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Net;
    using System.IO;
    using System.Text;
    using System.Configuration;

    public class OAuthHelper
    {
        public OAuthHelper() { }

        // consumerKey: ConfigurationManager.AppSettings["ConsumerKey"].ToString(),  //"gKjQjMgASK3PPNi1xd38HMidV",
                //consumerSecret: ConfigurationManager.AppSettings["ComsumerSecret"].ToString()); //"RtUyuTui56mZEMvrU9rZsJtZ1vqc1OuNOvtNscWS0sZB2yn7Bv");

        static string oauth_consumer_key = ConfigurationManager.AppSettings["ConsumerKey"];
        static string oauth_consumer_secret = ConfigurationManager.AppSettings["ComsumerSecret"];
        static string callbackUrl = "http://127.0.0.1:53795";

        #region (Changable) Do Not Change It
        static string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
        static string AUTHORIZE = "https://api.twitter.com/oauth/authorize";
        static string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";

        public static string GetConsumerKey { get { return oauth_consumer_key; } }
        public static string GetConsumerSecret { get { return oauth_consumer_secret; } }
        public static string GetAUTHORIZE { get { return AUTHORIZE; } }

        public enum httpMethod
        {
            POST, GET
        }
        public string oauth_request_token { get; set; }
        public string oauth_access_token { get; set; }
        public string oauth_access_token_secret { get; set; }
        public string user_id { get; set; }
        public string screen_name { get; set; }
        public string oauth_error { get; set; }

        public void LoginWithTwitter()
        {
            HttpWebRequest request = FetchRequestToken(httpMethod.POST, oauth_consumer_key, oauth_consumer_secret);
            string result = getResponce(request);
            Dictionary<string, string> resultData = OAuthUtility.GetQueryParameters(result);
            if (resultData.Keys.Contains("oauth_token"))
                this.oauth_request_token = resultData["oauth_token"];
            else
                this.oauth_error = result;
        }
        public void GetUserTwAccessToken(string oauth_token, string oauth_verifier)
        {
            HttpWebRequest request = FetchAccessToken(httpMethod.POST, oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_verifier);
            string result = getResponce(request);

            Dictionary<string, string> resultData = OAuthUtility.GetQueryParameters(result);
            if (resultData.Keys.Contains("oauth_token"))
            {
                this.oauth_access_token = resultData["oauth_token"];
                this.oauth_access_token_secret = resultData["oauth_token_secret"];
                this.user_id = resultData["user_id"];
                this.screen_name = resultData["screen_name"];
            }
            else
                this.oauth_error = result;
        }
        public void PostTwitsOnBehalfOf(string oauth_access_token, string oauth_token_secret, string postData)
        {
            HttpWebRequest request = OAuthHelper.PostTwits(oauth_consumer_key, oauth_consumer_secret, oauth_access_token, oauth_token_secret, postData);
            string result = OAuthHelper.getResponce(request);
            Dictionary<string, string> dcResult = OAuthUtility.GetQueryParameters(result);
            if (dcResult["status"] != "200")
            {
                this.oauth_error = result;
            }

        }

        public static string getResponce(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                string result = reader.ReadToEnd();
                reader.Close();
                return result + "&status=200";
            }
            catch (Exception ex)
            {
                string statusCode = "";
                if (ex.Message.Contains("403"))
                    statusCode = "403";
                else if (ex.Message.Contains("401"))
                    statusCode = "401";
                return string.Format("status={0}&error={1}", statusCode, ex.Message);
            }
        }


        static HttpWebRequest FetchRequestToken(httpMethod method, string oauth_consumer_key, string oauth_consumer_secret)
        {
            string OutUrl = "";
            string OAuthHeader = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(REQUEST_TOKEN), callbackUrl, method.ToString(), oauth_consumer_key, oauth_consumer_secret, "", "", out OutUrl);

            if (method == httpMethod.GET)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OutUrl + "?" + OAuthHeader);
                request.Method = method.ToString();
                return request;
            }
            else if (method == httpMethod.POST)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OutUrl);
                request.Method = method.ToString();
                request.Headers["Authorization"] = OAuthHeader;
                return request;
            }
            else
                return null;


        }
        static HttpWebRequest FetchAccessToken(httpMethod method, string oauth_consumer_key, string oauth_consumer_secret, string oauth_token, string oauth_verifier)
        {
            string postData = "oauth_verifier=" + oauth_verifier;
            string AccessTokenURL = string.Format("{0}?{1}", ACCESS_TOKEN, postData);
            string OAuthHeader = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(AccessTokenURL), callbackUrl, method.ToString(), oauth_consumer_key, oauth_consumer_secret, oauth_token, "", out AccessTokenURL);

            if (method == httpMethod.GET)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AccessTokenURL + "?" + OAuthHeader);
                request.Method = method.ToString();
                return request;
            }
            else if (method == httpMethod.POST)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AccessTokenURL);
                request.Method = method.ToString();
                request.Headers["Authorization"] = OAuthHeader;

                byte[] array = Encoding.ASCII.GetBytes(postData);
                request.GetRequestStream().Write(array, 0, array.Length);
                return request;
            }
            else
                return null;

        }
        public static HttpWebRequest PostTwits(string oauth_consumer_key, string oauth_consumer_secret, string oauth_access_token, string oauth_token_secret, string postData)
        {
            postData = "trim_user=true&include_entities=true&status=" + postData;
            string updateStatusURL = "https://api.twitter.com/1/statuses/update.json?" + postData;

            string outUrl;
            string OAuthHeaderPOST = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(updateStatusURL), callbackUrl, httpMethod.POST.ToString(), oauth_consumer_key, oauth_consumer_secret, oauth_access_token, oauth_token_secret, out outUrl);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(outUrl);
            request.Method = httpMethod.POST.ToString();
            request.Headers["Authorization"] = OAuthHeaderPOST;

            byte[] array = Encoding.ASCII.GetBytes(postData);
            request.GetRequestStream().Write(array, 0, array.Length);
            return request;

        }

        public static HttpWebRequest GetUsers(string oauth_consumer_key, string oauth_consumer_secret, string oauth_access_token, string oauth_token_secret, string username)
        {
            username = string.Format("q={0}&page=1&count=3" , username);
            string updateStatusURL = "https://api.twitter.com/1.1/users/search.json?" + username;

            string outUrl;
            string OAuthHeaderPOST = OAuthUtility.GetAuthorizationHeaderForPost_OR_QueryParameterForGET(new Uri(updateStatusURL), callbackUrl, httpMethod.GET.ToString(), oauth_consumer_key, oauth_consumer_secret, oauth_access_token, oauth_token_secret, out outUrl);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateStatusURL);
            request.Method = httpMethod.GET.ToString();
            request.Headers["Authorization"] = OAuthHeaderPOST;

            //byte[] array = Encoding.ASCII.GetBytes(postData);
            //request.GetRequestStream().Write(array, 0, array.Length);
            return request;

        }

        #endregion
    }
}