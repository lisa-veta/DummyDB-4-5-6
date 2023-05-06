using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HardLab5
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

        public static void CreateFile(List<Column> columnsTable, string nameTable, string path)
        {
            var tableScheme = new TableScheme
            {
                Name = nameTable,
                Columns = columnsTable
            };
            string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);
            /// File.Create(path + $"\\{nameTable}.json");
            string pathOfScheme = path + $"\\{nameTable}.json";
            string pathOfTable = path + $"\\{nameTable}.csv";
            File.WriteAllText(path, jsonNewScheme);
        }
    }
}
