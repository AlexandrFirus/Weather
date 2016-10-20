using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newtonsoft.Json;
using System.Net;
using System.IO;

using System.Net.Http;


namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetAllCities = new Cities();

            comboBox.Visibility = Visibility.Collapsed;
            label.Visibility = Visibility.Collapsed;
            button.Visibility = Visibility.Collapsed;
            imgTxbl.Visibility = Visibility.Collapsed;

        }
        Cities GetAllCities;
        

        async void OutputByAsync(int CityID)   
        {
            string uri = $"http://api.openweathermap.org/data/2.5/weather?id={CityID}&units=metric&APPID=861ee4fb7dc955580611e95897e7387f";
            HttpClient client = new HttpClient();   
            string content = await client.GetStringAsync(uri);  
            WeatherCity m = JsonConvert.DeserializeObject<WeatherCity>(content);

            // блок кода для подключения картинки
            BitmapImage img1 = new BitmapImage();
            img1.BeginInit();
            img1.UriSource = new Uri($"http://openweathermap.org/img/w/{m.weather[0].icon}.png");
            img1.EndInit();
            imgTxbl.Source = img1;
            imgTxbl.Visibility = Visibility.Visible;

            DataCity.Text = $"City:{m.name}\nState:{m.sys.country}\nTemprature:{m.main.temp}C°\nWindSpeed:{m.wind.speed}\nHumidity:{m.main.humidity}";
        }
        

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //вывод погоды из комбобокса
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = comboBox.SelectedItem as TextBlock;
            if (selectedItem != null)
            {
                OutputByAsync((int)selectedItem.Tag);

                FindField.Clear();
                comboBox.Visibility = Visibility.Collapsed;
                label.Visibility = Visibility.Collapsed;
                DataCity.Visibility = Visibility.Visible;
                imgTxbl.Visibility = Visibility.Collapsed;
            }
        }

        private void FindField_TextChanged(object sender, TextChangedEventArgs e)
        {
            comboBox.Items.Clear();
            List<City> matches = new List<City>();
            matches = GetAllCities.FindCityFromConsole(FindField.Text);// получить список городов со вей их инфой 
            if (matches != null)
            {
                if (matches.Count == 0)
                {
                    DataCity.Text = "Incorrect city's name. Try again.";
                    comboBox.Visibility = Visibility.Collapsed;
                    label.Visibility = Visibility.Collapsed;
                    DataCity.Visibility = Visibility.Visible;
                    imgTxbl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (matches.Count == 1)
                    {
                        OutputByAsync(matches[0]._id);
                        comboBox.Visibility = Visibility.Collapsed;
                        label.Visibility = Visibility.Collapsed;
                        DataCity.Visibility = Visibility.Visible;
                        imgTxbl.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (matches.Count > 1)
                        {
                            foreach (City val in matches)
                            {
                                comboBox.Items.Add(new TextBlock { Text = $"{val.name}  {val.country}\t[{val.coord.lat};{val.coord.lat}]", Tag = val._id });
                                if (comboBox.Items.Count > 20)
                                    break;
                            }
                            comboBox.Visibility = Visibility.Visible;
                            label.Visibility = Visibility.Visible;
                            DataCity.Visibility = Visibility.Collapsed;
                            imgTxbl.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            //else
        }
    }
}


