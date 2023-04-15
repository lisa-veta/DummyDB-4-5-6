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

namespace HardLab5
{
    
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

            foreach(string fileScheme in Directory.EnumerateFiles(folderPath))
            {
                if (fileScheme.Contains("json"))
                {
                    TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
                    foreach(string fileTable in Directory.EnumerateFiles(folderPath))
                    {
                        if (fileScheme.Contains("csv"))
                        {
                            try
                            {
                                Table table = TableData.GetInfoFromTable(fileScheme, fileTable);
                                keyTables.Add(tableScheme, table);
                            }
                            catch { continue; }
                        }
                    }
                    folderTree.Items.Add(fileScheme.Split('\\')[(fileScheme.Split('\\').Length - 1)]);
                }
                
            }
        }

    }


}
