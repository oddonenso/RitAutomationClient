namespace SuperServerRIT.Commands
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string NewRefreshToken { get; set; } = string.Empty;
    }
}
