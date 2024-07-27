using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MockAPI.AppLib.Common;
using MockAPI.AppLib.Services;

namespace MockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<TokenController> _logger;

        private List<Claim> userClaims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.NameId, Faker.Internet.UserName()),
            new Claim(JwtRegisteredClaimNames.GivenName, Faker.Name.FullName()),
            new Claim("Role", "admin"),
            new Claim("Role", "user")
        };

        public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IActionResult Create()
        {
            string token = _tokenService.CreateToken(new ClaimsIdentity(userClaims));

            _logger.LogInformation("Token Created");

            return Ok(ApiResponses.Success(token));
        }

        [HttpPost("[action]")]
        public IActionResult Validate([FromForm] string Token)
        {
            string message;
            SecurityToken? jwtToken;
            ClaimsPrincipal? claimsPrincipal;

            bool result = _tokenService.ValidateToken(Token, out message, out jwtToken, out claimsPrincipal);

            ClaimsIdentity? ci = claimsPrincipal?.Identity as ClaimsIdentity;

            if (result) return Ok(ApiResponses.Success(new
            {
                Message = message,
                Claims = ci?.Claims.Select(c => $"{c.Type.Substring(c.Type.LastIndexOf('/')+1) }: {c.Value}") .Aggregate((current, next) => $"{current}, {next}"),
                JWT = jwtToken
            }));
            else return Ok(ApiResponses.Fail(message));
        }

        [HttpGet("[action]")]
        public IActionResult CreateEncrypted()
        {
            string token = _tokenService.CreateTokenEncrypted(new ClaimsIdentity(userClaims));
            return Ok(ApiResponses.Success(token));
        }

        [HttpPost("[action]")]
        public IActionResult ValidateEncrypted([FromForm] string Token)
        {
            string message;
            SecurityToken? jwtToken;
            ClaimsPrincipal? claimsPrincipal;

            bool result = _tokenService.ValidateTokenEncrypted(Token, out message, out jwtToken, out claimsPrincipal);

            if (result) return Ok(ApiResponses.Success(new
            {
                Message = message,
                claimsPrincipal?.Claims,
                JWT = jwtToken
            }));
            else return Ok(ApiResponses.Fail(message));
        }
    }
}