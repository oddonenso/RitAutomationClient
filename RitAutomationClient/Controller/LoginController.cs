using RitAutomationClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RitAutomationClient.Controller
{
    public class LoginController
    {
        private readonly AuthService _authService;
        private readonly LoginPage _view;

        public LoginController(LoginPage view, AuthService authService)
        {
            _view = view;
            _authService = authService;
        }

        public async Task Login(string email, string password)
        {
            var loginResponse = await _authService.LoginAsync(email, password);

            if (loginResponse != null)
            {
                _view.UpdateStatusMessage("Вход выполнен успешно!");
                _view.NavigateToSensorEmulator();
            }
            else
            {
                _view.UpdateStatusMessage("Неверный email или пароль.");
            }
        }
    }

}
