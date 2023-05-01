using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace HardLab5
{
    class MainViewModelNewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string folderPath = "";

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

        public string Message1
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        public ICommand CreateNewFile => new DelegateCommand(param =>
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();

            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = openFolderDialog.SelectedPath;
            }
            folderPath += "\\" + Message;
            Message1 = "Путь к папке:\n" + folderPath;
        });

        public ICommand Create => new DelegateCommand(param =>
        {
            Directory.CreateDirectory(folderPath);
            MessageBox.Show("Папка для новой БД успешно создана! " + Message1 + "\n");
            Message = "";
        });
    }
}
