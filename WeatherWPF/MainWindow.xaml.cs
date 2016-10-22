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

using Microsoft.Maps.MapControl.WPF;


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

            Pushpin pin = new Pushpin();

            MapLocalization.Focus();
            MapLocalization.Center = new Location(50,10);

            KeepHistory();
        }
        Cities GetAllCities;
                

        async void OutputByAsync(int CityID)   
        {
            // блок кода для вывода погоды по городу
            string uri = $"http://api.openweathermap.org/data/2.5/weather?id={CityID}&units=metric&APPID=861ee4fb7dc955580611e95897e7387f";
            HttpClient client = new HttpClient();   
            string content = await client.GetStringAsync(uri);  
            WeatherCity m = JsonConvert.DeserializeObject<WeatherCity>(content);
            DataCity.Text = $"City:{m.name}\nState:{m.sys.country}\nTemprature:{m.main.temp}C°\nWindSpeed:{m.wind.speed}\nHumidity:{m.main.humidity}";

            // блок кода для подключения картинки
            BitmapImage img1 = new BitmapImage();
            img1.BeginInit();
            img1.UriSource = new Uri($"http://openweathermap.org/img/w/{m.weather[0].icon}.png");
            img1.EndInit();
            imgTxbl.Source = img1;
            imgTxbl.Visibility = Visibility.Visible;

            // блок кода для карты
            Pushpin pin = new Pushpin();
            MapLocalization.Children.Clear();  // очищаем предыдущие метки  
            pin.Location = new Location(m.coord.lat, m.coord.lon);
            MapLocalization.Children.Add(pin);
            MapLocalization.Center = new Location(m.coord.lat,m.coord.lon);
            MapLocalization.ZoomLevel =5;

            // блок кода для записи истории в файл
            File.AppendAllText(@"C:\Users\Кучерявый\Desktop\project\Weather\WeatherWPF\history.txt", m.id + Environment.NewLine);  // !! - путь нужно укорортить
         }
                

        // главный комбобокс - событие выбора со списка
        private void comboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
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


        // обработка совпадений в ТекстБоксе
        private void FindField_TextChanged(object sender, TextChangedEventArgs e)
        {
            MapLocalization.ZoomLevel = 3;
            comboBox.Items.Clear();            
            var matches = GetAllCities.FindCityFromConsole(FindField.Text);// получить список городов со вей их инфой 
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
                    else  // добавляем совпадения в главный комбобокс
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


        private void button_Click(object sender, RoutedEventArgs e)
        {

        }


        // комбобокс История
        public async void KeepHistory()
        {
            List <String> AllCitiesHistory = File.ReadAllLines("history.txt").ToList();
            if (AllCitiesHistory.Count > 10)
                AllCitiesHistory.RemoveRange(0, AllCitiesHistory.Count-10);
            foreach (string x in AllCitiesHistory)
            {                
                string uri = $"http://api.openweathermap.org/data/2.5/weather?id={Convert.ToInt32(x)}&units=metric&APPID=861ee4fb7dc955580611e95897e7387f";
                HttpClient client = new HttpClient();
                string content = await client.GetStringAsync(uri);
                WeatherCity H = JsonConvert.DeserializeObject<WeatherCity>(content);
                History.Items.Add(new TextBlock { Text = $"{H.name}  {H.sys.country}\t  [{H.coord.lat};{H.coord.lat}]", Tag = H.id});                
            }
        }

        // комбобокс История событие выбора со списка 
        private void History_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = History.SelectedItem as TextBlock;
            if (selectedItem != null)
            {
                OutputByAsync((int)selectedItem.Tag);

                FindField.Clear();
                comboBox.Visibility = Visibility.Collapsed;
                label.Visibility = Visibility.Collapsed;
                DataCity.Visibility = Visibility.Visible;
                imgTxbl.Visibility = Visibility.Collapsed;

                History.SelectedValue = null; 
            }


        }
    }
}