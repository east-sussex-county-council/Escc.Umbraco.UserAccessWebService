using System;

namespace Escc.Umbraco.UserAccessWebService.Models
{
    public class UmbracoUserModel
    {
        //public bool Equals(UmbracoUserModel other)
        //{
        //    return UserId == other.UserId;
        //}
        public override bool Equals(object other)
        {
            if (!(other is UmbracoUserModel)) throw new ArgumentException("obj is not an UmbracouserModel");

            var otherUser = (UmbracoUserModel) other;

            return UserId == otherUser.UserId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public int UserId { get; set; }

        public bool UserLocked { get; set; }

        public bool IsWebAuthor { get; set; }
    }
}