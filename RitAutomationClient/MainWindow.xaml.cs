using System.Windows;
using RitAutomationClient.Views;

namespace RitAutomationClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }
    }
}
