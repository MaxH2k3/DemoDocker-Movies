using Microsoft.Extensions.Configuration;

namespace DemoDocker.Dto
{
	public class JWTSetting
	{
		public string SecurityKey { get; set; } = null!;
		public string Issuer { get; set; } = null!;
		public string Audience { get; set; } = null!;
		public int TokenExpiry { get; set; }

		public JWTSetting()
		{
			GetSettingConfig();
		}

		private void GetSettingConfig()
		{
			IConfiguration config = new ConfigurationBuilder()

			.SetBasePath(Directory.GetCurrentDirectory())

			.AddJsonFile("appsettings.json", true, true)

			.Build();

			this.SecurityKey = config["JWTSetting:Securitykey"]!;
			this.Issuer = config["JWTSetting:Issuer"]!;
			this.Audience = config["JWTSetting:Audience"]!;
			this.TokenExpiry = Convert.ToInt32(config["JWTSetting:TokenExpiry"]);

		}
	}


}
