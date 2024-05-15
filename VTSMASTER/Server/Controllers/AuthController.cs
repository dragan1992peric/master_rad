using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Security.Claims;
using System.Security.Cryptography;

namespace VTSMASTER.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly DataContext _context;
		public AuthController(IAuthService authService, DataContext context)
		{
			_authService = authService;
			_context = context;
		}

		[HttpPost("register")]
		public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister request)
		{
			var response = await _authService.Register(
				new User 
				{ 
					Email = request.Email 
				}, 
				request.Password);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPost("login")]
		public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin request)
		{
			var responce = await _authService.Login(request.Email, request.Password);
			if (!responce.Success)
			{
				return BadRequest(responce);
			}
			return Ok(responce);
		}

		[HttpPost("change-password"), Authorize]
		public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string newPassword)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var responce = await _authService.ChangePassword(int.Parse(userId), newPassword);

			if (!responce.Success)
			{
				return BadRequest(responce);
			}

			return Ok(responce);
		}

		[HttpPost]
		public IActionResult SentEmail(User user)
		{
			_authService.SendEmail(user);

			return Ok();
		}

		[HttpGet ("verify")]
		public async Task<IActionResult> Verify(string token)
		{
			var user = await _authService.Verify(token);
			if(user == null)
			{
				return BadRequest("Invalid token");
			}
			user.VerifiedAt = DateTime.Now;
			await _context.SaveChangesAsync();

			return Ok("Korisnik verifikovan");
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _authService.ForgotPassword(email);
			if (user == null)
			{
				return BadRequest("Invalid user");
			}
			user.PasswordResetTopken = _authService.CreateRandomToken();
			user.ResetTokenExpires = DateTime.Now.AddDays(1);
			await _context.SaveChangesAsync();
			_authService.SendEmailForgot(user);

			return Ok("Poslat token za reset sifre");
		}
		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
		{
			var user = await _authService.ResetPassword(request);
			if (user == null || user.ResetTokenExpires < DateTime.Now)
			{
				return BadRequest("Invalid token");
			}

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.PasswordResetTopken = null;
			user.ResetTokenExpires = null;

			await _context.SaveChangesAsync();

			return Ok("Uspesno resetovana sifra");
		}
	}
}
