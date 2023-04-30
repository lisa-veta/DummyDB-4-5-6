using System.Collections.Generic;

namespace HardLab5
{
    public class Table
    {
        public List<Row> Rows { get; set; }
        public Table()
        {
            Rows = new List<Row>();
        }
        public TableScheme Scheme { get; set; }
    }
}
