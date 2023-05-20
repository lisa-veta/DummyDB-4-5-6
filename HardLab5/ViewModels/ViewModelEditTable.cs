using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using DummyDB.Core;
using HardLab5.ViewModels;

namespace HardLab5
{
    class ViewModelEditTable : BaseViewModel
    {
        public ObservableCollection<ViewModelEditTable> Items { get; } = new ObservableCollection<ViewModelEditTable>();

        public string folderPath;

        private List<string> _names;
        public List<string> ListOfColumns
        {
            get { return _names; }
            set
            {
                _names = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Controls.DataGrid DataGrid { get; set; }
        public TableScheme selectedScheme;
        public Table selectedTable;
        private DataTable _dataNewTable;
        public DataTable DataNewTable
        {
            get { return _dataNewTable; }
            set
            {
                _dataNewTable = value;
                OnPropertyChanged();
            }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                OnPropertyChanged();
            }
        }

        private string _newColumnName;
        public string NewColumnName
        {
            get { return _newColumnName; }
            set
            {
                _newColumnName = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumn;
        public string SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                _selectedColumn = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumnDelete;
        public string SelectedColumnDelete
        {
            get { return _selectedColumnDelete; }
            set
            {
                _selectedColumnDelete = value;
                OnPropertyChanged();
            }
        }

        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                _columnName = value;
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

        public ICommand AddRow => new DelegateCommand(param =>
        {
            DataRow newRow = DataNewTable.NewRow();
            DataNewTable.Rows.Add(newRow);
        });

        public ICommand CreateNewColumn => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelEditTable());
        });

        public ICommand RemoveNewColumn => new DelegateCommand(param =>
        {
            Items.RemoveAt(Items.Count - 1);
        });

        public ICommand DeleteRow => new DelegateCommand(param =>
        {
            if(DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Строка для удаления не была выделена");
                return;
            }
            RemoveRow();
        });

        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (GetEx())
            {
                return;
            }
            if (MainViewModel.tableName != TableName && TableName != null && TableName != "")
            {
                FileRewriter.RewriteFileName(folderPath, selectedScheme, TableName);
            }
            if(AddNewColumn())
            {
                System.Windows.MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            if (RemoveColumn())
            {
                return;
            }
            FileRewriter.RewriteJson(folderPath, selectedScheme);
            UpdateTable();
        });


        public ICommand EditRows => new DelegateCommand(param =>
        {
            if (EditTableData())
            {
                return;
            }
            UpdateTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        });

        private bool GetEx()
        {
            if (SelectedColumn != null && SelectedColumn != "" && NewColumnName != null && NewColumnName != "" && SelectedColumn != "нет выбора")
            {
                if (CheckEqualsNames("EditColumnName"))
                {
                    System.Windows.MessageBox.Show("Попытка переименования столбца в уже существующее имя");
                    return true;
                }
            }
            if (Items != null)
            {
                if (CheckEqualsNames("CreateNewColumn"))
                {
                    System.Windows.MessageBox.Show("Попытка добавления столбца с уже существующим именем");
                    return true;
                }
            }
            return false;
        }

        private bool AddNewColumn()
        {
            if(Items != null)
            {
                foreach (var item in Items)
                {
                    Column column = new Column();
                    if (item.ColumnName == null || item.ColumnName == "" || item.Type == null)
                    {
                        return true;
                    }
                    column.Name = item.ColumnName;
                    column.Type = item.Type;
                    column.IsPrimary = item.Primary;
                    selectedScheme.Columns.Add(column);
                    ColumnAdder.AddColumnInTable(column, selectedTable);
                    FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                }
            }
            return false;
        }

