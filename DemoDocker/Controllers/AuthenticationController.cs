using DemoDocker.Dto;
using DemoDocker.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FAI.API.Controllers
{
	[Route("api/v1/authentication")]
	[ApiController]
	public class AuthenticationController : Controller
	{
		private readonly IAuthenticationService _authenticationService;

		public AuthenticationController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
		{
			var response = await _authenticationService.Register(registerUser);
			
			if(response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
		{
			var response = await _authenticationService.Login(loginUser);

			if (response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpGet("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			var response = await _authenticationService.Logout();

			if (response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpGet("verify-account")]
		public async Task<IActionResult> VerifyAccount(string otp, string username)
		{
			var response = await _authenticationService.VerifyAccount(otp, username);

			if (response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpPost("resend-otp")]
		public async Task<IActionResult> ResendOTP([FromBody] string username)
		{
			var response = await _authenticationService.ResendOTP(username);

			if (response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] string accessToken)
		{
			var response = await _authenticationService.RefreshToken(accessToken);

			if (response.Status == HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

	}
}
