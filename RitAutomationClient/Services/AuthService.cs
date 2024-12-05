using Microsoft.AspNetCore.Identity.Data;
using RitAutomationClient.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthService
{
    private readonly string ApiUrl = "https://localhost:7183/api/auth/login";

    public async Task<LoginUserResponse> LoginAsync(string email, string password)
    {
        var loginData = new { Email = email, Password = password };
        var jsonContent = JsonSerializer.Serialize(loginData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(ApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginUserResponse>(responseBody);
        }

        return null;
    }
}


