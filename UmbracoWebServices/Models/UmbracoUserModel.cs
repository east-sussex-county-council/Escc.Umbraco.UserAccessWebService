using System;
using System.Collections.Generic;
using System.Linq;

namespace UmbracoWebServices.Models
{
    public class UmbracoUserModel
    {
        public string userName { get; set; }

        public string fullName { get; set; }

        public string emailAddress { get; set; }

        public int UserId { get; set; }

        public bool Lock { get; set; }
    }
}