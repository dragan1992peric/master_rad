﻿using System.Security.Cryptography;

namespace VTSMASTER.Server.Services.AuthService
{
	public class AuthService : IAuthService
	{
		private readonly DataContext _context;
        public AuthService(DataContext context)
        {
            _context = context;
        }

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
				responce.Data = "token";
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

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

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
	}
}
