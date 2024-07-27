using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MockAPI.AppLib.Services;

// dotnet add package Microsoft.IdentityModel.Tokens
// dotnet add package System.IdentityModel.Tokens.Jwt

public partial class TokenService : ITokenService
{
    /*
        var hash = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(CM.DataConfiguration["TokenParameters:SignKey"]!));    
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) result.Append(hash[i].ToString("X2"));
            return result.ToString();
        }
    */
    private SecurityKey GetSecurityKey(string key) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

    private SigningCredentials GetSigningCredentials()
    {
        SigningCredentials signingCredentials = new SigningCredentials(
            key: GetSecurityKey(App.Instance._DataConfiguration["TokenParameters:SignKey"]!),
            algorithm: SecurityAlgorithms.HmacSha512Signature
        );

        return signingCredentials;
    }

    private EncryptingCredentials GetEncryptingCredentials()
    {
        SecurityKey securityKey = GetSecurityKey(App.Instance._DataConfiguration["TokenParameters:EncryptKey"]!);

        var (al, en) = securityKey.KeySize switch
        {
            128 => (SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256),
            192 => (SecurityAlgorithms.Aes192KeyWrap, SecurityAlgorithms.Aes192CbcHmacSha384),
            256 => (SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512),
            _ => throw new ArgumentException("Size of encryption key can be 128, 192, or 256 bits")
        };

        return new EncryptingCredentials(key: securityKey, alg: al, enc: en);
    }

    private SecurityTokenDescriptor GetSecurityTokenDescriptor(ClaimsIdentity subject)
    {
        return new SecurityTokenDescriptor()
        {
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Subject = subject,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(App.Instance._DataConfiguration["TokenParameters:Timeout"])),
            Issuer = App.Instance._DataConfiguration["TokenParameters:Issuer"],
            Audience = App.Instance._DataConfiguration["TokenParameters:Audience"],
            SigningCredentials = GetSigningCredentials()
        };
    }

    private SecurityTokenDescriptor GetSecurityTokenDescriptorEncrypted(ClaimsIdentity subject)
    {
        SecurityTokenDescriptor tokenDescriptor = GetSecurityTokenDescriptor(subject);

        tokenDescriptor.EncryptingCredentials = GetEncryptingCredentials();

        return tokenDescriptor;
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = App.Instance._DataConfiguration["TokenParameters:Issuer"],
            ValidAudience = App.Instance._DataConfiguration["TokenParameters:Audience"],
            IssuerSigningKey = GetSecurityKey(App.Instance._DataConfiguration["TokenParameters:SignKey"]!),
            ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(App.Instance._DataConfiguration["TokenParameters:ClockSkew"]))
        };
    }

    private TokenValidationParameters GetTokenValidationParametersEncrypted()
    {
        TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();
        tokenValidationParameters.TokenDecryptionKey = GetSecurityKey(App.Instance._DataConfiguration["TokenParameters:EncryptKey"]!);
        return tokenValidationParameters;
    }

    public string CreateToken(string UserName)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        var userClaims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.NameId, UserName)
        };

        ClaimsIdentity ci = new ClaimsIdentity(userClaims);

        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptor(ci));
        return tokenHandler.WriteToken(token);
    }

    public string CreateToken(ClaimsIdentity UserClaims)
    {
        // var jwt = new JwtSecurityToken(issuer: "", audience: "", claims: null, notBefore: null, expires: null, signingCredentials: null);
        // string tkn = new JwtSecurityTokenHandler().WriteToken(jwt);

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        // SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor(UserClaims));
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptor(UserClaims));
        return tokenHandler.WriteToken(token);
    }

    public string CreateTokenEncrypted(ClaimsIdentity UserClaims)
    {
        // var jwt = new JwtSecurityToken(issuer: "", audience: "", claims: null, notBefore: null, expires: null, signingCredentials: null);
        // string tkn = new JwtSecurityTokenHandler().WriteToken(jwt);

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        // SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor(UserClaims));
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptorEncrypted(UserClaims));
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string Token, out string Message, out SecurityToken? JWT, out ClaimsPrincipal? claimsPrincipal)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal user = tokenHandler.ValidateToken(Token, GetTokenValidationParameters(), out JWT);
            claimsPrincipal = user;
            Message = "Token is validated";
            return true;
        }
        catch (Exception ex)
        {
            Message = $"{ex.Message}";
            JWT = null;
            claimsPrincipal = null;
            return false;
        }
    }

    public bool ValidateTokenEncrypted(string Token, out string Message, out SecurityToken? JWT, out ClaimsPrincipal? claimsPrincipal)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal user = tokenHandler.ValidateToken(Token, GetTokenValidationParametersEncrypted(), out JWT);
            Message = "Token is validated";
            claimsPrincipal = user;
            return true;
        }
        catch (Exception ex)
        {
            Message = $"{ex.Message}";
            JWT = null;
            claimsPrincipal = null;
            return false;
        }
    }
}