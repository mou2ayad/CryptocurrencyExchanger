using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace App.Components.Utilities.JWT_Auth
{

    public abstract class UserService : IUserService
    {
        

        private readonly JWTSettings _jwtSettings;

        public UserService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public abstract UserClient Login(string UserName,string Password);
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            
            var user = Login(model.Username, model.Password);
            if (user == null) return null;
            
            DateTime TokenExpiryDate;
            var token = generateJwtToken(user, out TokenExpiryDate);

            return new AuthenticateResponse(token, TokenExpiryDate);
        }
        private string generateJwtToken(UserClient user,out DateTime TokenExpiryDate)
        {           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            TokenExpiryDate = DateTime.UtcNow.AddHours(_jwtSettings.ValidityInHours);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("UserName", user.UserName) , new Claim("permissions",string.Join(",", user.Permissions)) }),
                Expires = TokenExpiryDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
  
}
