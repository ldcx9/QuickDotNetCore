using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Models;
using QuickDotNetCore.Src.vo;

namespace QuickDotNetCore.Src.Utils
{
    public class JwtUtils
    {
        public static JwtSecurityToken CreateToken(UserDO user) {
            Claim[] claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role)
                };
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(Const.SecurityKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                issuer: Const.JwtIssuer,
                audience: Const.JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            return token;
        }
    }
}
