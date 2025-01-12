namespace DemoDocker.Models
{
	public class User
	{
		public Guid UserId { get; set; }
		public string? Email { get; set; }
		public string? Username { get; set; }
		public byte[]? Password { get; set; }
		public byte[]? PasswordSalt { get; set; }
		public string? Role { get; set; }
		public string? Status { get; set; }
		public DateTime? DateCreated { get; set; }
		public string? RefreshToken { get; set; }
		public string? AccessToken { get; set; }
		public DateTime? TokenExpired { get; set; }
		public string? Otp { get; set; }
	}
}
