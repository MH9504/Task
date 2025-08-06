using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Task
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ////Application.Run(new Form1());
            //string connectionString = "Server=localhost;Database=AdventureWorks;Trusted_Connection=True;";

            //var loader = new DatabaseSchemaLoader(connectionString);
            //loader.LoadSchema();

            //var tables = loader.Tables;
            //var relations = loader.Relations;

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            ////
            //Application.Run(new Form1(tables, relations));
            // טען את מחרוזת החיבור מ-App.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AdventureWorksConnectionString"].ConnectionString;

            // טען את הטבלאות והקשרים מהמסד
            var loader = new DatabaseSchemaLoader(connectionString);
            loader.LoadSchema();


            List<TableInfo> tables = loader.Tables;
            List<RelationInfo> relations = loader.Relations;
            foreach (var rel in relations)
            {
                var foreignTable = tables.FirstOrDefault(t => t.TableName == rel.ForeignTable);
                if (foreignTable != null && !foreignTable.ForeignKeys.Contains(rel.ForeignKeyColumn))
                {
                    foreignTable.ForeignKeys.Add(rel.ForeignKeyColumn);
                }
            }

            foreach (var rel in relations)
            {
                System.Diagnostics.Debug.WriteLine($"{rel.PrimaryTable} -> {rel.ForeignTable} : {rel.Cardinality}");
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(tables, relations));
        }
    }
}
