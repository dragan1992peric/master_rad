﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Security.Claims;

namespace VTSMASTER.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
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
	}
}
