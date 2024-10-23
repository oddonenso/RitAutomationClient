using MediatR;

namespace SuperServerRIT.Commands
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;

    }
}
