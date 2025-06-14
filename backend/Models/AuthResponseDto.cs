namespace Backend.Services
{
	public class AuthResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string NickName { get; set; } = string.Empty;
	}
}

