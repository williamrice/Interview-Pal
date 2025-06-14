using Backend.Models;

namespace Backend.Services
{
	public interface IAuthService
	{
		Task<ServiceResult> RegisterAsync(RegisterDto model);
		Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model);
		Task<ServiceResult<object>> GetProfileAsync(string userId);
	}
}
