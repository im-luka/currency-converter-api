using CurrencyConverter_API.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;
using CurrencyConverter_API.exceptions;

namespace CurrencyConverter_API
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Root root = new Root();
            AssignRoot(root);

        }

        private async void AssignRoot(Root root)
        {
            // Insert your app_id below
            root = await ImportData<Root>("https://openexchangerates.org/api/latest.json?app_id=???");
            ComboBoxFill(inputFrom, root);
            ComboBoxFill(inputTo, root);
        }

        private static async Task<Root> ImportData<T>(string url)
        {
            Root tmpRoot = new Root();

            try
            {
                HttpClient client = new HttpClient();
                using(client)
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage responseMessage = await client.GetAsync(url);
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string response = await responseMessage.Content.ReadAsStringAsync();
                        Root getObject = JsonConvert.DeserializeObject<Root>(response);

                        return getObject;
                    }
                    else
                        return tmpRoot;
                }
            }
            catch
            {
                return tmpRoot;
            }
        }

        private void ComboBoxFill(ComboBox inputBox, Root root)
        {
            DataTable dataTable = new DataTable();

            Dictionary<string, double> dict = root.rates.GetType().GetProperties()
                                                .ToDictionary(prop => prop.Name, prop => (double)prop.GetValue(root.rates, null));

            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Value");

            dataTable.Rows.Add("--- SELECT ---", 0);
            foreach (KeyValuePair<string, double> entry in dict)
            {
                dataTable.Rows.Add(entry.Key.ToString(), entry.Value);
            }

            inputBox.DisplayMemberPath = "Name";
            inputBox.SelectedIndex = 0;
            inputBox.ItemsSource = dataTable.DefaultView;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            bool isNum = Int32.TryParse(inputAmount.Text, out int number);
            try
            {
                CheckInput(isNum);

                if (inputFrom.SelectedIndex == 0 || inputTo.SelectedIndex == 0)
                {
                    MessageBox.Show("Please fill currency type field.", "Currency converter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    double amount = Convert.ToDouble(inputAmount.Text);

                    DataRowView dataFrom = (DataRowView)inputFrom.SelectedItem;
                    double valueFrom = Convert.ToDouble(dataFrom["Value"]);

                    DataRowView dataTo = (DataRowView)inputTo.SelectedItem;
                    double valueTo = Convert.ToDouble(dataTo["Value"]);

                    if (inputFrom.Text.Equals("USD"))
                    {
                        lbCurrency.Content = $"{amount * valueTo} {dataTo["Name"]}";
                    }
                    else
                    {
                        lbCurrency.Content = $"{amount / valueFrom * valueTo} {dataTo["Name"]}";
                    }
                }
            }
            catch (WrongInputException ex)
            {
                MessageBox.Show(ex.Message + " \nPlease try again.", "Currency converter", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckInput(bool isNum)
        {
            if (Regex.IsMatch(inputAmount.Text, @"^\s+$") || inputAmount.Text == "")
            {
                inputAmount.Text = string.Empty;
                throw new WrongInputException("Wrong input! Input is empty.");
            }
            else if (!isNum)
            {
                inputAmount.Text = string.Empty;
                throw new WrongInputException("Wrong input! Input must be a number.");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            lbCurrency.Content = string.Empty;
            inputAmount.Text = string.Empty;
            inputFrom.SelectedIndex = 0;
            inputTo.SelectedIndex = 0;
        }
    }
}
