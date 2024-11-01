using System.Windows;
using RitAutomationClient.Views;
using Microsoft.Extensions.Configuration;
using SuperServerRIT.Services;

namespace RitAutomationClient
{
    public partial class MainWindow : Window
    {
        private readonly JwtService _jwtService;

        public MainWindow()
        {
            InitializeComponent();

            
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") 
                .Build();


            _jwtService = new JwtService(config);
            MainFrame.Navigate(new LoginPage(_jwtService));
        }
    }
}
