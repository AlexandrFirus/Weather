using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

class History
{
    public List<City> history { get; set; } // список <городов> у файле 

    public History()   // конструктор к переменной history
    {
        String AllCitiesData = File.ReadAllText("city.list.json");
        var AllCities = JsonConvert.DeserializeObject<List<City>>(AllCitiesData);   // - это ВСЕ города

        List<String> GetHistoryFromFile = File.ReadAllLines("history.txt").ToList();

        List<int> KeeperFromFile = null;                                            // - это города записаные у файл
        foreach (string x in GetHistoryFromFile)
        {
            int id = Convert.ToInt32(x);
            KeeperFromFile.Add(id);
        }
        history = AllCities.Where(X => KeeperFromFile.Contains(X._id)).ToList();
    }   
}
