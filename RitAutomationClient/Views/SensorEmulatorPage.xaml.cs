using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;

namespace RitAutomationClient.Views
{
    public partial class SensorEmulatorPage : Page
    {
        public SensorEmulatorPage()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            
            var temperature = TemperatureTextBox.Text;
            var pressure = PressureTextBox.Text;
            var latitude = LatitudeTextBox.Text;
            var longitude = LongitudeTextBox.Text;

            
            if (string.IsNullOrWhiteSpace(temperature) ||
                string.IsNullOrWhiteSpace(pressure) ||
                string.IsNullOrWhiteSpace(latitude) ||
                string.IsNullOrWhiteSpace(longitude))
            {
                StatusMessageTextBlock.Text = "Заполните все поля.";
                return;
            }

            
            StatusMessageTextBlock.Text = "Данные успешно отправлены!";

            
            MessageBox.Show($"Температура: {temperature} °C\nДавление: {pressure} кПа\nКоординаты: {latitude}, {longitude}");
        }

        private void SensorMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            var mouseLocation = e.GetPosition(SensorMap);
            Location location = SensorMap.ViewportPointToLocation(mouseLocation);

           
            LatitudeTextBox.Text = location.Latitude.ToString();
            LongitudeTextBox.Text = location.Longitude.ToString();

            
            e.Handled = true;
        }
    }
}
