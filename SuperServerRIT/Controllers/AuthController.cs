using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using SuperServerRIT.Commands;
using Microsoft.AspNetCore.JsonPatch;
using Data.Tables;
using Microsoft.AspNetCore.Authorization;
using SuperServerRIT.Model; 

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

        // Обработка входа пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            try
            {
                var command = new LoginUserCommand
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _mediator.Send(command);

                var response = new LoginResponseDto
                {
                    Message = result.Message,
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        // Обновление пользователя
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] JsonPatchDocument<User> patch)
        {
            try
            {
                var command = new UpdateUserCommand { UserId = id, PatchDocument = patch };
                var message = await _mediator.Send(command);

                var response = new UpdateUserResponseDto
                {
                    Message = message,
                    UserId = id
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        // Обработка Refresh Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
        {
            try
            {
                var command = new RefreshTokenCommand { RefreshToken = request.RefreshToken };
                var result = await _mediator.Send(command);

                var response = new RefreshTokenResponseDto
                {
                    Message = "Токен обновлен",
                    NewToken = result.Token,
                    NewRefreshToken = result.NewRefreshToken
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        
        [Authorize]
        [HttpGet("secure-endpoint")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "Это защищенные данные" });
        }
    }
}
