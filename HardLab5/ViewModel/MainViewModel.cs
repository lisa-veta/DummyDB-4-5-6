using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HardLab5
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        Dictionary<TableScheme, Table> keyTables = new Dictionary<TableScheme, Table>();

        private DataTable _dataTable;
        public DataTable DataTable
        {
            get { return _dataTable; }
            set
            {
                _dataTable = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenSourceClick => new DelegateCommand(param =>
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            string folderPath = "";

            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = openFolderDialog.SelectedPath;
            }

            string folderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];

            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header = folderName;
            string fileTable1 = "";
            foreach (string fileScheme in Directory.EnumerateFiles(folderPath))
            {
                if (fileScheme.Contains("json"))
                {
                    TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
                    foreach (string fileTable in Directory.EnumerateFiles(folderPath))
                    {
                        if (fileTable.Contains("csv"))
                        {
                            try
                            {
                                fileTable1 = fileTable;
                                Table table = TableData.GetInfoFromTable(fileScheme, fileTable);
                                keyTables.Add(tableScheme, table);
                                TreeViewItem treeViewItem = new TreeViewItem();
                                treeViewItem.Header = fileTable1.Split('\\')[(fileTable1.Split('\\').Length - 1)];
                                ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Text = "кипит работа";
                                treeViewItem.Selected += TableSelected;
                                treeViewItem.Unselected += TableUnselected;
                                ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add(treeViewItem);
                            }
                            catch { continue; }
                        }
                    }
                    ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add(fileScheme.Split('\\')[(fileScheme.Split('\\').Length - 1)]);
                }
            }
        });


        public void TableSelected(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DataGrid.Columns.Clear();
            string tableName = ((TreeViewItem)sender).Header.ToString().Replace(".csv", "");
            DataTable dataTable = new DataTable();
            ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Text = $"вы жмали на {tableName}";
            foreach (var keyTable in keyTables)
            {
                if (keyTable.Key.Name == tableName)
                {
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
            DataTable = dataTable;
        }

        private void TableUnselected(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DataGrid.Columns.Clear();
        }
    }
}
