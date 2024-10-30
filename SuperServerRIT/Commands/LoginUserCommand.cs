using MediatR;

namespace SuperServerRIT.Commands
{
    public class LoginUserCommand : IRequest<LoginUserResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
