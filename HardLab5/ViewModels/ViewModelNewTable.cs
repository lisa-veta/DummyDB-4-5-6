using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
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

        public ICommand CreateTable => new DelegateCommand(param =>
        {
            if(Message == null || Items == null)
            {
                MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            string name = Message;
            List<Column> columns = new List<Column>();
            foreach (var item in Items)
            {
                Column column = new Column();
                if(item.Text == null || item.Type == null)
                {
                    MessageBox.Show("Данные не заполнены до конца");
                    return;
                }
                column.Name = item.Text;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                columns.Add(column);
            }
            //TableScheme.CreateFile(columns, name, MainViewModel.folderPath);
            //MainViewModel.GetEquals(MainViewModel.folderPath);

            var tableScheme = new TableScheme
            {
                Name = name,
                Columns = columns
            };
            TableScheme.SafeNewData(tableScheme);
            //string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);
            //string pathOfScheme = MainViewModel.folderPath + $"\\{name}.json";
            //string pathOfTable = MainViewModel.folderPath + $"\\{name}.csv";
            //File.WriteAllText(pathOfScheme, jsonNewScheme);
            //File.Create(pathOfTable);

            System.Windows.MessageBox.Show("Успешно!");
            //CloseWindow(param);
        });
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
