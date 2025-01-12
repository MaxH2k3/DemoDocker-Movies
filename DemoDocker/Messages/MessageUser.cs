namespace DemoDocker.Messages
{
	public class MessageUser
	{
		//User Message response
		public const string UserNotFound = "User not found!";
		public const string LoginFailed = "Username or password incorrect!";
		public const string LoginSuccess = "Login successfully";
		public const string RegisterSuccess = "User registered successfully";
		public const string RegisterFailed = "Failed to register user";
		public const string LogoutSuccess = "Logged out successfully";

		public const string OTPSuccess = "OTP sent successfully";
		public const string OTPExpired = "OTP expired";
		public const string OTPNotValid = "OTP not valid";
		public const string PhoneExisted = "Phone has already existed";
		public const string UserBlocked = "User has been blocked";
		public const string UserDeleted = "User has been deleted";
		public const string UserActive = "User has been activated";

		public const string ResetPasswordSuccess = "Reset password successfully";
		public const string ResetPasswordFailed = "Failed to reset password";

		//Authentication Message response
		public const string TokenInvalid = "Invalid token";
		public const string TokenExpired = "Tokens expired";
		public const string TokenRefreshSuccess = "Token refreshed successfully";
		public const string OtpInvalid = "OTP invalid";

		//Role Message response
		public const string UserIsNotCustomer = "User is not customer";
		public const string UserIsNotBrand = "User is not brand";

		public const string VerifySuccess = "Verify successfully";
	}
}
