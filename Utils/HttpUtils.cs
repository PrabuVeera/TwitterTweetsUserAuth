using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TwitterTweetsUserAuth.Utils
{
    /// <summary>
    /// Helper class for Http requests
    /// </summary>
    public static class HttpUtils
    {
        #region HttpCallResult
        /// <summary>
        /// Helper class that has the web request reposne details
        /// </summary>
        public class HttpCallResult
        {
            #region Properties
            /// <summary>
            /// The status code reply obtained
            /// </summary>
            public HttpStatusCode ReplyStatusCode { get; set; }

            /// <summary>
            /// Response body obtained from htptp request
            /// </summary>
            public string ResponseBody { get; set; }
            #endregion Properties

            #region Constructor
            /// <summary>
            /// Default constructor
            /// </summary>
            public HttpCallResult()
            {
                this.ReplyStatusCode = HttpStatusCode.Unused;
                this.ResponseBody = null;
            }
            #endregion Constructor
        }

        #endregion HttpCallResult

        #region MakeHttpPostRequest(string _url, string _authenticationHeaderValue , string _contentType, string _requestBodyString)
        /// <summary>
        /// Makes http request with the passed in parameters
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_authenticationHeaderValue"></param>
        /// <param name="_contentType"></param>
        /// <param name="_requestBodyString"></param>
        /// <returns></returns>
        public static HttpCallResult MakeHttpPostRequest(string _url, string _authenticationHeaderValue, string _contentType, string _requestBodyString)
        {

            HttpCallResult retVal = new HttpCallResult();

            // Create web request
            var post = WebRequest.Create(_url) as HttpWebRequest;

            // Set the mthod type, content and authentication
            post.Method = "POST";
            post.ContentType = _contentType;
            post.Headers[HttpRequestHeader.Authorization] = _authenticationHeaderValue;

            // Ste the body of the request
            var reqbody = Encoding.UTF8.GetBytes(_requestBodyString);
            post.ContentLength = reqbody.Length;
            using (var req = post.GetRequestStream())
            {
                req.Write(reqbody, 0, reqbody.Length);
            }


            // Make the request
            // ansd set the status flags accordingly
            try
            {
                using (var response = post.GetResponse() as HttpWebResponse)
                {
                    if (post.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            retVal.ReplyStatusCode = HttpStatusCode.OK;
                            retVal.ResponseBody = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        retVal.ReplyStatusCode = errorResponse.StatusCode;
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            retVal.ResponseBody = reader.ReadToEnd();
                        }
                    }
                }
            }

            return retVal;
        }

        #endregion MakeHttpPostRequest(string _url, string _authenticationHeaderValue , string _contentType, string _requestBodyString)


        #region MakeHttpGetRequest(string _url, string _methodType, string _authenticationHeaderValue , string _contentType, string _requestBodyString)
        /// <summary>
        /// Makes http request with the passed in parameters
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_authenticationHeaderValue"></param>
        /// <param name="_contentType"></param>
        /// <param name="_requestBodyString"></param>
        /// <returns></returns>
        public static HttpCallResult MakeHttpGetRequest(string _url, string _authenticationHeaderValue, string _contentType)
        {

            HttpCallResult retVal = new HttpCallResult();

            // Create web request
            var post = WebRequest.Create(_url) as HttpWebRequest;

            // Set the mthod type, content and authentication
            post.Method = "GET";
            post.ContentType = _contentType;
            post.Headers[HttpRequestHeader.Authorization] = _authenticationHeaderValue;

            // Make the request
            // ansd set the status flags accordingly
            try
            {
                using (var response = post.GetResponse() as HttpWebResponse)
                {
                    if (post.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            retVal.ReplyStatusCode = HttpStatusCode.OK;
                            retVal.ResponseBody = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        retVal.ReplyStatusCode = errorResponse.StatusCode;
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            retVal.ResponseBody = reader.ReadToEnd();
                        }
                    }
                }
            }

            return retVal;
        }

        #endregion MakeHttpRequest(string _url, string _methodType, string _authenticationHeaderValue , string _contentType, string _requestBodyString)
    }
}