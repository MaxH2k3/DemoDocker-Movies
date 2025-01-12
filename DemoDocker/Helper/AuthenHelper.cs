using DemoDocker.Constants;
using DemoDocker.Dto;
using DemoDocker.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoDocker.Helper
{
	public class AuthenHelper
	{
		private readonly JWTSetting _jwtsetting;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthenHelper(IOptions<JWTSetting> jwtsetting, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_jwtsetting = jwtsetting.Value;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

		public string? GenerateAccessToken(User? user)
		{
			if (user == null)
			{
				return null;
			}

			var tokenhandler = new JwtSecurityTokenHandler();
			var tokenkey = Encoding.UTF8.GetBytes(_jwtsetting.SecurityKey!);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(
					new Claim[]
					{
						new Claim(UserClaimType.UserId, user.UserId.ToString()),
						new Claim(UserClaimType.Email, user.Email!),
						new Claim(UserClaimType.Role, user.Role!)
					}
				),
				Expires = DateTime.Now.AddMinutes((double)_jwtsetting.TokenExpiry!),
				Issuer = _jwtsetting.Issuer,
				Audience = _jwtsetting.Audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
			};
			var token = tokenhandler.CreateToken(tokenDescriptor);
			string finaltoken = tokenhandler.WriteToken(token);

			return finaltoken;
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		public string GenerateOTP()
		{
			var randomNumber = new byte[6];
			RandomNumberGenerator.Fill(randomNumber);
			var otp = BitConverter.ToUInt32(randomNumber, 0) % 1000000;
			return otp.ToString("D6");
		}

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		public void CreatePasswordHash(out byte[] passwordHash, out byte[] passwordSalt)
		{
			string password = PasswordGenerator.GenerateRandomPassword();
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}

		}

		public Guid GetUserId()
		{
			return _httpContextAccessor.HttpContext?.User.GetUserIdFromToken() ?? throw new Exception("Unauthorize!");
		}

		public string GetUserEmail()
		{
			return _httpContextAccessor.HttpContext?.User.GetEmailFromToken() ?? throw new Exception("Unauthorize!");
		}

		public int GetUserRole()
		{
			return _httpContextAccessor.HttpContext?.User.GetRoleFromToken() ?? throw new Exception("Unauthorize!");
		}

		public bool? CheckAuthentication()
		{
			return _httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated;
		}
	}
}
