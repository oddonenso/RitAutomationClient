using Microsoft.AspNetCore.Identity.Data;
using RitAutomationClient.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthService
{
    private readonly HttpClient _client;
    private string _token;

    public AuthService()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7183/"); // Ваш URL сервера
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            _token = tokenResponse.Token;
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            return true;
        }

        return false;
    }

    public async Task<string> GetSecureDataAsync()
    {
        var response = await _client.GetAsync("secure-endpoint"); // Замените на защищенный endpoint

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        return "Ошибка доступа";
    }
}


