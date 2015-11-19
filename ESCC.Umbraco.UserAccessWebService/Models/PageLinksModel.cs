using System.Collections.Generic;

namespace ESCC.Umbraco.UserAccessWebService.Models
{
    /// <summary>
    /// Contains details of links into a page from multiple sources
    /// </summary>
    public class PageLinksModel
    {
        public PageLinksModel()
        {
            InboundLinksLocal = new List<PageInLinkModel>();
        }

        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PageUrl { get; set; }

        public List<PageInLinkModel> InboundLinksLocal { get; set; }
    }
}