        private void RemoveRow()
        {
            for (int i = 0; i < DataGrid.Columns.Count; i++)
            {
                if (DataGrid.Items[i] == DataGrid.SelectedItem)
                {
                    DialogResult dialogResult = MessageBox.Show($"Вы уверены, что хотите безвозвратно удалить {i + 1} строку?", "Подтверждение действий", MessageBoxButtons.YesNo); ;
                    if (dialogResult == DialogResult.Yes)
                    {
                        selectedTable = ElementRemover.DeleteTableRow(i, selectedTable);
                        FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                        UpdateTable();
                        return;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }
            }
        }

        private bool RemoveColumn()
        {
            if (SelectedColumnDelete != null && SelectedColumnDelete != "нет выбора")
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите безвозвратно удалить столбец?", "Подтверждение действий", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    ElementRemover.DeleteColumn(selectedScheme, selectedTable, SelectedColumnDelete);
                    FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool CheckEqualsNames(string location)
        {
            if(location == "EditColumnName")
            {
                foreach (Column column in selectedScheme.Columns)
                {
                    if (column.Name == NewColumnName)
                    {
                        return true;
                    }
                }
                if (SelectedColumn != null && SelectedColumn != "" && NewColumnName != null && NewColumnName != "" && SelectedColumn != "нет выбора")
                {
                    ElementRemover.RemoveColumnName(selectedScheme, SelectedColumn, NewColumnName);
                }
            }
            else if(location == "CreateNewColumn")
            {
                foreach (var item in Items)
                {
                    foreach (Column column in selectedScheme.Columns)
                    {
                        if ((column.Name == item.ColumnName || item.ColumnName == NewColumnName) && NewColumnName != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void UpdateTable()
        {
            DataNewTable.Clear();
            DataTable dataTable = new DataTable();
            foreach (Column column in selectedScheme.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }

            for (int i = 0; i < selectedTable.Rows.Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (var rows in selectedTable.Rows[i].Data)
                {
                    newRow[rows.Key.Name] = rows.Value;
                }
                dataTable.Rows.Add(newRow);
            }
            DataNewTable = dataTable;
            ClearData();
        }

        private void ClearData()
        {
            Items.Clear();
            NewColumnName = null;
            SelectedColumn = null;
            SelectedColumnDelete = null;
            ListOfColumns.Clear();

            List<string> names = new List<string>();
            foreach (var column in DataNewTable.Columns)
            {
                names.Add(column.ToString());
            }
            names.Add("нет выбора");
            ListOfColumns = names;
        }

        private bool EditTableData()
        {
            int count = selectedTable.Rows.Count - 1;
            for (int i = 0; i < DataNewTable.Rows.Count; i++)
            {
                for (int j = 0; j < selectedScheme.Columns.Count; j++)
                {
                    if (i >= selectedTable.Rows.Count)
                    {
                        selectedTable.Rows.Add(new Row() { Data = new Dictionary<Column, object>() });
                    }
                    object data = CheckCorrectData(i, j);
                    if (data == null && i < count)
                    {
                        DataNewTable.Rows[i][selectedScheme.Columns[j].Name] = selectedTable.Rows[i].Data[selectedScheme.Columns[j]];
                        return true;
                    }
                    else if (data == null) { return true; }
                    selectedTable.Rows[i].Data[selectedScheme.Columns[j]] = data;
                    DataNewTable.Rows[i][selectedScheme.Columns[j].Name] = data;
                }
            }
            return false;
        }

        private object CheckCorrectData(int numOfRow, int numOfColumn)
        {
            string data = DataNewTable.Rows[numOfRow][selectedScheme.Columns[numOfColumn].Name].ToString();
            switch (selectedScheme.Columns[numOfColumn].Type)
            {
                case "uint":
                    {
                        return TryUint(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "int":
                    {
                        return TryInt(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "double":
                    {
                        return TryDouble(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "datetime":
                    {
                        return TryDateTime(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "string":
                    {
                        return data;
                    }
            }
            return null;
        }

       private object TryUint(string data, int numOfRow, int numOfColumn )
        {
            if (uint.TryParse(data, out uint number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1))
            {
                return 0;
            }
            return null;
        }
        private object TryInt(string data, int numOfRow, int numOfColumn)
        {
            if (int.TryParse(data, out int number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1))
            {
                return 0;
            }
            return null;
        }
        private object TryDouble(string data, int numOfRow, int numOfColumn)
        {
            if (double.TryParse(data, out double number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1))
            {
                return 0;
            }
            return null;
        }

        private object TryDateTime(string data, int numOfRow, int numOfColumn)
        {
            if (DateTime.TryParse(data, out DateTime number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1))
            {
                return DateTime.MinValue;
            }
            return null;
        }

        private bool ShowMessage(int numOfRow, int numOfColumn)
        {
            DialogResult dialogResult = MessageBox.Show($"В строке {numOfRow} столбце {numOfColumn} введены некорректные данные или найдены пустые ячейки, они будут заполнены значениями по умолчанию", "Подтверждение действий", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else if(dialogResult == DialogResult.No)
            {
                return false;
            }
            return false;
        }
    }
}
