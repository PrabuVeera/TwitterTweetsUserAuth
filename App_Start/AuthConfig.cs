using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using TwitterTweetsUserAuth.Models;
using System.Configuration;
using TwitterTweetsUserAuth.Utils;

namespace TwitterTweetsUserAuth
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
              //  consumerKey: ConfigurationManager.AppSettings["ConsumerKey"].ToString(),  //"gKjQjMgASK3PPNi1xd38HMidV",
                //consumerSecret: ConfigurationManager.AppSettings["ComsumerSecret"].ToString()); //"RtUyuTui56mZEMvrU9rZsJtZ1vqc1OuNOvtNscWS0sZB2yn7Bv");

            OAuthWebSecurity.RegisterClient(new TwitterClient(
    consumerKey: ConfigurationManager.AppSettings["ConsumerKey"].ToString(),
    consumerSecret: ConfigurationManager.AppSettings["ConsumerSecret"].ToString()), "Twitter", null);

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
