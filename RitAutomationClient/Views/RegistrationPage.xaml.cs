using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RitAutomationClient.Views
{
    public partial class RegistrationPage : Page
    {
        private static readonly string ApiUrl = "https://localhost:7183/api/auth/register"; // Замените на URL вашего API

        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем введенные данные пользователя
            var firstName = FirstNameTextBox.Text;
            var lastName = LastNameTextBox.Text;
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            // Проверяем корректность данных
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                StatusMessageTextBlock.Text = "Заполните все поля.";
                return;
            }

            if (password != confirmPassword)
            {
                StatusMessageTextBlock.Text = "Пароли не совпадают.";
                return;
            }

            // Создаем объект данных для отправки на сервер
            var registrationData = new
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var jsonContent = JsonSerializer.Serialize(registrationData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                using var httpClient = new HttpClient();

                // Отправляем POST-запрос на сервер
                var response = await httpClient.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация прошла успешно!");
                    StatusMessageTextBlock.Text = "Регистрация успешна!";

                    // Переход на страницу авторизации
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    // Обработка ошибок
                    var responseBody = await response.Content.ReadAsStringAsync();
                    StatusMessageTextBlock.Text = $"Ошибка регистрации: {responseBody}";
                }
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }
    }
}
