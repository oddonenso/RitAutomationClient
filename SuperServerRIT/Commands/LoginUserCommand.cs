using MediatR;

namespace SuperServerRIT.Commands
{
    public class LoginUserCommand : IRequest<LoginUserResponse>
    {
        public string Email { get; set; } = string.Empty; // Email пользователя
        public string Password { get; set; } = string.Empty; // Пароль пользователя
    }
}
