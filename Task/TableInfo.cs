using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    public class TableInfo
    {
        public string TableName { get; set; }
        public List<string> PrimaryKeys { get; set; } = new List<string>();
        public List<string> Columns { get; set; } = new List<string>();
        public List<string> ForeignKeys { get; set; } = new List<string>();

    }
}
