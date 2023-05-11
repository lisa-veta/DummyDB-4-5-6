using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace HardLab5
{
    class ViewModelNewTable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        
       // private bool _isChecked;
        public ObservableCollection<ViewModelNewTable> Items { get; } = new ObservableCollection<ViewModelNewTable>();

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
            if (Message == null || Message == "" || Items == null)
            {
                MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            string name = Message;
            List<Column> columns = new List<Column>();
            foreach (var item in Items)
            {
                Column column = new Column();
                if(item.Text == null || item.Text == "" || item.Type == null)
                {
                    MessageBox.Show("Данные не заполнены до конца");
                    return;
                }
                column.Name = item.Text;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                columns.Add(column);
                
            }
            TableScheme tableScheme = new TableScheme
            {
                Name = name,
                Columns = columns
            };

            if (CheckEqualsNames(tableScheme))
            {
                System.Windows.MessageBox.Show("Найдены столбцы с повторяющимися именами");
                return;
            }

            string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);
            string pathOfScheme = MainViewModel.folderPath + $"\\{tableScheme.Name}.json";
            File.WriteAllText(pathOfScheme, jsonNewScheme);

            string pathTable = MainViewModel.folderPath + $"\\{tableScheme.Name}.csv";
            string newFile = AddColumnInTable(columns);
            File.WriteAllText(pathTable, newFile.ToString());

            System.Windows.MessageBox.Show("Успешно!");
        });

        public string AddColumnInTable(List<Column> columns)
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
                    case "float":
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
                            newFile.Append($"{null}");
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

        public bool CheckEqualsNames(TableScheme tableScheme)
        {
            for(int i = 0; i < Items.Count()-1; i++)
            {
                for(int j = i+1; j < Items.Count(); j++)
                {
                    if (Items[i].Text == Items[j].Text)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //private void CloseWindow(object obj)
        //{
        //    Close(obj, true);
        //}
        //void Close(object obj, bool dialogResult)
        //{
        //    var w = ((WindowTable)(System.Windows.Window)obj);
        //    w.DialogResult = dialogResult;
        //    w.Close();
        //}
    }
}
