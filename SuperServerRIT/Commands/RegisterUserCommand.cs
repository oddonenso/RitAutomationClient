using MediatR;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class RegisterUserCommand : IRequest<string>
    {
        public UserRegistrationDto UserRegistrationDto { get; set; }

        public RegisterUserCommand(UserRegistrationDto userRegistrationDto)
        {
            UserRegistrationDto = userRegistrationDto;
        }
    }
}
