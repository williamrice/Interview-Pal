using Backend.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Backend.Repositories
{
	public class AuthRepository : IAuthRepository
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
		{
			return await _userManager.CreateAsync(user, password);
		}

		public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}

		public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
		{
			return await _signInManager.CheckPasswordSignInAsync(user, password, false) == SignInResult.Success;
		}

		public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
		{
			return await _userManager.GetRolesAsync(user);
		}

		public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
		{
			return await _userManager.FindByIdAsync(userId);
		}
	}
}
