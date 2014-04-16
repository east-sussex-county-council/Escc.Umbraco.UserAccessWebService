using System;

namespace UmbracoWebServices.Services
{
    public interface IHashService
    {
        string HashPassword(string password);
    }
}