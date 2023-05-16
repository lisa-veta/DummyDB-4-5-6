using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace DummyDB.Core
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class TableScheme
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // в таблице есть список столбцов
        [JsonPropertyName("columns")]
        public List<Column> Columns { get; set; }

        // конструктор, чтобы заполнить объект при создании
        public static TableScheme ReadFile(string path)
        {
            return JsonSerializer.Deserialize<TableScheme>(File.ReadAllText(path));
        }

        //public static void SafeNewData(TableScheme tableScheme)
        //{
        //    string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);
        //    string pathOfScheme = MainViewModel.folderPath + $"\\{tableScheme.Name}.json";
        //    string pathOfTable = MainViewModel.folderPath + $"\\{tableScheme.Name}.csv";
        //    File.WriteAllText(pathOfScheme, jsonNewScheme);
        //    File.Create(pathOfTable);
        //}
    }
}
