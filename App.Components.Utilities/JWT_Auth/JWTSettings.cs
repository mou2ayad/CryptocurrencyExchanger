using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.Utilities.JWT_Auth
{
    public class JWTSettings
    {
        public string SecretKey { set; get; }
        public int ValidityInHours { set; get; }     
    }
}
