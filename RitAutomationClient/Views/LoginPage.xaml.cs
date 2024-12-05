using RitAutomationClient.Controller;
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
        private readonly LoginController _controller;
        private readonly JwtService _jwtService;

        public string Email { get; set; }
        public string Password { get; set; }

        public LoginPage(JwtService jwtService)
        {
            InitializeComponent();
            _jwtService = jwtService; 
            var authService = new AuthService();
            _controller = new LoginController(this, authService);

            this.DataContext = this;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessageTextBlock.Text = "Заполните все поля.";
                return;
            }

            // Вызываем логин через контроллер
            await _controller.Login(Email, Password);
        }

        public void UpdateStatusMessage(string message)
        {
            StatusMessageTextBlock.Text = message;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;  
        }

        private void RegistrationLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage(_jwtService));
        }

        public void NavigateToSensorEmulator()
        {
            NavigationService.Navigate(new SensorEmulatorPage(_jwtService));
        }
    }


}
