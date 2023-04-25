using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// <summary>
    ///  DataContext="{Binding DataTable}" ItemsSource="{Binding DefaultView}"
    ///  
    /// 
    /// Selected="TableSelected"
               /// Unselected="TableUnselected"
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<TableScheme, Table> keyTables = new Dictionary<TableScheme, Table>();

       

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenDataFile(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            string folderPath = "";

            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = openFolderDialog.SelectedPath; 
            }

            string folderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];

            folderTree.Header = folderName;
            string fileTable1 = "";
            foreach (string fileScheme in Directory.EnumerateFiles(folderPath))
            {
                if (fileScheme.Contains("json"))
                {
                    TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
                    foreach(string fileTable in Directory.EnumerateFiles(folderPath))
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
                                TextBox1.Text = fileTable1.Split('\\')[(fileTable1.Split('\\').Length - 1)];
                                treeViewItem.Selected += TableSelected;
                                treeViewItem.Unselected += TableUnselected;
                                folderTree.Items.Add(treeViewItem);
                            }
                            catch { continue; }
                        }
                    }
                    folderTree.Items.Add(fileScheme.Split('\\')[(fileScheme.Split('\\').Length - 1)]);
                }
            }
        }

        public void TableSelected(object sender, RoutedEventArgs e)
        {
            //DataTable.ItemsSource = LoadCollectionData();
            //DataTable.Columns.Clear();
            string tableName = ((TreeViewItem)sender).Header.ToString();
            TextBox1.Text = tableName;

            //foreach (var keyTable in keyTables)
            //{
            //    if (keyTable.Key.Name == tableName)
            //    {
            //        //List<RowAdapter> rowsData = new List<RowAdapter>();
            //        DataTable dt = new DataTable();
            //        foreach (Row row in keyTable.Value.Rows)
            //        {
            //            //List<object> rowData = new List<object>();
            //            foreach (object cell in row.Data.Values)
            //            {
            //                //rowData.Add(cell);
            //                dt.Rows.Add(row);
            //            }
            //            //rowsData.Add(new RowAdapter() { Data = rowData });
            //        }

            //        //DataTable.ItemsSource = rowsData;

            //        for (int i = 0; i < keyTable.Key.Columns.Count; i++)
            //        {
            //            DataGridTextColumn tableTextColumn = new DataGridTextColumn()
            //            {
            //                Header = keyTable.Key.Columns[i].Name,
            //                Binding = new System.Windows.Data.Binding($"DataTables[{i}]")
            //            };

            //            DataTable.Columns.Add(tableTextColumn);
            //        }
            //        DataTable.ItemsSource = dt.DefaultView;
            //        break;
            //    }
            //}
        }

        private void TableUnselected(object sender, RoutedEventArgs e)
        {
            DataTable.Columns.Clear();
        }

    }
    public class RowAdapter
    {
        public List<Object> Data { get; set; }
    }

}
