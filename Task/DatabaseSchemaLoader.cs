using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    public class DatabaseSchemaLoader
    {
        private readonly string _connectionString;

        public DatabaseSchemaLoader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<TableInfo> Tables { get; private set; }
        public List<RelationInfo> Relations { get; private set; }

        //public void LoadSchema()
        //{
        //    Tables = new List<TableInfo>();
        //    Relations = new List<RelationInfo>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();

        //        // שליפת כל הטבלאות
        //        string queryTables = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

        //        using (SqlCommand cmd = new SqlCommand(queryTables, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Tables.Add(new TableInfo
        //                {
        //                    TableName = reader.GetString(0)
        //                });
        //            }
        //        }

        //        // שליפת מפתחות ראשיים
        //        string queryPKs = @"
        //        SELECT KU.TABLE_NAME, KU.COLUMN_NAME
        //        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
        //        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KU
        //            ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
        //        WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'";

        //        using (SqlCommand cmd = new SqlCommand(queryPKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string tableName = reader.GetString(0);
        //                string columnName = reader.GetString(1);

        //                var table = Tables.Find(t => t.TableName == tableName);
        //                if (table != null)
        //                {
        //                    table.PrimaryKeys.Add(columnName);
        //                }
        //            }
        //        }

        //        // שליפת קשרים (Foreign Keys)
        //        string queryFKs = @"
        //        SELECT
        //            FK.TABLE_NAME AS ForeignTable,
        //            CU.COLUMN_NAME AS ForeignColumn,
        //            PK.TABLE_NAME AS PrimaryTable,
        //            PT.COLUMN_NAME AS PrimaryColumn
        //        FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
        //        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
        //            ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
        //        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
        //            ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
        //        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU
        //            ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
        //        INNER JOIN (
        //            SELECT i1.TABLE_NAME, i2.COLUMN_NAME
        //            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
        //            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2
        //                ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
        //            WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
        //        ) PT ON PT.TABLE_NAME = PK.TABLE_NAME";

        //        using (SqlCommand cmd = new SqlCommand(queryFKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Relations.Add(new RelationInfo
        //                {
        //                    ForeignTable = reader.GetString(0),
        //                    ForeignKeyColumn = reader.GetString(1),
        //                    PrimaryTable = reader.GetString(2),
        //                    PrimaryKeyColumn = reader.GetString(3),
        //                    Cardinality = "1:n" // הוספת מאפיין חדש - תוודאי שהוספת אותו למחלקה
        //                });
        //            }
        //        }
        //        }
        //        }
        //public void LoadSchema()
        //{
        //    Tables = new List<TableInfo>();
        //    Relations = new List<RelationInfo>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();

        //        // שליפת כל הטבלאות
        //        string queryTables = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

        //        using (SqlCommand cmd = new SqlCommand(queryTables, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Tables.Add(new TableInfo
        //                {
        //                    TableName = reader.GetString(0)
        //                });
        //            }
        //        }

        //        // שליפת מפתחות ראשיים
        //        string queryPKs = @"
        //    SELECT KU.TABLE_NAME, KU.COLUMN_NAME
        //    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
        //    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KU
        //        ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
        //    WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'";

        //        using (SqlCommand cmd = new SqlCommand(queryPKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string tableName = reader.GetString(0);
        //                string columnName = reader.GetString(1);

        //                var table = Tables.Find(t => t.TableName == tableName);
        //                if (table != null)
        //                {
        //                    table.PrimaryKeys.Add(columnName);
        //                }
        //            }
        //        }

        //        // שליפת קשרים (Foreign Keys) כולל בדיקת Nullable לעמודת מפתח זר
        //        string queryFKs = @"
        //    SELECT
        //        FK.TABLE_NAME AS ForeignTable,
        //        CU.COLUMN_NAME AS ForeignColumn,
        //        COLUMNS.IS_NULLABLE,
        //        PK.TABLE_NAME AS PrimaryTable,
        //        PT.COLUMN_NAME AS PrimaryColumn
        //    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
        //    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
        //        ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
        //    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
        //        ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
        //    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU
        //        ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
        //    INNER JOIN (
        //        SELECT i1.TABLE_NAME, i2.COLUMN_NAME
        //        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
        //        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2
        //            ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
        //        WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
        //    ) PT ON PT.TABLE_NAME = PK.TABLE_NAME AND PT.COLUMN_NAME = CU.COLUMN_NAME
        //    INNER JOIN INFORMATION_SCHEMA.COLUMNS COLUMNS
        //        ON COLUMNS.TABLE_NAME = CU.TABLE_NAME AND COLUMNS.COLUMN_NAME = CU.COLUMN_NAME";

        //        using (SqlCommand cmd = new SqlCommand(queryFKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string foreignTable = reader.GetString(0);
        //                string foreignColumn = reader.GetString(1);
        //                string isNullable = reader.GetString(2); // "YES" או "NO"
        //                string primaryTable = reader.GetString(3);
        //                string primaryColumn = reader.GetString(4);

        //                // קביעת קרדינליות לפי Nullable
        //                string cardinality = isNullable == "YES" ? "0:n" : "1:n";

        //                Relations.Add(new RelationInfo
        //                {
        //                    ForeignTable = foreignTable,
        //                    ForeignKeyColumn = foreignColumn,
        //                    PrimaryTable = primaryTable,
        //                    PrimaryKeyColumn = primaryColumn,
        //                    Cardinality = cardinality
        //                });
        //            }
        //        }
        //    }
        //}
        //public void LoadSchema()
        //{
        //    Tables = new List<TableInfo>();
        //    Relations = new List<RelationInfo>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();

        //        // שלב 1: שליפת כל הטבלאות
        //        string queryTables = @"
        //    SELECT TABLE_NAME
        //    FROM INFORMATION_SCHEMA.TABLES
        //    WHERE TABLE_TYPE = 'BASE TABLE'
        //    ORDER BY TABLE_NAME";

        //        using (SqlCommand cmd = new SqlCommand(queryTables, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Tables.Add(new TableInfo
        //                {
        //                    TableName = reader.GetString(0),
        //                    PrimaryKeys = new List<string>(),
        //                    Columns = new List<string>()
        //                });
        //            }
        //        }

        //        // שלב 2: שליפת כל העמודות
        //        string queryColumns = @"
        //    SELECT TABLE_NAME, COLUMN_NAME
        //    FROM INFORMATION_SCHEMA.COLUMNS
        //    ORDER BY TABLE_NAME, ORDINAL_POSITION";

        //        using (SqlCommand cmd = new SqlCommand(queryColumns, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string tableName = reader.GetString(0);
        //                string columnName = reader.GetString(1);

        //                var table = Tables.Find(t => t.TableName == tableName);
        //                if (table != null)
        //                {
        //                    table.Columns.Add(columnName);
        //                }
        //            }
        //        }

        //        // שלב 3: שליפת מפתחות ראשיים (Primary Keys)
        //        string queryPKs = @"
        //    SELECT KU.TABLE_NAME, KU.COLUMN_NAME
        //    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
        //    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
        //        ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
        //       AND TC.TABLE_NAME = KU.TABLE_NAME
        //    WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
        //    ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION";

        //        using (SqlCommand cmd = new SqlCommand(queryPKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string tableName = reader.GetString(0);
        //                string columnName = reader.GetString(1);

        //                var table = Tables.Find(t => t.TableName == tableName);
        //                if (table != null)
        //                {
        //                    table.PrimaryKeys.Add(columnName);
        //                }
        //            }
        //        }

        //        // שלב 4: שליפת קשרים (Foreign Keys) עם Nullable ו-Cardinality
        //        string queryFKs = @"
        //    SELECT
        //        FK.TABLE_NAME AS ForeignTable,
        //        FKC.COLUMN_NAME AS ForeignColumn,
        //        COLS.IS_NULLABLE,
        //        PK.TABLE_NAME AS PrimaryTable,
        //        PKC.COLUMN_NAME AS PrimaryColumn
        //    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
        //    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
        //        ON RC.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
        //    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
        //        ON RC.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
        //    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE FKC
        //        ON RC.CONSTRAINT_NAME = FKC.CONSTRAINT_NAME
        //    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE PKC
        //        ON RC.UNIQUE_CONSTRAINT_NAME = PKC.CONSTRAINT_NAME
        //       AND FKC.ORDINAL_POSITION = PKC.ORDINAL_POSITION
        //    INNER JOIN INFORMATION_SCHEMA.COLUMNS COLS
        //        ON COLS.TABLE_NAME = FKC.TABLE_NAME
        //       AND COLS.COLUMN_NAME = FKC.COLUMN_NAME
        //    ORDER BY ForeignTable, ForeignColumn";

        //        using (SqlCommand cmd = new SqlCommand(queryFKs, conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                string foreignTable = reader.GetString(0);
        //                string foreignColumn = reader.GetString(1);
        //                string isNullable = reader.GetString(2);
        //                string primaryTable = reader.GetString(3);
        //                string primaryColumn = reader.GetString(4);

        //                string cardinality = isNullable.Equals("YES", StringComparison.OrdinalIgnoreCase)
        //                    ? "0:n"
        //                    : "1:n";

        //                Relations.Add(new RelationInfo
        //                {
        //                    ForeignTable = foreignTable,
        //                    ForeignKeyColumn = foreignColumn,
        //                    PrimaryTable = primaryTable,
        //                    PrimaryKeyColumn = primaryColumn,
        //                    Cardinality = cardinality
        //                });
        //            }
        //        }
        //    }
        //}
        public void LoadSchema()
        {
            Tables = new List<TableInfo>();
            Relations = new List<RelationInfo>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // שלב 1: שליפת כל הטבלאות
                string queryTables = @"
            SELECT TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME";

                using (SqlCommand cmd = new SqlCommand(queryTables, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tables.Add(new TableInfo
                        {
                            TableName = reader.GetString(0),
                            PrimaryKeys = new List<string>(),
                            Columns = new List<string>()
                        });
                    }
                }

                // שלב 2: שליפת כל העמודות
                string queryColumns = @"
            SELECT TABLE_NAME, COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            ORDER BY TABLE_NAME, ORDINAL_POSITION";

                using (SqlCommand cmd = new SqlCommand(queryColumns, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        string columnName = reader.GetString(1);

                        var table = Tables.Find(t => t.TableName == tableName);
                        if (table != null)
                        {
                            table.Columns.Add(columnName);
                        }
                    }
                }

                // שלב 3: שליפת מפתחות ראשיים (Primary Keys)
                string queryPKs = @"
            SELECT KU.TABLE_NAME, KU.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
                ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
               AND TC.TABLE_NAME = KU.TABLE_NAME
            WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION";

                using (SqlCommand cmd = new SqlCommand(queryPKs, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        string columnName = reader.GetString(1);

                        var table = Tables.Find(t => t.TableName == tableName);
                        if (table != null)
                        {
                            table.PrimaryKeys.Add(columnName);
                        }
                    }
                }

                // שלב 4: שליפת קשרים (Foreign Keys) עם Nullable + Unique => Cardinality מלא
                string queryFKs = @"
            SELECT
                FK.TABLE_NAME AS ForeignTable,
                FKC.COLUMN_NAME AS ForeignColumn,
                COLS.IS_NULLABLE,
                PK.TABLE_NAME AS PrimaryTable,
                PKC.COLUMN_NAME AS PrimaryColumn,
                CASE 
                    WHEN idx.is_unique = 1 THEN 
                        CASE WHEN COLS.IS_NULLABLE = 'YES' THEN '0:1' ELSE '1:1' END
                    ELSE 
                        CASE WHEN COLS.IS_NULLABLE = 'YES' THEN '0:n' ELSE '1:n' END
                END AS Cardinality
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
                ON RC.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
                ON RC.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE FKC
                ON RC.CONSTRAINT_NAME = FKC.CONSTRAINT_NAME
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE PKC
                ON RC.UNIQUE_CONSTRAINT_NAME = PKC.CONSTRAINT_NAME
               AND FKC.ORDINAL_POSITION = PKC.ORDINAL_POSITION
            INNER JOIN INFORMATION_SCHEMA.COLUMNS COLS
                ON COLS.TABLE_NAME = FKC.TABLE_NAME
               AND COLS.COLUMN_NAME = FKC.COLUMN_NAME
            LEFT JOIN sys.index_columns ic
                ON ic.object_id = OBJECT_ID(QUOTENAME(FKC.TABLE_SCHEMA) + '.' + QUOTENAME(FKC.TABLE_NAME))
                AND ic.column_id = COLUMNPROPERTY(OBJECT_ID(QUOTENAME(FKC.TABLE_SCHEMA) + '.' + QUOTENAME(FKC.TABLE_NAME)), FKC.COLUMN_NAME, 'ColumnId')
            LEFT JOIN sys.indexes idx
                ON idx.object_id = ic.object_id
                AND idx.index_id = ic.index_id
            ORDER BY ForeignTable, ForeignColumn";

                using (SqlCommand cmd = new SqlCommand(queryFKs, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string foreignTable = reader.GetString(0);
                        string foreignColumn = reader.GetString(1);
                        string primaryTable = reader.GetString(3);
                        string primaryColumn = reader.GetString(4);
                        string cardinality = reader.GetString(5); // מוכן מה-SQL

                        Relations.Add(new RelationInfo
                        {
                            ForeignTable = foreignTable,
                            ForeignKeyColumn = foreignColumn,
                            PrimaryTable = primaryTable,
                            PrimaryKeyColumn = primaryColumn,
                            Cardinality = cardinality
                        });
                    }
                }
            }
        }

    }
}
