using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UmbracoWebServices.Services
{
    public class SHA1HashService : IHashService
    {
        public string HashPassword(string password)
        {
            HMACSHA1 hash = new HMACSHA1() { Key = Encoding.Unicode.GetBytes(password) };

            string encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));

            hash.Dispose();

            return encodedPassword;
        }
    }
}