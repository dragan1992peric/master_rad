﻿namespace VTSMASTER.Client.Services.AuthService
{
	public class AuthService : IAuthService
	{
		private readonly HttpClient _http;
		private readonly AuthenticationStateProvider _authStateProvider;
        public AuthService(HttpClient http, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
			_authStateProvider = authStateProvider;
        }

		public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
		{
			var result = await _http.PostAsJsonAsync("api/auth/change-password", request.Password);
			return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
		}
		public async Task<User> ForgotPassword(string email)
		{
			var result = await _http.PostAsJsonAsync("api/auth/forgot-password", email);
			return await result.Content.ReadFromJsonAsync<User>();
		}

		public async Task<bool> IsUserAuthenticated()
        {
			return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task<ServiceResponse<string>> Login(UserLogin request)
		{
			var result = await _http.PostAsJsonAsync("api/auth/login", request);
			return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
		}

		public async Task<ServiceResponse<int>> Register(UserRegister request)
		{
			var result = await _http.PostAsJsonAsync("api/auth/register", request);
			return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
		}

		public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordRequest request)
		{
			var result = await _http.PostAsJsonAsync("api/auth/reset-password", request.Token);
			return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
		}
	}
}
