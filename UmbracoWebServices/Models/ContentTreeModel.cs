using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoWebServices.Models
{
    public class ContentTreeModel
    {
        public int PageId { get; set; }

        public int ParentId { get; set; }

        public int RootId { get; set; }

        public string PageName { get; set; }

        public bool Published { get; set; }

        public DateTime PublishedDate { get; set; }
    }
}