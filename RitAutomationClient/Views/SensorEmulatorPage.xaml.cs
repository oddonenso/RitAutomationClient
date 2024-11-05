using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Maps.MapControl.WPF;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using SuperServerRIT.Services;

namespace RitAutomationClient.Views
{
    public partial class SensorEmulatorPage : Page
    {
        private readonly JwtService _jwtService;
        private Pushpin _pushpin;
        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("https://localhost:7183/api/") };


      
        private RabbitMqService RabbitMqService => _rabbitMqService ??= new RabbitMqService();
        private RabbitMqService _rabbitMqService;

        public SensorEmulatorPage(JwtService jwtService)
        {
            InitializeComponent();
            _jwtService = jwtService;
            LoadEquipmentList();
        }

        private async void LoadEquipmentList()
        {
            try
            {
                string token = _jwtService.GetTokenFromStorage();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.GetAsync("Equipment");

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Ошибка загрузки списка оборудования: " + response.StatusCode);
                    return;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var equipmentList = JsonSerializer.Deserialize<List<Equipment>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (equipmentList != null && equipmentList.Count > 0)
                {
                    // Очищаем предыдущие данные
                    EquipmentStatusComboBox.ItemsSource = null;
                    UpdateEquipmentComboBox.ItemsSource = null;
                    DeleteEquipmentComboBox.ItemsSource = null;

                    // Назначаем новый список
                    EquipmentStatusComboBox.ItemsSource = equipmentList;
                    UpdateEquipmentComboBox.ItemsSource = equipmentList;
                    DeleteEquipmentComboBox.ItemsSource = equipmentList;

                    EquipmentStatusComboBox.DisplayMemberPath = "Name";
                    UpdateEquipmentComboBox.DisplayMemberPath = "Name";
                    DeleteEquipmentComboBox.DisplayMemberPath = "Name";

                    EquipmentStatusComboBox.SelectedIndex = 0;
                    UpdateEquipmentComboBox.SelectedIndex = 0;
                    DeleteEquipmentComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }


        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddEquipmentNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddEquipmentTypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddEquipmentStatusTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            var newEquipment = new CreateEquipmentCommand
            {
                Name = AddEquipmentNameTextBox.Text.Trim(),
                Type = AddEquipmentTypeTextBox.Text.Trim(),
                Status = AddEquipmentStatusTextBox.Text.Trim()
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7183/api/");
                string token;

                try
                {
                    token = _jwtService.GetTokenFromStorage();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var content = new StringContent(JsonSerializer.Serialize(newEquipment), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("Equipment", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Оборудование успешно добавлено.");
                        LoadEquipmentList();
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка добавления оборудования: {response.StatusCode} - {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления оборудования: " + ex.Message);
                }
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateEquipmentComboBox.SelectedItem is Equipment selectedEquipment)
            {
                try
                {
                    selectedEquipment.Name = UpdateEquipmentNameTextBox.Text.Trim();
                    selectedEquipment.Type = UpdateEquipmentTypeTextBox.Text.Trim();
                    selectedEquipment.Status = UpdateEquipmentStatusTextBox.Text.Trim();

                    // Создаем команду для обновления
                    var patchDoc = new JsonPatchDocument<Equipment>();
                    if (!string.IsNullOrEmpty(selectedEquipment.Name))
                        patchDoc.Replace(e => e.Name, selectedEquipment.Name);
                    if (!string.IsNullOrEmpty(selectedEquipment.Type))
                        patchDoc.Replace(e => e.Type, selectedEquipment.Type);
                    if (!string.IsNullOrEmpty(selectedEquipment.Status))
                        patchDoc.Replace(e => e.Status, selectedEquipment.Status);

                    // Используем локально инициализированный HttpClient
                    using (var client = new HttpClient { BaseAddress = new Uri("https://localhost:7183/api/") })
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());
                        var request = new HttpRequestMessage(HttpMethod.Patch, $"Equipment/{selectedEquipment.EquipmentID}")
                        {
                            Content = new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, "application/json")
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Оборудование успешно обновлено.");
                            LoadEquipmentList();
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Ошибка обновления оборудования: {response.StatusCode} - {errorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления оборудования: " + ex.Message);
                }
            }
        }




        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteEquipmentComboBox.SelectedItem is Equipment selectedEquipment)
            {
                try
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());
                    var response = await _client.DeleteAsync($"Equipment/{selectedEquipment.EquipmentID}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Оборудование успешно удалено.");
                        LoadEquipmentList();  // Перезагружаем данные
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка удаления оборудования: {response.StatusCode} - {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления оборудования: " + ex.Message);
                }
            }
        }


        private void MapOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapOptionsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "Показать карту")
                {
                    SensorMap.Visibility = Visibility.Visible;
                }
                else
                {
                    SensorMap.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ToggleMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (SensorMap.Visibility == Visibility.Visible)
            {
                SensorMap.Visibility = Visibility.Collapsed;
                ToggleMapButton.Content = "Открыть карту";


                StatusPanel.Visibility = Visibility.Collapsed;
                UpdatePanel.Visibility = Visibility.Visible;
                DeletePanel.Visibility = Visibility.Visible;
                AddPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SensorMap.Visibility = Visibility.Visible;
                ToggleMapButton.Content = "Скрыть карту";


                StatusPanel.Visibility = Visibility.Visible;
                UpdatePanel.Visibility = Visibility.Collapsed;
                DeletePanel.Visibility = Visibility.Collapsed;
                AddPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void EquipmentStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipmentStatusComboBox.SelectedItem is Equipment selectedEquipment)
            {
                StatusTextBox.Text = selectedEquipment.Status;
            }
        }

