using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers
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
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			var result = await _authService.RegisterAsync(model);
			if (!result.IsSuccess)
			{
				return BadRequest(new { Errors = result.Errors });
			}

			return Ok(new { Message = "User registered successfully" });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			var result = await _authService.LoginAsync(model);
			if (!result.IsSuccess)
			{
				return Unauthorized(new { Message = "Invalid credentials" });
			}

			return Ok(result.Data);
		}

		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new { Message = "Invalid user" });
			}

			var result = await _authService.GetProfileAsync(userId);
			if (!result.IsSuccess)
			{
				return NotFound(new { Message = "User not found" });
			}

			return Ok(result.Data);
		}
	}
}
