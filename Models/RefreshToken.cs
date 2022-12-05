namespace Models.RefreshToken {
    class RefreshToken {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreatedAt { get; set; } = DateTime.Now;
        public DateTime TokenExpires { get; set; }

    }
}