using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Net;
using System.IO;

// макет для хоанения информации с внетренней базы городов (формат JSON)
public class City
{
    public int _id { get; set; }
    public string name { get; set; }
    public string country { get; set; }
    public Coord coord { get; set; }
}


// класс, который извлекает все города из json-файла
public class Cities
{
    public List<City> cities { get; set; } // поле типа список городов

    public Cities()   // конструктор к переменной cities
    {
        String AllCitiesData = File.ReadAllText("city.list.json"); 
        cities = JsonConvert.DeserializeObject<List<City>>(AllCitiesData); 
    }

    // метод поиска города и его ID по вводимому из консоли
    public List<City> FindCityFromConsole(string FindX)
    {       
        if (FindX.Length>1)
            return cities.Where(s => s.name.Contains(FindX)).ToList();
        return null;
    }
}
