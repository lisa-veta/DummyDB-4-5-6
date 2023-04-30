using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Data;

namespace HardLab5
{
    public partial class MainWindow : Window
    {
        //Dictionary<TableScheme, Table> keyTables = new Dictionary<TableScheme, Table>();
        

        ///public DataTable DT { get; set; }

        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        //private void OpenDataFile(object sender, RoutedEventArgs e)
        //{
            //FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            //string folderPath = "";

            //if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    folderPath = openFolderDialog.SelectedPath;
            //}

            //string folderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];

            //folderTree.Header = folderName;
            //string fileTable1 = "";
            //foreach (string fileScheme in Directory.EnumerateFiles(folderPath))
            //{
            //    if (fileScheme.Contains("json"))
            //    {
            //        TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
            //        foreach (string fileTable in Directory.EnumerateFiles(folderPath))
            //        {
            //            if (fileTable.Contains("csv"))
            //            {
            //                try
            //                {
            //                    fileTable1 = fileTable;
            //                    Table table = TableData.GetInfoFromTable(fileScheme, fileTable);
            //                    keyTables.Add(tableScheme, table);
            //                    TreeViewItem treeViewItem = new TreeViewItem();
            //                    treeViewItem.Header = fileTable1.Split('\\')[(fileTable1.Split('\\').Length - 1)];
            //                    TextBox1.Text = "кипит работа";
            //                    treeViewItem.Selected += TableSelected;
            //                    treeViewItem.Unselected += TableUnselected;
            //                    folderTree.Items.Add(treeViewItem);
            //                }
            //                catch { continue; }
            //            }
            //        }
            //        folderTree.Items.Add(fileScheme.Split('\\')[(fileScheme.Split('\\').Length - 1)]);
            //    }
            //}
       // }

        //public void TableSelected(object sender, RoutedEventArgs e)
        //{
        //    // DataGrid.ItemsSource = LoadCollectionData();
        //    DataGrid.Columns.Clear();
        //    string tableName = ((TreeViewItem)sender).Header.ToString().Replace(".csv", "");
        //    TextBox1.Text =  $"вы нажали на {tableName}";
        //    foreach (var keyTable in keyTables)
        //    {
        //        //DataGrid.ItemsSource = DataTable.DefaultView;

        //        List<RowAdapter> rowsData = new List<RowAdapter>();
        //        foreach (Row row in keyTable.Value.Rows)
        //        {
        //            List<object> rowData = new List<object>();
        //            foreach (object cell in row.Data.Values)
        //            {
        //                rowData.Add(cell);
        //                //dt.Rows.Add(row);
        //            }
        //            rowsData.Add(new RowAdapter() { Data = rowData });
        //        }

        //        DataGrid.ItemsSource = rowsData;

        //        for (int i = 0; i < keyTable.Key.Columns.Count; i++)
        //        {
        //            DataGridTextColumn tableTextColumn = new DataGridTextColumn()
        //            {
        //                Header = keyTable.Key.Columns[i].Name,
        //                Binding = new System.Windows.Data.Binding($"Data[{i}]")
        //            };

        //            DataGrid.Columns.Add(tableTextColumn);
        //        }
        //        //DataGrid.ItemsSource = dt.DefaultView;
        //        break;
        //    }

        //}

        //private class RowAdapter
        //{
        //    public List<Object> Data { get; set; }
        //}
        //private void TableUnselected(object sender, RoutedEventArgs e)
        //{
        //    DataGrid.Columns.Clear();
        //}

    }
}
