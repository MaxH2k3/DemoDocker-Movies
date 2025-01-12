using DemoDocker.Constants;
using DemoDocker.Enums;

namespace DemoDocker.Configuration
{
	public static class PolicyConfig
	{
		public static void AddPolicies(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(PolicyType.Moderator, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserRole.Admin).ToString(), ((int)UserRole.Staff).ToString()));
				options.AddPolicy(PolicyType.Admin, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserRole.Admin).ToString()));
				options.AddPolicy(PolicyType.Guest, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserRole.Unknown).ToString()));
			});
		}
	}
}
