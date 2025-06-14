using Backend.Models;
using Backend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IConfiguration _configuration;

		public AuthService(IAuthRepository authRepository, IConfiguration configuration)
		{
			_authRepository = authRepository;
			_configuration = configuration;
		}

		public async Task<ServiceResult> RegisterAsync(RegisterDto model)
		{
			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				NickName = model.NickName
			};

			var result = await _authRepository.CreateUserAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return ServiceResult.Failure(result.Errors.Select(e => e.Description));
			}

			return ServiceResult.Success();
		}

		public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model)
		{
			var user = await _authRepository.FindUserByEmailAsync(model.Email);
			if (user == null || !await _authRepository.CheckPasswordAsync(user, model.Password))
			{
				return ServiceResult<AuthResponseDto>.Failure("Invalid credentials");
			}

			var token = await GenerateJwtToken(user);
			var response = new AuthResponseDto
			{
				Token = token,
				UserId = user.Id,
				Email = user.Email ?? string.Empty,
				NickName = user.NickName ?? string.Empty
			};

			return ServiceResult<AuthResponseDto>.Success(response);
		}

		public async Task<ServiceResult<object>> GetProfileAsync(string userId)
		{
			var user = await _authRepository.FindUserByIdAsync(userId);
			if (user == null)
			{
				return ServiceResult<object>.Failure("User not found");
			}

			var profile = new
			{
				user.Id,
				user.Email,
				user.NickName,
			};

			return ServiceResult<object>.Success(profile);
		}

		private async Task<string> GenerateJwtToken(ApplicationUser user)
		{
			var jwtSettings = _configuration.GetSection("JwtConfig");
			var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret Key is not configured"));

			var claims = new List<Claim>
						{
								new Claim(ClaimTypes.NameIdentifier, user.Id),
								new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
								new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
						};

			var roles = await _authRepository.GetUserRolesAsync(user);
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(2),
				Issuer = jwtSettings["Issuer"],
				Audience = jwtSettings["Audience"],
				SigningCredentials = new SigningCredentials(
							new SymmetricSecurityKey(key),
							SecurityAlgorithms.HmacSha256Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}

}