        private void SensorMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = SensorMap.ViewportPointToLocation(e.GetPosition(SensorMap));

            if (_pushpin == null)
            {
                _pushpin = new Pushpin();
                SensorMap.Children.Add(_pushpin);
            }

            _pushpin.Location = location;
            LatitudeTextBox.Text = $"{location.Latitude:F6}";
            LongitudeTextBox.Text = $"{location.Longitude:F6}";


            var city = GetCityByCoordinates(location.Latitude, location.Longitude); 
            LocationTextBox.Text = city;
        }

        private async void SaveStatusButton_Click(object sender, RoutedEventArgs e)
        {
            decimal temperature, pressure, latitude, longitude;

            // Проверка и парсинг значений
            if (!decimal.TryParse(TemperatureTextBox.Text, out temperature))
            {
                MessageBox.Show("Некорректное значение температуры.");
                return;
            }

            if (!decimal.TryParse(PressureTextBox.Text, out pressure))
            {
                MessageBox.Show("Некорректное значение давления.");
                return;
            }

            if (!decimal.TryParse(LatitudeTextBox.Text, out latitude))
            {
                MessageBox.Show("Некорректное значение широты.");
                return;
            }

            if (!decimal.TryParse(LongitudeTextBox.Text, out longitude))
            {
                MessageBox.Show("Некорректное значение долготы.");
                return;
            }

            if (EquipmentStatusComboBox.SelectedItem is not Equipment selectedEquipment)
            {
                MessageBox.Show("Оборудование не выбрано.");
                return;
            }

            int equipmentID = selectedEquipment.EquipmentID;
            bool isTemperatureOutOfRange = temperature > 100 || temperature < -50;
            bool isPressureOutOfRange = pressure > 300 || pressure < 50;

            // Обработка случаев некорректных значений температуры или давления
            if (isTemperatureOutOfRange || isPressureOutOfRange)
            {
                var alertMessage = new AlertMessage
                {
                    EquipmentID = equipmentID,
                    Temperature = temperature,
                    Pressure = pressure,
                    Location = LocationTextBox.Text,
                    Latitude = latitude,
                    Longitude = longitude,
                    Timestamp = DateTime.UtcNow,
                    AlertType = isTemperatureOutOfRange ? "Температура" : "Давление",
                    Message = isTemperatureOutOfRange
                        ? "Температура вне допустимого диапазона"
                        : "Давление вне допустимого диапазона"
                };

                try
                {
                    RabbitMqService.SendAlertMessage(alertMessage);
                    MessageBox.Show("Обнаружены некорректные данные. Сообщение отправлено в очередь RabbitMQ.");
                    ClearStatusInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка отправки сообщения: " + ex.Message);
                }
                return;
            }

            // Создание и отправка статуса оборудования
            var statusMessage = new AddEquipmentStatusCommand
            {
                EquipmentID = equipmentID,
                Temperature = temperature,
                Pressure = pressure,
                Location = LocationTextBox.Text,
                Status = StatusTextBox.Text,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow
            };

            var debugMessage = $"Отправляемые данные:\n" +
                               $"Оборудование ID: {equipmentID}\n" +
                               $"Температура: {temperature}\n" +
                               $"Давление: {pressure}\n" +
                               $"Местоположение: {LocationTextBox.Text}\n" +
                               $"Статус: {StatusTextBox.Text}\n" +
                               $"Широта: {latitude}\n" +
                               $"Долгота: {longitude}";
            MessageBox.Show(debugMessage);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7183/api/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());

                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(statusMessage), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("EquipmentStatus", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Статус оборудования сохранен.");
                        ClearStatusInputs();
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();

                        // Попытка вывести детализированную информацию об ошибке
                        try
                        {
                            var errorDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(errorMessage);
                            StringBuilder detailedError = new StringBuilder("Ошибка при добавлении данных статуса:\n");

                            foreach (var detail in errorDetails)
                            {
                                detailedError.AppendLine($"{detail.Key}: {detail.Value}");
                            }

                            MessageBox.Show(detailedError.ToString());
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка обновления статуса оборудования: {response.StatusCode} - {errorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке запроса на сервер: " + ex.Message);
                }
            }
        }



        private void ClearStatusInputs()
        {
            TemperatureTextBox.Clear();
            PressureTextBox.Clear();
            LatitudeTextBox.Clear();
            LongitudeTextBox.Clear();
            LocationTextBox.Clear();
        }

        private string GetCityByCoordinates(double latitude, double longitude)
        {
            // Метод получения города по координатам с использованием внешнего API
            return "Город (пример)";
        }
    }
}
