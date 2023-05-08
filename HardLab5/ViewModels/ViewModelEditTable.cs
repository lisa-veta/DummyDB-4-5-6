using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
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
        private List<string> _names;
        public List<string> listOfColumns
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

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                //if (value == _text) return;
                _text = value;
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
            List<string> names = new List<string>();
            foreach (var column in DataNewTable.Columns)
            {
                names.Add(column.ToString());
            }
            listOfColumns = names;
        }


        public ICommand CreateColumn => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelEditTable());

        });

        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (MainViewModel.tableName != Message && Message != null)
            {
                //File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.json", MainViewModel.folderPath + $"\\{Message}.json");
                //File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.csv", MainViewModel.folderPath + $"\\{Message}.csv");
                MainViewModel.selectedScheme.Name = Message;
            }
            if (SelectedColumn != null && NewColumnName != null)
            {
                foreach (Column column in MainViewModel.selectedScheme.Columns)
                {
                    if (column.Name == SelectedColumn)
                    {
                        column.Name = NewColumnName;
                    }
                }
            }
            foreach (var item  in Items) 
            {
                Column column = new Column();
                if (item.Text == null || item.Type == null)
                {
                    MessageBox.Show("Данные не заполнены до конца");
                    return;
                }
                column.Name = item.Text;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                MainViewModel.selectedScheme.Columns.Add(column);
            }

            string jsonNewScheme = JsonSerializer.Serialize(MainViewModel.selectedScheme);
            File.WriteAllText(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.json", jsonNewScheme);
            UpdateTable();
        });

        public void UpdateTable()
        {
            DataNewTable.Clear();
            DataTable dataTable = new DataTable();
            foreach (var keyTable in MainViewModel.keyTables)
            {
                if (keyTable.Key.Name == MainViewModel.tableName)
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


        public ICommand AddTable => new DelegateCommand(param =>
        {

        });
    }
}
