using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MiniKreuzwortraetsel
{
    class MySqlQueries
    {
        static MySqlConnection conn = new MySqlConnection("Server=localhost;Database=minikreuzwortraetsel;Uid=root;Pwd=;");
        public static List<string> SHOW_TABLES()
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SHOW TABLES", conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string> tableNames = new List<string>();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }
            reader.Close();
            conn.Close();

            return tableNames;
        }
        // TODO: Generalize this method
        public static void CREATE_TABLE(string newTableName)
        {
            conn.Open();
            string query = " CREATE TABLE " + newTableName + "(" + "\n" +
                           " ID          INT NOT NULL AUTO_INCREMENT,                   \n" +
                           " Question     VARCHAR(255) NOT NULL,                         \n" +
                           " Answer      VARCHAR(255) NOT NULL,                         \n" +
                           " PRIMARY KEY (ID) );                                        \n";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static void DROP_TABLE(string table)
        {
            conn.Open();
            string query = " DROP TABLE " + table;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static List<string[]> SELECT(string table)
        {
            conn.Open();
            string query = " SELECT * FROM " + table;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string[]> rows = new List<string[]>();
            while (reader.Read())
            {
                rows.Add(new string[reader.FieldCount]);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rows.Last()[i] = reader.GetString(i);
                }
            }
            conn.Close();
            return rows;
        }
        public static void INSERT(string table, string[] columnNames, string[] values)
        {
            conn.Open();
            string query = "INSERT INTO " + table + "(";

            for (int i = 0; i < values.Length; i++)
                query += columnNames[i] + ", ";

            query = query.Substring(0, query.Length - 2); // removes last comma
            query += ")\n VALUES (";

            for (int i = 0; i < values.Length; i++)
                query += "'" + values[i] + "', ";

            query = query.Substring(0, query.Length - 2); // removes last comma
            query += ")";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
