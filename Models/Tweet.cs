using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TwitterTweetsUserAuth.Models
{
    public class Tweet
    {
        /// <summary>
        /// Date and time the tweet was created
        /// <remarks>The format of the string date is : Tue Aug 28 21:16:23 +0000 2012 </remarks>
        /// </summary>
        [JsonProperty("created_at")]
        public string created_at { get; set; }

        /// <summary>
        /// The tweet text
        /// </summary>
        [JsonProperty("text ")]
        public string Text { get; set; }

        /// <summary>
        /// Gets both date and time
        /// </summary>
        public DateTime DateTimeCreatedOn
        {
            get
            {
                DateTime retVAl = DateTime.MinValue;

                if (!string.IsNullOrEmpty(created_at))
                {
                    retVAl = DateTime.ParseExact(created_at, "ddd MMM dd HH:mm:ss zzz yyyy",
                             CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                }

                return retVAl;
            }
        }
    }
}