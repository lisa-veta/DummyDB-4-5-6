using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;

namespace HardLab5
{
    class ViewModelEditTable : MainViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

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

       
        public ICommand EditTable => new DelegateCommand(param =>
        {
            if(tableName != Message && Message != null)
            {
                //File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.json", MainViewModel.folderPath + $"\\{Message}.json");
                //File.Move(MainViewModel.folderPath + $"\\{MainViewModel.selectedScheme.Name}.csv", MainViewModel.folderPath + $"\\{Message}.csv");
                MainViewModel.selectedScheme.Name = Message;
            }
            if (SelectedColumn != null && NewColumnName != null)
            {
                foreach(Column column in selectedScheme.Columns)
                {
                    if(column.Name == SelectedColumn)
                    {
                        column.Name = NewColumnName;
                    }
                }
            }
            string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(selectedScheme);
            File.WriteAllText(folderPath + $"\\{selectedScheme.Name}.json", jsonNewScheme);
            GetEquals(folderPath);
            DataNewTable = copyDataTable;
        });

    }
}
