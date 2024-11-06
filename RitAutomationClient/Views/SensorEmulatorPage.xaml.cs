using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly HttpClient _client;
        private RabbitMqService RabbitMqService => _rabbitMqService ??= new RabbitMqService();
        private RabbitMqService _rabbitMqService;

        private readonly string _apiBaseUrl;

        public SensorEmulatorPage(JwtService jwtService)
        {
            InitializeComponent();
            _jwtService = jwtService;
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? throw new Exception("API base URL not configured");
            _client = new HttpClient { BaseAddress = new Uri(_apiBaseUrl) };  
            LoadEquipmentList();
            LoadEquipmentTypes();
            LoadEquipmentStatuses();
        }

        private async void LoadEquipmentTypes()
        {
            try
            {
                string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                string typesUrl = $"{apiBaseUrl}EquipmentInfo/EquipmentTypes";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());

                var typeResponse = await _client.GetAsync(typesUrl);
                if (typeResponse.IsSuccessStatusCode)
                {
                    var typeContent = await typeResponse.Content.ReadAsStringAsync();
                    var types = JsonSerializer.Deserialize<List<Data.Tables.Type>>(typeContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    if (types != null && types.Count > 0)
                    {
                        AddEquipmentTypeComboBox.ItemsSource = types;
                        UpdateEquipmentTypeComboBox.ItemsSource = types;
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка загрузки типов оборудования: {typeResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки типов оборудования: " + ex.Message);
            }
        }

        private async void LoadEquipmentStatuses()
        {
            try
            {
                string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                string statusesUrl = $"{apiBaseUrl}EquipmentInfo/EquipmentStatuses";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());

                var statusResponse = await _client.GetAsync(statusesUrl);
                if (statusResponse.IsSuccessStatusCode)
                {
                    var statusContent = await statusResponse.Content.ReadAsStringAsync();
                    var statuses = JsonSerializer.Deserialize<List<Data.Tables.Status>>(statusContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    if (statuses != null && statuses.Count > 0)
                    {
                        AddEquipmentStatusComboBox.ItemsSource = statuses;
                        UpdateEquipmentStatusComboBox.ItemsSource = statuses;
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка загрузки статусов оборудования: {statusResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статусов оборудования: " + ex.Message);
            }
        }




        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new LoginPage(_jwtService));
            }
        }

        private async void AddEquipmentTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEquipmentTypes();
            if (AddEquipmentStatusComboBox.SelectedItem is Data.Tables.Type selectedEquipment)
            {
                UpdateEquipmentStatusComboBox.Text = selectedEquipment.typeName;
            }
        }

        private async void AddEquipmentStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEquipmentStatuses();
            if (AddEquipmentStatusComboBox.SelectedItem is Status selectedEquipment)
            {
                UpdateEquipmentStatusComboBox.Text = selectedEquipment.statusName;
            }
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

                if (equipmentList != null)
                {
                    EquipmentStatusComboBox.ItemsSource = equipmentList;
                    UpdateEquipmentComboBox.ItemsSource = equipmentList;
                    DeleteEquipmentComboBox.ItemsSource = equipmentList;
                }
                else
                {
                    MessageBox.Show("Не удалось десериализовать список оборудования.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка оборудования: " + ex.Message);
            }
        }
      
        private async void EquipmentStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           LoadEquipmentList();

            if (EquipmentStatusComboBox.SelectedItem is Equipment selectedEquipment)
            {
                UpdateEquipmentStatusComboBox.Text = selectedEquipment.Status?.statusName;
            }
        }
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddEquipmentNameTextBox.Text) ||
                AddEquipmentTypeComboBox.SelectedItem == null ||
                AddEquipmentStatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            var newEquipment = new CreateEquipmentCommand
            {
                Name = AddEquipmentNameTextBox.Text.Trim(),
                TypeId = (int)AddEquipmentTypeComboBox.SelectedValue,
                StatusId = (int)AddEquipmentStatusComboBox.SelectedValue
            };

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());
                var content = new StringContent(JsonSerializer.Serialize(newEquipment), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("Equipment", content);
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

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateEquipmentComboBox.SelectedItem is Equipment selectedEquipment)
            {
                try
                {
                    selectedEquipment.Name = UpdateEquipmentNameTextBox.Text.Trim();
                    selectedEquipment.TypeId = (int)UpdateEquipmentTypeComboBox.SelectedValue;
                    selectedEquipment.StatusId = (int)UpdateEquipmentStatusComboBox.SelectedValue;

                    var patchDoc = new JsonPatchDocument<Equipment>();
                    patchDoc.Replace(e => e.Name, selectedEquipment.Name);
                    patchDoc.Replace(e => e.TypeId, selectedEquipment.TypeId);
                    patchDoc.Replace(e => e.StatusId, selectedEquipment.StatusId);

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());
                    var request = new HttpRequestMessage(HttpMethod.Patch, $"Equipment/{selectedEquipment.EquipmentID}")
                    {
                        Content = new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, "application/json")
                    };

                    var response = await _client.SendAsync(request);

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
                        LoadEquipmentList();
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
                SensorMap.Visibility = selectedItem.Content.ToString() == "Показать карту" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ToggleMapButton_Click(object sender, RoutedEventArgs e)
        {
            SensorMap.Visibility = SensorMap.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ToggleMapButton.Content = SensorMap.Visibility == Visibility.Visible ? "Скрыть карту" : "Открыть карту";
            StatusPanel.Visibility = SensorMap.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            UpdatePanel.Visibility = DeletePanel.Visibility = AddPanel.Visibility = SensorMap.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
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
            if (!decimal.TryParse(TemperatureTextBox.Text, out var temperature))
            {
                MessageBox.Show("Некорректное значение температуры.");
                return;
            }

            if (!decimal.TryParse(PressureTextBox.Text, out var pressure))
            {
                MessageBox.Show("Некорректное значение давления.");
                return;
            }

            if (!decimal.TryParse(LatitudeTextBox.Text, out var latitude))
            {
                MessageBox.Show("Некорректное значение широты.");
                return;
            }

            if (!decimal.TryParse(LongitudeTextBox.Text, out var longitude))
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
            bool isPressureOutOfRange = pressure > 1013 || pressure < 860;

            if (isTemperatureOutOfRange || isPressureOutOfRange)
            {
                var alertMessage = new EquipmentStatusDto
                {
                    EquipmentID = equipmentID,
                    Temperature = temperature,
                    Pressure = pressure,
                    Longitude = longitude,
                    Latitude = latitude,
                    Timestamp = DateTime.Now,
                    Type = "Warning",
                    Message = "Параметры температуры или давления вне допустимых значений."
                };

                RabbitMqService.SendAlertMessage(alertMessage);
                MessageBox.Show("Сообщение об оповещении отправлено в RabbitMQ.");
            }
            else
            {
                var sensorData = new AddEquipmentStatusCommand
                {
                    EquipmentID = equipmentID,
                    Temperature = temperature,
                    Pressure = pressure,
                    Longitude = longitude,
                    Latitude = latitude,
                    Location = "LocationNotSpecified",
                    
                };

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetTokenFromStorage());
                var content = new StringContent(JsonSerializer.Serialize(sensorData), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("EquipmentStatus", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Статус успешно сохранен в базе данных.");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сохранения статуса: {response.StatusCode} - {errorMessage}");
                }
            }
        }
        

            private string GetCityByCoordinates(double latitude, double longitude)
            {

                return "Gород";
            }
        }
    }
