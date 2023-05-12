using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace HardLab5
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public static Dictionary<TableScheme, Table> keyTables = new Dictionary<TableScheme, Table>();
        public int countOfTables;
        public int countOfSchemes;
        public static string folderPath = "";
        public static DataTable copyDataTable;
        public static string tableName;
        public  TableScheme selectedScheme;
        public Table selectedTable;

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

        public ICommand OpenDataFile => new DelegateCommand(param =>
        {
            keyTables.Clear();
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            folderPath = "";

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = openFolderDialog.SelectedPath;
            }
            if (folderPath == "")
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Text = "!СООБЩЕНИЕ! Вы не выбрали папку";
                return;
            }

            string folderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header = folderName;

            countOfSchemes = countOfTables = 0;
            GetEquals(folderPath);
        });


        public void GetEquals(string folderPath)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Clear();
            foreach (string fileScheme in Directory.EnumerateFiles(folderPath))
            {
                if (fileScheme.Contains("json"))
                {
                    countOfSchemes++;
                    TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
                    foreach (string fileTable in Directory.EnumerateFiles(folderPath))
                    {
                        if (fileTable.Contains("csv"))
                        {
                            try
                            {
                                Table table = TableData.GetInfoFromTable(tableScheme, fileTable);
                                keyTables.Add(tableScheme, table);
                                TreeViewItem treeViewItem = new TreeViewItem();
                                treeViewItem.Header = fileTable.Split('\\')[(fileTable.Split('\\').Length - 1)];
                                treeViewItem.Selected += TableSelected;
                                treeViewItem.Unselected += TableUnselected;

                                foreach (Column column in table.Scheme.Columns)
                                {
                                    treeViewItem.Items.Add(column.Name + " - " + column.Type + " - isPrimary: " + column.IsPrimary);
                                }
                                ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add(treeViewItem);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add(fileScheme.Split('\\')[(fileScheme.Split('\\').Length - 1)]);
                }
                if (fileScheme.Contains("csv"))
                {
                    countOfTables++;
                }
            }
            GetExeption();
        }

        private void GetExeption()
        {
            if (countOfTables > countOfSchemes || keyTables.Count < countOfTables)
            { 
                ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Text = "!СООБЩЕНИЕ! не все таблицы будут отображены, так как в файле недостаточно схем\\корректных схем";
            }
            else if (countOfTables < countOfSchemes)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Text = "!СООБЩЕНИЕ! найдены лишние схемы";
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).TextBox1.Clear();
            }
        }

        public void TableSelected(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DataGrid.Columns.Clear();
            tableName = ((TreeViewItem)sender).Header.ToString().Replace(".csv", "");
            DataTable dataTable = new DataTable();
            foreach (var keyTable in keyTables)
            {
                if (keyTable.Key.Name == tableName)
                {
                    selectedScheme = keyTable.Key;
                    selectedTable = keyTable.Value;
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
            copyDataTable = dataTable;
        }

        private void TableUnselected(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DataGrid.Columns.Clear();
        }

        public ICommand UpdateFile => new DelegateCommand(param =>
        {
            if(folderPath == "")
            {
                return;
            }
            GetEquals(folderPath);
        });

        public static WindowDB wind;
        public ICommand CreateNewDB => new DelegateCommand(param =>
        {
            if (wind == null)
            {
                wind = new WindowDB();
                wind.ShowDialog();
            }
            //else wind.Activate();
        });


        public static WindowTable wind1;
        public ICommand CreateNewTable => new DelegateCommand(param =>
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header != null)
            {
                //if (wind == null)
                //{
                    wind1 = new WindowTable();
                    wind1.ShowDialog();
                //}
                //else wind1.Activate();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите БД или создайте ее");
            }
        });

        public static WindowEditTable wind2;
        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (DataTable != null)
            {
                //if (wind == null)
                //{
                wind2 = new WindowEditTable();
                ViewModelEditTable vmEditTable = new ViewModelEditTable();
                wind2.DataContext = vmEditTable;

                vmEditTable.DataNewTable = copyDataTable;
                vmEditTable.TableName = tableName;
                ObservableCollection<string> names = new ObservableCollection<string>();
                foreach (var column in copyDataTable.Columns)
                {
                    names.Add(column.ToString());
                }
                vmEditTable.ListOfColumns = names;
                vmEditTable.ListOfColumns.Add("нет выбора");
                vmEditTable.selectedScheme = selectedScheme;
                vmEditTable.selectedTable = selectedTable;
                wind2.ShowDialog();
                //}
                //else wind1.Activate();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите таблицу");
            }
        });

    }
}
