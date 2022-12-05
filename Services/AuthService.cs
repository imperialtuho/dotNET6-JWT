// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using System.Text;
// using dotNET_JWT.Models.UserDto;
// using dotNET_JWT.Models.Users;
// using Microsoft.IdentityModel.Tokens;

// namespace dotNET_JWT.Services {
    
//     public class AuthService : IAuthService {

//         private readonly IConfiguration _configuration;
//         private static readonly User _user = new User();
//         private readonly string privateKey = "this is private key for JWT authentication";
//         public AuthService(IConfiguration configuration){
//             _configuration = configuration;
//         }
        
//         public string CreateToken (User user) {
//             List<Claim> claims = new List<Claim> {
//                 new Claim(ClaimTypes.Name, user.Name),
//                 new Claim(ClaimTypes.Role, "Administrator")
//             };
//             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
//             var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
//             var token = new JwtSecurityToken(
//                 claims  : claims,
//                 expires : DateTime.Now.AddDays(1),
//                 signingCredentials : credentials
//             );
//             var jwt = new JwtSecurityTokenHandler().WriteToken(token);
//             return jwt;
//         }
//         public void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt){
//             using (var hmac = new HMACSHA512()){
//                 passwordSalt = hmac.Key;
//                 passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
//             }
//         }

//         public bool IsPasswordHashValid(string password, byte[] passwordHash, byte[] passwordSalt) {
//             using(var hmac = new HMACSHA512(_user.PasswordSalt)){
//                 var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
//                 return computedHash.SequenceEqual(passwordHash);
//             }
//         }

//         public object Register(UserDto request) {
//             CreatePasswordHash(request.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
//             _user.Name = request.Name;
//             _user.PasswordHash = PasswordHash;
//             _user.PasswordSalt = PasswordSalt;
//             return _user;
//         }
        
//     }
// }