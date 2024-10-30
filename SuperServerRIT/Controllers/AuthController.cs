using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
        {
            var command = new RegisterUserCommand(request);
            var token = await _mediator.Send(command);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var command = new LoginUserCommand { Email = request.Email, Password = request.Password };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] JsonPatchDocument<User> patch)
        {
            var command = new UpdateUserCommand { UserId = id, PatchDocument = patch };
            var message = await _mediator.Send(command);
            return Ok(new { message });
        }
    }
}
