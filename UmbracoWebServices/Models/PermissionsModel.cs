using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoWebServices.Models
{
    public class PermissionsModel
    {
        public int PermissionId { get; set; }

        public int PageId { get; set; }

        public int UserId { get; set; }

        public string Created { get; set; }

        public int TargetId { get; set; }

        public string PageName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }
    }
}