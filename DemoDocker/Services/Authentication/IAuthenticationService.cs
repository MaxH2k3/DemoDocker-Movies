using DemoDocker.Dto;
using DemoDocker.Models;

namespace DemoDocker.Services.Authentication
{
	public interface IAuthenticationService
	{
		Task<APIResponse> Register(RegisterUser registerUser);
		Task<APIResponse> Login(LoginUser loginUser);
		Task<APIResponse> Logout();
		Task<APIResponse> VerifyAccount(string otp, string username);
		Task<APIResponse> ResendOTP(string username);
		Task<APIResponse> RefreshToken(string accessToken);
	}
}