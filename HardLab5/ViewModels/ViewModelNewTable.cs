using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
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

        private string _text;
       // private bool _isChecked;
        public ObservableCollection<ViewModelNewTable> Items { get; } = new ObservableCollection<ViewModelNewTable>();
        Dictionary<string, string> ColumnsOfNewTable = new Dictionary<string, string>();

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

        public ICommand CreateColumns => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelNewTable());

        });

        public ICommand CreateTable => new DelegateCommand(param =>
        {
            ColumnsOfNewTable.Clear();
            foreach (var item in Items)
            {
                ColumnsOfNewTable.Add(item.Text, item.Type);
                Message = item.Type;
            }
            foreach(var key in ColumnsOfNewTable.Values)
            {
                Message = key;
                break;
            }

        });
    }
}
