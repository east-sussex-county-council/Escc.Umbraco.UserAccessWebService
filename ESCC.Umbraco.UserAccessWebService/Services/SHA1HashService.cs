using System;
using System.Security.Cryptography;
using System.Text;
using UmbracoWebServices.Services.Interfaces;

namespace UmbracoWebServices.Services
{
    public class SHA1HashService : IHashService
    {
        public string HashPassword(string password)
        {
            var hash = new HMACSHA1 { Key = Encoding.Unicode.GetBytes(password) };

            var encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));

            hash.Dispose();

            return encodedPassword;
        }
    }
}