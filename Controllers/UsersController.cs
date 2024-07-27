using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using MockAPI.AppData;
using MockAPI.AppData.DTOs;
using MockAPI.AppData.Entities;
using MockAPI.AppLib.Common;
using MockAPI.AppLib.Extensions;
using MockAPI.AppLib.Filters;
using MockAPI.AppLib.Services;

namespace MockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(AppDbContext appDbContext, ITokenService tokenService) : ControllerBase
    {
        [HttpGet] // https://localhost:5001/users
        [ActionLogFilter]
        public async Task<ApiResponse> GetUsers()
        {
            var r = HttpContext.Items[Literals.TraceId];

            var func = async () => await appDbContext.Users
                .Include(u => u.Address).ThenInclude(a => a!.County)
                .Include(u => u.Company)
                .Include(u => u.Roles)
                .ToListAsync();

            var result = await new ExecuterService().ExecuterAsync(async () => await func());

            return ApiResponses.Success(result.Data, result.Elapsed);
        }

        [HttpGet("{id:int}")] // https://localhost:5001/users/1
        [ActionLogFilter]
        public async Task<ApiResponse> GetUser([FromRoute(Name = "id")] int userId)
        {
            var func = async () => await appDbContext.Users
                .Include(u => u.Address).ThenInclude(a => a!.County)
                .Include(u => u.Company)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            var result = await new ExecuterService().ExecuterAsync(async () => await func());

            return ApiResponses.Success(result.Data, result.Elapsed);
        }

        [ActionLogFilter]
        [HttpPut("[action]")]
        public async Task<ApiResponse> Register(RegisterDto registerDto)
        {
            try
            {
                using var hmac = new HMACSHA512();
                
                User user = new User
                {
                    Name = registerDto.Name,
                    Surname = registerDto.Surname,
                    Username = registerDto.Email.ToLower(new System.Globalization.CultureInfo("tr-TR")),
                    Email = registerDto.Email,
                    PasswordHash = hmac.ComputeHash(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password))),
                    PasswordSalt = hmac.Key,
                    Roles = appDbContext.Roles.Where(r => r.RoleCode == "user").ToList()                    
                }; 

                appDbContext.Users.Add(user);                
                var result = await appDbContext.SaveChangesAsync();
                return ApiResponses.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponses.Exception(ex);
            }            
        }

        [ActionLogFilter]
        [HttpPost("[action]")]
        public async Task<ApiResponse> Login(LoginDto loginDto)
        {
            try
            {
                var user = await appDbContext.Users
                    .Include(u => u.Roles)
                    .Where(u => u.Username == loginDto.Username.ToLower(new System.Globalization.CultureInfo("tr-TR")))
                    .FirstOrDefaultAsync();

                if(user == null) return ApiResponses.Fail("Invalid user credentials");

                using var hmac = new HMACSHA512(user.PasswordSalt);

                var pwdhash = hmac.ComputeHash(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));

                for (int i = 0; i < pwdhash.Length; i++)
                {
                    if (pwdhash[i] != user.PasswordHash[i]) return ApiResponses.Fail("Invalid user credentials"); ;
                }

                List<Claim> claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Username),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim("ClientIp", HttpContext.GetClientIP())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
                
                var token = tokenService.CreateToken(claimsIdentity);

                return ApiResponses.Success(new {
                    Username = loginDto.Username,
                    Token = token
                });               
            }
            catch (Exception ex)
            {
                return ApiResponses.Exception(ex);
            }
        }
    }
}