using MailKit.Security;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;

namespace VTSMASTER.Server.Services.AuthService
{
	public class AuthService : IAuthService
	{
		private readonly DataContext _context;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

		public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

		public string GetUserEmail() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

		public async Task<ServiceResponse<string>> Login(string email, string password)
		{
			var responce = new ServiceResponse<string>();
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
			if (user == null)
			{
				responce.Success = false;
				responce.Message = "Nema takvoga ovde";
			}
			else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			{
				responce.Success = false;
				responce.Message = "Pogreshna sifra";
			}
			else
			{
				responce.Data = CreateToken(user);
			}
			
		
			return responce;
		}

		public async Task<ServiceResponse<int>> Register(User user, string password)
		{
			if(await UserExists(user.Email))
			{
				return new ServiceResponse<int> 
				{ 
					Success = false, 
					Message="Већ постоји налог" 
				};
			}

			CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.VerificationToken = CreateRandomToken();

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			SendEmail(user);

			return new ServiceResponse<int> { Data = user.Id, Message = "Регистрација УСПЕЛА!" };
		}

		public async Task<bool> UserExists(string email)
		{
			if(await _context.Users.AnyAsync(user => user.Email.ToLower()
				.Equals(email.ToLower())))
			{
				return true;
			}
			return false;
		}

		private void CreatePasswordHash(string  password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using(var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passswordSalt)
		{
			using(var hmac = new HMACSHA512(passswordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}

		public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
		{
			var user = await _context.Users.FindAsync(userId);
			if(user == null)
			{
				return new ServiceResponse<bool>
				{
					Success = false,
					Message = "корисник није наџен"
				};
			}

			CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			await _context.SaveChangesAsync();

			return new ServiceResponse<bool>
			{
				Data = true,
				Message = "Променио си шифру"
			};
		}

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
		public async Task<User> Verify(string token)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
		}
		public async Task<User> ForgotPassword(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
		public async Task<User> ResetPassword(ResetPasswordRequest request)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetTopken == request.Token);
		}

		public void SendEmail(User user)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("elijah.harber87@ethereal.email"));
			email.To.Add(MailboxAddress.Parse(user.Email));
			email.Subject = "Честитамо на успешној регистрацији!! БРАВОО!!!!!!";
			email.Body = new TextPart(TextFormat.Html) { Text = "Честитамо! ти си наш нови:" + user.Role + "tvoj link za verifikaciju je https://localhost:7081/api/Auth/verify?token="+user.VerificationToken };

			using var smtp = new SmtpClient();
			smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("elijah.harber87@ethereal.email", "yywfzv2Aaer1XamKdz");
			smtp.Send(email);
			smtp.Disconnect(true); ;
		}

		public string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}
}
