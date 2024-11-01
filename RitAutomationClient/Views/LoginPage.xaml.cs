using SuperServerRIT.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace RitAutomationClient.Views
{
    public partial class LoginPage : Page
    {
        private static readonly string ApiUrl = "https://localhost:7183/api/auth/login";
        private int loginAttempts = 0;
        private bool isLocked = false;
        private readonly JwtService _jwtService;


        public LoginPage(JwtService jwtService)
        {
            InitializeComponent();
            _jwtService = jwtService;
        }

        private void RegistrationLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage(_jwtService));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка состояния блокировки
            if (isLocked)
            {
                StatusMessageTextBlock.Text = "Попробуйте снова позже.";
                return;
            }

            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                StatusMessageTextBlock.Text = "Заполните все поля.";
                return;
            }

            var loginData = new
            {
                Email = email,
                Password = password
            };

            var jsonContent = JsonSerializer.Serialize(loginData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(ApiUrl, content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ от сервера: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    loginAttempts = 0;
                    var loginResponse = JsonSerializer.Deserialize<LoginUserResponse>(responseBody);

                    if (loginResponse != null)
                    {
                        StatusMessageTextBlock.Text = "Вход выполнен успешно!";
                        NavigationService.Navigate(new SensorEmulatorPage(_jwtService));
                    }
                    else
                    {
                        StatusMessageTextBlock.Text = "Ошибка: Неверный формат ответа от сервера.";
                    }
                }
                else
                {
                    await HandleFailedLoginAttempt(response);
                }
            }
            catch (JsonException jsonEx)
            {
                StatusMessageTextBlock.Text = $"Ошибка JSON: {jsonEx.ToString()}";
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Ошибка авторизации: {ex.ToString()}";
            }
        }



        private async Task HandleFailedLoginAttempt(HttpResponseMessage response)
        {
            // Увеличиваем счетчик попыток
            loginAttempts++;

            var errorResponse = await response.Content.ReadAsStringAsync();
            StatusMessageTextBlock.Text = $"Ошибка авторизации: {errorResponse}";

            // Если достигли 3 попыток, блокируем поля
            if (loginAttempts >= 3)
            {
                StatusMessageTextBlock.Text = "Вы превысили количество попыток. Попробуйте снова через 10 секунд.";
                SetControlsEnabled(false);

                // Сброс счетчика и разблокировка через 10 секунд
                await Task.Delay(TimeSpan.FromSeconds(10));
                loginAttempts = 0;
                SetControlsEnabled(true);
                StatusMessageTextBlock.Text = string.Empty;
            }
        }

        // Метод для активации/деактивации полей и кнопок
        private void SetControlsEnabled(bool isEnabled)
        {
            EmailTextBox.IsEnabled = isEnabled;
            PasswordBox.IsEnabled = isEnabled;
            LoginButton.IsEnabled = isEnabled;
        }
    }
}
