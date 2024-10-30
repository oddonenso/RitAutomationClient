using System.Windows.Input;
using RitAutomationClient.Commands;
using RitAutomationClient.Models;


namespace RitAutomationClient.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly AuthService _authService = new AuthService();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async () =>
            {
                var registerDto = new RegisterUserDto
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    Password = this.Password
                };

                //await _authService.RegisterUserAsync(registerDto);
            });
        }
    }
}
