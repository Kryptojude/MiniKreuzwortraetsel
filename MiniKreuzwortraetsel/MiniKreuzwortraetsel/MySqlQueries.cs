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
        public static void CREATE_TABLE(string newTableName)
        {
            conn.Open();
            string query = " CREATE TABLE " + newTableName + "(" + "\n" +
                           " ID          INT NOT NULL AUTO_INCREMENT,                   \n" +
                           " Answer      VARCHAR(255) NOT NULL,                         \n" +
                           " Question    VARCHAR(255) NOT NULL,                         \n" +
                           " PRIMARY KEY (ID) );                                        \n";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void DROP_TABLE()
        {
            throw new NotImplementedException();
        }
    }
}
