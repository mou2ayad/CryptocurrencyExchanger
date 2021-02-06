using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace App.Components.Utilities.JWT_Auth
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }
        public DateTime TokenExpiryDate { get; set; }


        public AuthenticateResponse( string token,DateTime tokenExpiryDate)
        {
            Token = token;
            TokenExpiryDate = tokenExpiryDate;
        }
    }
}
