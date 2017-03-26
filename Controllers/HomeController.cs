using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TweetSharp;
using TwitterTweetsUserAuth.Models;
using TwitterTweetsUserAuth.Utils;

namespace TwitterTweetsUserAuth.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        /// <summary>
        /// Controller method to serach with username like ''
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchForUsersLike(string Prefix)
        {
            var stringResult = new object { };
            if (!string.IsNullOrEmpty(Prefix))
            {
                try
                {
                    var service = OuthAuthenticationHelper.GetAuthenticatedTwitterService(System.Web.HttpContext.Current);

                    stringResult = service.SearchForUser(
                        new SearchForUserOptions() { Q = Prefix, Count = 10, Page = 1 })
                         .Select(x => new { x.ScreenName, profurl = x.ProfileImageUrl }).ToArray();
                }
                catch (Exception ex)
                {
                    //throw new Exception("Error-" + ex);
                }
            }
           return Json(stringResult);
        }

        public ActionResult SearchTweetsForUser(SearchEntity user)
        {

            if (!string.IsNullOrEmpty(user.UserName))
            {
                TwitterTweetsUserAuth.Models.TwitterSearchResult tweetSearchResult = GetTweets(user.UserName, user.NumberOfTweets);

                if (string.IsNullOrEmpty(tweetSearchResult.ErrorMessage))
                {
                    ViewData[TwitterTweetsUserAuth.Models.TwitterSearchResult.PARAM_NAME] = tweetSearchResult.Tweets;
                }
                else
                {
                    ModelState.AddModelError("", tweetSearchResult.ErrorMessage);
                }

            }
            return View("Output");
        }


        /// <summary>
        /// gets the tweets
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TwitterTweetsUserAuth.Models.TwitterSearchResult GetTweets(string userName, int count)
        {
            Models.TwitterSearchResult retVal = new Models.TwitterSearchResult();
            try
            {
                // Get the access
                var aToken = OuthAuthenticationHelper.GetAccessToken() ;
                if (aToken != null)
                {
                    userName = userName.Replace(" ", "%20");

                    string TWEETS_GET_UNFORMATTED_URL = "https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&trim_user=1&exclude_replies=1";

                    string url = string.Format(TWEETS_GET_UNFORMATTED_URL,
                              count, userName);
                    // using the access toke get the tweets
                    var result = HttpUtils.MakeHttpGetRequest(url, aToken.OAuthToken.GetAuthenticationHeaderValue(),
                        "application/json");

                    // only id repsosne is ok proceed
                    if (result.ReplyStatusCode == HttpStatusCode.OK)
                    {
                        retVal.Tweets = new JavaScriptSerializer().Deserialize<Tweet[]>(result.ResponseBody);
                    }
                    else
                    {
                        retVal.ErrorMessage = result.ResponseBody;
                    }
                }
            }
            catch (Exception exc)
            {
                retVal.ErrorMessage = exc.Message;
            }
            return retVal;
            
        }


        

    }
}
