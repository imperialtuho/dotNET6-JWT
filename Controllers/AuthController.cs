using dotNET_JWT.Models.UserDto;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using dotNET_JWT.Models.Users;
using Microsoft.IdentityModel.Tokens;
namespace dotNET_JWT.Controllers {
    [Route("[controller]")]
    [ApiController]
        public class AuthController : ControllerBase {
            private readonly IConfiguration _configuration;
            public AuthController(IConfiguration configuration) {
                _configuration = configuration;
            }

            private static readonly User _user = new User();
            private readonly string privateKey = "this is private key for JWT authentication";
            
            private string CreateToken (User user) {
                List<Claim> claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(
                    claims  : claims,
                    expires : DateTime.Now.AddDays(1),
                    signingCredentials : credentials
                );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;
            }
            private void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt){
                using (var hmac = new HMACSHA512()){
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
            }

            private bool IsPasswordHashValid(string password, byte[] passwordHash, byte[] passwordSalt) {
                using(var hmac = new HMACSHA512(_user.PasswordSalt)){
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return computedHash.SequenceEqual(passwordHash);
                }
            }
        [HttpPost("register")]
        public ActionResult Register(UserDto request) {
            CreatePasswordHash(request.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            _user.Name = request.Name;
            _user.PasswordHash = PasswordHash;
            _user.PasswordSalt = PasswordSalt;
            return Ok(_user);
        }
        [HttpPost("login")]
        public ActionResult Login(UserDto request){
            if(_user.Name != request.Name){
                return BadRequest("User not found");
            }
            if(!IsPasswordHashValid(request.Password, _user.PasswordHash, _user.PasswordSalt)){
                return BadRequest("Wrong password");
            }        
            string token = CreateToken(_user);
            return Ok(token);
        }
    }
}