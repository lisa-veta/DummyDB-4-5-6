using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Input;
using DummyDB.Core;
using HardLab5.ViewModels;

namespace HardLab5
{
    class ViewModelNewTable : BaseViewModel
    {
        public ObservableCollection<ViewModelNewTable> Items { get; } = new ObservableCollection<ViewModelNewTable>();


        public string folderPath;
        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                _columnName = value;
                OnPropertyChanged();
            }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                OnPropertyChanged();
            }
        }

        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        private bool _primary;
        public bool Primary
        {
            get { return _primary; }
            set
            {
                _primary = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateColumns => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelNewTable());
        });
        public ICommand RemoveColumn => new DelegateCommand(param =>
        {
            int count = 0;
            foreach (var item in Items)
            {
                count += 1;
                if (count == Items.Count)
                {
                    Items.Remove(item);
                    break;
                }
            }
        });

        public ICommand CreateTable => new DelegateCommand(param =>
        {
            if (TableName == null || TableName == "" || Items.Count == 0 || CheckEqualsData())
            {
                MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            string name = TableName;
            List<Column> columns = GetList();
            TableScheme tableScheme = new TableScheme
            {
                Name = name,
                Columns = columns
            };

            if (CheckEqualsNames())
            {
                System.Windows.MessageBox.Show("Найдены столбцы с повторяющимися именами");
                return;
            }
            CreateFiles(tableScheme, columns);
        });

        private string AddColumnInTable(List<Column> columns)
        {
            StringBuilder newFile = new StringBuilder();
            int count = 0;
            foreach (var column in columns)
            {
                count += 1;
                switch (column.Type)
                {
                    case "uint":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "int":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "double":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "datetime":
                        {
                            newFile.Append($"{DateTime.MinValue}");
                            break;
                        }
                    case "string":
                        {
                            newFile.Append($"");
                            break;
                        }
                }
                if(count != columns.Count())
                {
                    newFile.Append(";");
                }
            }
            return newFile.ToString();
        }

        private bool CheckEqualsNames()
        {
            for(int i = 0; i < Items.Count()-1; i++)
            {
                for(int j = i+1; j < Items.Count(); j++)
                {
                    if (Items[i].ColumnName == Items[j].ColumnName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<Column> GetList()
        {
            List<Column> columns = new List<Column>();
            foreach (var item in Items)
            {
                Column column = new Column();
                column.Name = item.ColumnName;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                columns.Add(column);
            }
            return columns;
        }

        private bool CheckEqualsData()
        {
            foreach (var item in Items)
            {
                if (item.ColumnName == null || item.ColumnName == "" || item.Type == null)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateFiles(TableScheme tableScheme, List<Column> columns)
        {
            string pathScheme = folderPath + $"\\{tableScheme.Name}.json";
            string pathTable = folderPath + $"\\{tableScheme.Name}.csv";
            string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);
            
            File.WriteAllText(pathScheme, jsonNewScheme);
            string newFile = AddColumnInTable(columns);
            File.WriteAllText(pathTable, newFile);

            System.Windows.MessageBox.Show("Успешно!");
            Items.Clear();
            TableName = null;
        }
    }
}
