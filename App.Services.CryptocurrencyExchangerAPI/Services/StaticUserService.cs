using App.Components.Utilities.JWT_Auth;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Services.CryptocurrencyExchangerAPI.Services
{
    //IMPORTANT: this is just mock testing (POC), we should logging using real Authentication and Authorization service
    public class StaticUserService : UserService
    {
        private List<UserClient> _users;
        public StaticUserService(IOptions<JWTSettings> jwtSettings) : base(jwtSettings)
        {
            _users = new List<UserClient>
            {
                new UserClient { Id = 1, UserName = "knab", Password = "knab2021", Permissions = new List<string>() { "Quotes" } },
                new UserClient { Id = 2, UserName = "test", Password = "test2021", Permissions = new List<string>() { } }
            };


        }
        public override UserClient Login(string UserName, string Password)
        {
            return _users.SingleOrDefault(x => x.UserName.ToLower() == UserName.ToLower() && x.Password == Password);
        }
    }
}
