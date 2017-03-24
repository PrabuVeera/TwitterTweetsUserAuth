using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterTweetsUserAuth.Models
{
    /// <summary>
    /// Indicates the oAuth ACcess token obatined from Twitter api
    /// </summary>
    public class OAuthAccessToken
    {
        /// <summary>
        /// Obtained token type
        /// </summary>
        [JsonProperty("token_type")]
        public string Token_type { get; set; }

        /// <summary>
        /// Obtained access token
        /// </summary>
        [JsonProperty("access_token")]
        public string Access_Token { get; set; }

        /// <summary>
        /// Gets the value to be used in the header
        /// </summary>
        /// <returns></returns>
        public string GetAuthenticationHeaderValue()
        {
            return string.Format("{0} {1}", Token_type, Access_Token);
        }
    }
}