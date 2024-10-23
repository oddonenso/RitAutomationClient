using MediatR;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Tables;
using SuperServerRIT.Services;
using SuperServerRIT.Commands;
using System.Security.Cryptography;

namespace SuperServerRIT.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly Connection _connection;
        private readonly JwtService _jwtService;

        public RefreshTokenHandler(Connection connection, JwtService jwtService)
        {
            _connection = connection;
            _jwtService = jwtService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _connection.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new Exception("Refresh Token недействителен");
            }

            var token = _jwtService.GenerateJWT(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
            await _connection.SaveChangesAsync();

            return new RefreshTokenResponse
            {
                Token = token,
                NewRefreshToken = newRefreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
