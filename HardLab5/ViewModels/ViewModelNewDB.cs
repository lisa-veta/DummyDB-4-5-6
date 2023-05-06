using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace HardLab5
{
    class ViewModelNewDB : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string folderPathDB = "";

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
                folderPathDB = openFolderDialog.SelectedPath;
            }
            folderPathDB += "\\" + Message;
            Message1 = "Путь к папке:\n" + folderPathDB;
        });

        public ICommand Create => new DelegateCommand(param =>
        {
            if(folderPathDB == "" || Message == "")
            {
                MessageBox.Show("Вы не ввели имя БД или не выбрали путь!");
                return;
            }
            Directory.CreateDirectory(folderPathDB);
            MessageBox.Show("Папка для новой БД успешно создана! " + Message1 + "\n");
            Message = "";
            string folderName = folderPathDB.Split('\\')[folderPathDB.Split('\\').Length - 1];
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header = folderName;
            MainViewModel.folderPath = folderPathDB;
        });
    }
}
