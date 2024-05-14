using Microsoft.AspNetCore.Mvc;

namespace VTSMASTER.Server.Services.AuthService
{
	public interface IAuthService
	{
		Task<ServiceResponse<int>> Register(User user, string password);
		Task<bool> UserExists(string email);
		Task<ServiceResponse<string>> Login(string email, string password);
		Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
		int GetUserId();
		string GetUserEmail();
		Task<User> GetUserByEmail(string email);
		Task<User> Verify(string token);
		Task<User> ForgotPassword(string email);
		Task<User> ResetPassword(ResetPasswordRequest request);
		void SendEmail(User user);
		string CreateRandomToken();
	}
}
