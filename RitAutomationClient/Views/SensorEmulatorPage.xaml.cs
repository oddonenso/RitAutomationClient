using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace RitAutomationClient.Views
{
    public partial class SensorEmulatorPage : Page
    {
        private readonly JwtService _jwtService;

        public SensorEmulatorPage(JwtService jwtService)
        {
            InitializeComponent();
            _jwtService = jwtService;
            LoadEquipmentList();
        }

        private async void LoadEquipmentList()
        {
            using (var client = new HttpClient())
            {
                // Установка базового адреса
                client.BaseAddress = new Uri("https://localhost:7183/api/");
                string token;

                try
                {
                    // Получение токена
                    token = _jwtService.GetTokenFromStorage();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Асинхронный запрос к API
                    var response = await client.GetAsync("Equipment");

                    if (response.IsSuccessStatusCode)
                    {
                        // Чтение содержимого ответа
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Десериализация JSON-ответа
                        var equipmentList = JsonSerializer.Deserialize<List<Equipment>>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Убедитесь, что регистры совпадают
                        });

                        // Проверка, что список оборудования не пуст
                        if (equipmentList != null && equipmentList.Count > 0)
                        {
                            // Обновление источников данных для комбобоксов
                            UpdateEquipmentComboBox.ItemsSource = equipmentList;
                            DeleteEquipmentComboBox.ItemsSource = equipmentList;

                            // Установка выбранного элемента по умолчанию
                            UpdateEquipmentComboBox.SelectedIndex = 0;
                            DeleteEquipmentComboBox.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("Список оборудования пуст.");
                        }
                    }
                    else
                    {
                        // Обработка ошибок при получении данных
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка загрузки списка оборудования: {response.StatusCode} - {errorContent}");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show("Ошибка сети: " + httpEx.Message);
                }
                catch (JsonException jsonEx)
                {
                    MessageBox.Show("Ошибка парсинга JSON: " + jsonEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
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

            var equipmentMessage = new CreateEquipmentCommand
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
                    var content = new StringContent(JsonSerializer.Serialize(equipmentMessage), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("Equipment", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var equipmentId = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Оборудование успешно добавлено. ID: {equipmentId}");
                        ClearInputs();
                        LoadEquipmentList(); // Перезагрузить список оборудования
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка добавления оборудования: {response.StatusCode} - {errorMessage}");
                    }
                }
                catch (NullReferenceException nullRefEx)
                {
                    MessageBox.Show("Ошибка добавления оборудования (NullReference): " + nullRefEx.ToString());
                }
                catch (JsonException jsonEx)
                {
                    MessageBox.Show("Ошибка добавления оборудования (JSON): " + jsonEx.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления оборудования: " + ex.ToString());
                }
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateEquipmentComboBox.SelectedItem is Equipment selectedEquipment)
            {
                
                selectedEquipment.Name = UpdateEquipmentNameTextBox.Text.Trim();
                selectedEquipment.Type = UpdateEquipmentTypeTextBox.Text.Trim(); 
                selectedEquipment.Status = UpdateEquipmentStatusTextBox.Text.Trim();

                var patch = new JsonPatchDocument<Equipment>();
                patch.Replace(e => e.Name, selectedEquipment.Name);
                patch.Replace(e => e.Type, selectedEquipment.Type); 
                patch.Replace(e => e.Status, selectedEquipment.Status);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7183/api/");
                    string token;

                    try
                    {
                        token = _jwtService.GetTokenFromStorage();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var content = new StringContent(JsonSerializer.Serialize(patch), Encoding.UTF8, "application/json");

                        var response = await client.PatchAsync($"Equipment/{selectedEquipment.EquipmentID}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Оборудование успешно обновлено.");
                            LoadEquipmentList(); 
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка обновления оборудования: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка обновления оборудования: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оборудование для обновления.");
            }
        }


        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteEquipmentComboBox.SelectedItem is Equipment selectedEquipment)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7183/api/");
                    string token;

                    try
                    {
                        token = _jwtService.GetTokenFromStorage();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var response = await client.DeleteAsync($"Equipment/{selectedEquipment.EquipmentID}");

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Оборудование успешно удалено.");
                            LoadEquipmentList(); // Перезагрузить список после удаления
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка удаления оборудования: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления оборудования: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оборудование для удаления.");
            }
        }

        private void ClearInputs()
        {
            AddEquipmentNameTextBox.Clear();
            AddEquipmentTypeTextBox.Clear();
            AddEquipmentStatusTextBox.Clear();
        }

        private void MapOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapOptionsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "Показать карту")
                {
                    SensorMap.Visibility = Visibility.Visible;
                    ToggleMapButton.Content = "Скрыть карту";
                }
                else
                {
                    SensorMap.Visibility = Visibility.Collapsed;
                    ToggleMapButton.Content = "Открыть карту";
                }
            }
        }

        private void ToggleMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (SensorMap.Visibility == Visibility.Visible)
            {
                SensorMap.Visibility = Visibility.Collapsed;
                ToggleMapButton.Content = "Открыть карту";
            }
            else
            {
                SensorMap.Visibility = Visibility.Visible;
                ToggleMapButton.Content = "Скрыть карту";
            }
        }
    }
}
