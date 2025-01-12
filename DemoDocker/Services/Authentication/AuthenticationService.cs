using AutoMapper;
using DemoDocker.Dto;
using DemoDocker.Enums;
using DemoDocker.Helper;
using DemoDocker.Messages;
using DemoDocker.Models;
using DemoDocker.Repositories.UnitOfWork;
using System.Net;

namespace DemoDocker.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly AuthenHelper _authenHelper;
		private readonly IMapper _mapper;

		public AuthenticationService(AuthenHelper authenHelper,
			IMapper mapper, IUnitOfWork unitOfWork)
		{
			_authenHelper = authenHelper;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public async Task<APIResponse> Register(RegisterUser registerUser)
		{
			var isExisted = await _unitOfWork.UserRepository.IsExisted(registerUser.Email!, registerUser.Username!);

			if(isExisted)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = "Email or username is existed"
				};
			}

			// Create password hash and salt
			_authenHelper.CreatePasswordHash(registerUser.Password!, out byte[] passwordHash, out byte[] passwordSalt);

			// Create user
			var baseUser = new User()
			{
				UserId = Guid.NewGuid(),	
				DateCreated = DateTime.Now,
				Role = registerUser.Role,
				Status = AccountStatus.NotVerify.ToString(),
				Password = passwordHash,
				PasswordSalt = passwordSalt,
				Email = registerUser.Email,
				Username = registerUser.Username
			};

			// Save otp and information user to redis cache
			baseUser.Otp = _authenHelper.GenerateOTP();

			await _unitOfWork.UserRepository.Add(baseUser);

			if(!await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageCommon.ServerError
				};
			}

			return new APIResponse()
			{
				Status = HttpStatusCode.OK,
				Message = MessageUser.RegisterSuccess,
				Data = new
				{
					Username = baseUser.Username,
					Email = baseUser.Email,
					OTP = baseUser.Otp
				}
			};
		}
		
		public async Task<APIResponse> Login(LoginUser loginUser)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(loginUser.Username);

			if (user == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			} else if (!_authenHelper.VerifyPasswordHash(loginUser.Password, user.Password!, user.PasswordSalt!))
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.LoginFailed
				};
			} else if (user.Status!.Equals(AccountStatus.Blocked.ToString()))
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.Forbidden,
					Message = MessageUser.UserBlocked
				};
			}

			var userLogin = _mapper.Map<UserLoginResponse>(user);

			userLogin.AccessToken = _authenHelper.GenerateAccessToken(user);

			if (userLogin.AccessToken == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			user.AccessToken = userLogin.AccessToken;
			user.RefreshToken = _authenHelper.GenerateRefreshToken();
			user.TokenExpired = DateTime.Now.AddDays(7);

			await _unitOfWork.UserRepository.Update(user);
			if(!await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			return new APIResponse()
			{
				Status = HttpStatusCode.OK,
				Message = MessageUser.LoginSuccess,
				Data = userLogin
			};
		}

		public async Task<APIResponse> Logout()
		{
			var userId = _authenHelper.GetUserId();
			var user = await _unitOfWork.UserRepository.GetById(userId);

			if (user == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			user.RefreshToken = null;
			user.TokenExpired = null;
			user.AccessToken = null;

			await _unitOfWork.UserRepository.Update(user);
			if (!await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			return new APIResponse()
			{
				Status = HttpStatusCode.OK,
				Message = MessageUser.LogoutSuccess
			};
		}
	
		public async Task<APIResponse> VerifyAccount(string otp, string username)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(username);

			if (user == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			if(user.Status!.Equals(AccountStatus.Active.ToString()))
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.UserActive
				};
			} else if(!user.Otp!.Equals(otp))
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.OTPNotValid
				};
			}

			user.Status = AccountStatus.Active.ToString();
			user.Otp = null;

			await _unitOfWork.UserRepository.Update(user);

			if (!await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			return new APIResponse()
			{
				Status = HttpStatusCode.OK,
				Message = MessageUser.VerifySuccess
			};
		}
	
		public async Task<APIResponse> ResendOTP(string username)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(username);

			if (user == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			string otp = _authenHelper.GenerateOTP();

			return new APIResponse()
			{
				Status = HttpStatusCode.OK,
				Message = MessageUser.OTPSuccess,
				Data = otp
			};
		}
	
		public async Task<APIResponse> RefreshToken(string accessToken)
		{
			var userId = _authenHelper.GetUserId();
			var user = await _unitOfWork.UserRepository.GetById(userId);

			if(user == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			if(user.TokenExpired < DateTime.Now)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.Unauthorized,
					Message = MessageUser.TokenExpired
				};
			} else if(user.RefreshToken == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.TokenInvalid
				};
			} else if(user.AccessToken != accessToken)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.TokenInvalid
				};
			}

			user.AccessToken = _authenHelper.GenerateAccessToken(user);

			if (user.AccessToken == null)
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			await _unitOfWork.UserRepository.Update(user);

			if (await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = HttpStatusCode.OK,
					Message = MessageUser.TokenRefreshSuccess,
					Data = user.AccessToken
				};
			}

			return new APIResponse()
			{
				Status = HttpStatusCode.InternalServerError,
				Message = MessageCommon.ServerError
			};
		}
	}
}
