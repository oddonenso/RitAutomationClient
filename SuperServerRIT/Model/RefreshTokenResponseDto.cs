namespace SuperServerRIT.Model
{
    public class RefreshTokenResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string NewToken { get; set; } = string.Empty;
        public string NewRefreshToken { get; set; } = string.Empty;
    }
}
