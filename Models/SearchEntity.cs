using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitterTweetsUserAuth.Models
{
    public class SearchEntity
    {
        public string UserName { get; set; }


        public int NumberOfTweets { get; set; }


        /// <summary>
        /// Gets the list of items to be displayed to give the option to user
        /// </summary>
        /// <returns></returns>
        public static SelectListItem[] GetListItems()
        {
            int[] retVal = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return retVal.Select(x => new SelectListItem() { Selected = x == 5, Text = x + "", Value = x + "" }).ToArray();
        }
    }
}