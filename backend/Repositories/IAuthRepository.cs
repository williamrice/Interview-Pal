using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Repositories
{
	public interface IAuthRepository
	{
		Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
		Task<ApplicationUser?> FindUserByEmailAsync(string email);
		Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
		Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
		Task<ApplicationUser?> FindUserByIdAsync(string userId);
	}
}
