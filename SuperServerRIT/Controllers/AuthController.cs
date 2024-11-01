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

        /// <summary>
        /// Регистрирует нового пользователя.
        /// </summary>
        /// <param name="request">Данные для регистрации пользователя.</param>
        /// <returns>Токен для авторизации.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
        {
            var command = new RegisterUserCommand(request);
            var token = await _mediator.Send(command);
            return Ok(new { token });
        }

        /// <summary>
        /// Выполняет вход пользователя.
        /// </summary>
        /// <param name="request">Данные для входа пользователя.</param>
        /// <returns>Результат операции входа.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email и пароль не могут быть пустыми.");
            }

            var command = new LoginUserCommand { Email = request.Email, Password = request.Password };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Обновляет данные пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя для обновления.</param>
        /// <param name="patch">Документ для частичного обновления пользователя.</param>
        /// <returns>Сообщение о результате обновления.</returns>
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] JsonPatchDocument<User> patch)
        {
            var command = new UpdateUserCommand { UserId = id, PatchDocument = patch };
            var message = await _mediator.Send(command);
            return Ok(new { message });
        }
    }
}
