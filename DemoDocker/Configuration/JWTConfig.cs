using DemoDocker.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DemoDocker.Configuration
{
	public static class JWTConfig
	{
		public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("JWTSetting").Get<JWTSetting>();

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = jwtSettings?.Issuer,
					ValidAudience = jwtSettings?.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey!)),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true
				};
			});

		}
	}
}
