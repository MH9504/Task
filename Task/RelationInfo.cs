using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    public class RelationInfo
    {
        public string ForeignTable { get; set; }    // הטבלה עם המפתח הזר
        public string ForeignKeyColumn { get; set; } 
        public string PrimaryTable { get; set; }   // הטבלה שממנה מגיע המפתח הראשי
        public string PrimaryKeyColumn { get; set; }
        public string Cardinality { get; set; }
        public string Description { get; set; }
    }
}
