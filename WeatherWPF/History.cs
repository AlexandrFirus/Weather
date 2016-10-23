using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

public class History
{
    public List<City> history { get; set; } // список <городов> у файле 

    public History()   // конструктор к переменной history
    {
        String AllCitiesData = File.ReadAllText("city.list.json");
        var AllCities = JsonConvert.DeserializeObject<List<City>>(AllCitiesData);   // - это ВСЕ города

        List<String> GetHistoryFromFile = File.ReadAllLines("history.txt").ToList();

        if (GetHistoryFromFile.Count > 10)                                          // - обрезка до 10 на приеме
            GetHistoryFromFile.RemoveRange(0, GetHistoryFromFile.Count - 10);

        List<int> KeeperFromFile = new List<int>();                                 // - это города записаные у файл
        foreach (string x in GetHistoryFromFile)
        {
            int id = Convert.ToInt32(x);
            KeeperFromFile.Add(id);
        }

        history = AllCities.Where(X => KeeperFromFile.Contains(X._id)).ToList();
    }

    public List<City> AddCity(int ID)
    {
        if (history.Count > 10)
        {
            history.RemoveAt(0);
            history.Add(history.First(s => s._id == ID));
            return history;
        }
        return history;
    }
}
