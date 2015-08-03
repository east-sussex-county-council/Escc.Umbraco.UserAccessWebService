using System;
using System.Collections.Generic;

namespace ESCC.Umbraco.UserAccessWebService.Models
{
    public class ExpiringPageModel
    {
        public ExpiringPageModel()
        {
            PageUsers = new List<UmbracoUserModel>();
        }

        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PagePath { get; set; }

        public DateTime ExpiryDate { get; set; }

        public IList<UmbracoUserModel> PageUsers { get; set; }
    }
}