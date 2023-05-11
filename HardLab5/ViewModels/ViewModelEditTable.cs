using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace HardLab5
{
    class ViewModelEditTable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<ViewModelEditTable> Items { get; } = new ObservableCollection<ViewModelEditTable>();
        private ObservableCollection<string> _names;
        public ObservableCollection<string> listOfColumns
        {
            get { return _names; }
            set
            {
                _names = value;
                OnPropertyChanged();
            }
        }

        private DataTable _dataNewTable;
        public DataTable DataNewTable
        {
            get { return _dataNewTable; }
            set
            {
                _dataNewTable = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        private string _message2;
        public string Message2
        {
            get { return _message2; }
            set
            {
                _message2 = value;
                OnPropertyChanged();
            }
        }

        private string _newColumnName;
        public string NewColumnName
        {
            get { return _newColumnName; }
            set
            {
                _newColumnName = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumn;
        public string SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                _selectedColumn = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumnDelete;
        public string SelectedColumnDelete
        {
            get { return _selectedColumnDelete; }
            set
            {
                _selectedColumnDelete = value;
                OnPropertyChanged();
            }
        }

        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                //if (value == _text) return;
                _columnName = value;
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

        public ViewModelEditTable()
        {
            DataNewTable = MainViewModel.copyDataTable;
            Message = MainViewModel.tableName;
            ObservableCollection<string> names = new ObservableCollection<string>();
            foreach (var column in DataNewTable.Columns)
            {
                names.Add(column.ToString());
            }
            listOfColumns = names;
            listOfColumns.Add("нет выбора");
        }

        public ICommand CreateColumn => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelEditTable());

        });

        public ICommand RemoveColumn => new DelegateCommand(param =>
        {
            int count = 0;
            foreach(var item in Items)
            {
                count += 1;
                if (count == Items.Count)
                {
                    Items.Remove(item);
                    break;
                }
            }
        });

        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (MainViewModel.tableName != Message && Message != null && Message != "")
            {
                CreateNewFiles();
            }
            if (SelectedColumn != null && SelectedColumn != "" && NewColumnName != null && NewColumnName != "" && SelectedColumn != "нет выбора")
            {
                RemoveColumnName();
                SelectedColumn = null;
            }
            if(Items != null)
            {
                AddNewColumn();
            }
            if (SelectedColumnDelete != null && SelectedColumnDelete != "нет выбора")
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Вы уверены, что хотите безвозвратно удалить столбец?", "Подтверждение действий", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteColumn();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
                
            }

            string jsonNewScheme = JsonSerializer.Serialize(MainViewModel.selectedScheme);
            File.WriteAllText(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.json", jsonNewScheme);
            UpdateTable();
        });


        public void CreateNewFiles()
        {
            File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.json", MainViewModel.folderPath + $"\\{Message}.json");
            File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.csv", MainViewModel.folderPath + $"\\{Message}.csv");
            MainViewModel.selectedScheme.Name = Message;
        }

        public void RemoveColumnName()
        {
            if (CheckEqualsNames("EditColumnName"))
            {
                System.Windows.MessageBox.Show("Попытка добавления столбца с уже существующим именем");
                return;
            }
            foreach (Column column in MainViewModel.selectedScheme.Columns)
            {
                if (column.Name == SelectedColumn)
                {
                    column.Name = NewColumnName;
                }
            }
        }

        public void AddNewColumn()
        {
            int countOfColumn = 0;
            foreach (Column column in MainViewModel.selectedScheme.Columns)
            {
                countOfColumn += 1;
            }

            foreach (var item in Items)
            {
                Column column = new Column();
                if (item.ColumnName == null || item.ColumnName == "" || item.Type == null)
                {
                    System.Windows.MessageBox.Show("Данные не заполнены до конца");
                    return;
                }
                if(CheckEqualsNames("CreateNewColumn"))
                {
                    System.Windows.MessageBox.Show("Попытка добавления столбца с уже существующим именем");
                    return;
                }
                column.Name = item.ColumnName;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                MainViewModel.selectedScheme.Columns.Add(column);
                listOfColumns.Add(column.Name);
                countOfColumn += 1;
                AddColumnInTable(column, countOfColumn);

            }
        }
        public bool CheckEqualsNames(string location)
        {
            if(location == "EditColumnName")
            {
                foreach (Column column in MainViewModel.selectedScheme.Columns)
                {
                    if (column.Name == NewColumnName)
                    {
                        return true;
                    }
                }
            }
            else if(location == "CreateNewColumn")
            {
                foreach (var item in Items)
                {
                    foreach (Column column in MainViewModel.selectedScheme.Columns)
                    {
                        if (column.Name == item.ColumnName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void AddColumnInTable(Column column, int countOfColumn)
        {
            foreach (var row in MainViewModel.keyTables[MainViewModel.selectedScheme].Rows)
            {
                switch (column.Type)
                {
                    case "uint":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "int":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "float":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "double":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "datetime":
                        {
                            row.Data.Add(column, DateTime.MinValue);
                            break;
                        }
                    case "string":
                        {
                            row.Data.Add(column, null);
                            break;
                        }
                }
            }
            string pathTable = MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.csv";
            RewriteCSV(pathTable, countOfColumn);
        }

        public void DeleteColumn()
        {
            int countOfColumn = 0;
            foreach (Column column in MainViewModel.selectedScheme.Columns)
            {
                if (column.Name == SelectedColumnDelete)
                {
                    MainViewModel.selectedScheme.Columns.Remove(column);
                    listOfColumns.Remove(column.Name);
                    foreach (var row in MainViewModel.keyTables[MainViewModel.selectedScheme].Rows)
                    {
                        row.Data.Remove(column);
                    }
                    string pathTable = MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.csv";
                    RewriteCSV(pathTable, countOfColumn);
                    break;
                }
                countOfColumn += 1;
            }
        }

        public void RewriteCSV(string pathTable, int numberOfColumn)
        {
            int count = 1;
            StringBuilder newFile = new StringBuilder();
            foreach (var row in MainViewModel.keyTables[MainViewModel.selectedScheme].Rows)
            {
                foreach (Column column in row.Data.Keys)
                {
                    if (count == numberOfColumn)
                    {
                        newFile.Append($"{row.Data[column]}" + "\n");
                        break;
                    }
                    newFile.Append($"{row.Data[column]}" + ";");
                    count += 1;
                }
                count = 1;
            }
            Message2 = newFile.ToString();
            File.WriteAllText(pathTable, newFile.ToString());

        }

        public void UpdateTable()
        {
            Items.Clear();
            NewColumnName = null;
            DataNewTable.Clear();
            DataTable dataTable = new DataTable();
            foreach (var keyTable in MainViewModel.keyTables)
            {
                if (keyTable.Key.Name == MainViewModel.selectedScheme.Name)
                {
                    MainViewModel.selectedScheme = keyTable.Key;
                    foreach (Column column in keyTable.Key.Columns)
                    {
                        dataTable.Columns.Add(column.Name);
                    }

                    for (int i = 0; i < keyTable.Value.Rows.Count; i++)
                    {
                        DataRow newRow = dataTable.NewRow();
                        foreach (var rows in keyTable.Value.Rows[i].Data)
                        {
                            newRow[rows.Key.Name] = rows.Value;
                        }
                        dataTable.Rows.Add(newRow);
                    }
                    break;
                }
            }
            DataNewTable = dataTable;
        }
    }
}
