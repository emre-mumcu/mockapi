using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MockAPI.AppLib.Services
{
    // dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer 
    public interface ITokenService
    {
        string CreateToken(string UserName);
        string CreateToken(ClaimsIdentity UserClaims);
        string CreateTokenEncrypted(ClaimsIdentity UserClaims);
        bool ValidateToken(string Token, out string Message, out SecurityToken? JWT, out ClaimsPrincipal? claimsPrincipal);
        bool ValidateTokenEncrypted(string Token, out string Message, out SecurityToken? JWT, out ClaimsPrincipal? claimsPrincipal);
    }
}