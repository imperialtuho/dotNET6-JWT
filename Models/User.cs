namespace dotNET_JWT.Models.Users {
    public class User {
        public string Name { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreatedAt{ get; set; } = DateTime.Now;
        public DateTime TokenExpires{ get; set; }

    }
}