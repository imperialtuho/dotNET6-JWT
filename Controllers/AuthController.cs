using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
//User defined
using Models.RefreshToken;
using dotNET_JWT.Models.Users;
using dotNET_JWT.Models.UserDto;

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
            
        private string CreateToken(User user) {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims:claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials:credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
            private void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
                using (var hmac = new HMACSHA512()) {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
            }

            private bool IsPasswordHashValid(string password, byte[] passwordHash, byte[] passwordSalt) {
                using(var hmac = new HMACSHA512(_user.PasswordSalt)) {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return computedHash.SequenceEqual(passwordHash);
                }
            }
        private RefreshToken GenerateRefreshToken() {
            var refreshToken = new RefreshToken{
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TokenExpires = DateTime.Now.AddDays(2),
                TokenCreatedAt = DateTime.Now
            };
            return refreshToken;
        }
        private void SetRefreshToken(RefreshToken refreshToken) {
            var cookieOptions = new CookieOptions {
                HttpOnly = true,
                Expires = refreshToken.TokenExpires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            _user.RefreshToken = refreshToken.Token;
            _user.TokenCreatedAt = refreshToken.TokenCreatedAt;
            _user.TokenExpires = refreshToken.TokenExpires;
        }    

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request) {
            CreatePasswordHash(request.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            _user.Name = request.Name;
            _user.PasswordHash = PasswordHash;
            _user.PasswordSalt = PasswordSalt;
            return Ok(_user);
        }
        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request){
            if(_user.Name != request.Name){
                return BadRequest("User not found");
            }
            if(!IsPasswordHashValid(request.Password, _user.PasswordHash, _user.PasswordSalt)){
                return BadRequest("Wrong password");
            }        
            string token = CreateToken(_user);
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            return Ok(token);
        }
        [HttpPost("refresh-token")]
        public ActionResult<string> RefreshToken() {
            var refreshToken = Request.Cookies["refreshToken"];

            if(!_user.RefreshToken.Equals(refreshToken)) {
                return Unauthorized("Invalid refresh token");
            }
            else if(_user.TokenExpires < DateTime.Now) {
                return Unauthorized("Token expired");
            }
            string token = CreateToken(_user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);
            return Ok(token);
        }
    }
}