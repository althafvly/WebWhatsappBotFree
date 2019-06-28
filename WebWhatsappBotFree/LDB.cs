using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace WebWhatsappBotFree
{
    class LDB
    {

        public static SQLiteConnection baglanti=new SQLiteConnection("Data Source=wwbf.sqlite;Version=3;");
        
        public static bool sqlcalistir(String sorgu)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = sorgu;
                command.Connection = baglanti;
                baglanti.Open();
                command.ExecuteNonQuery();
                baglanti.Close();
                return true;
            }
            catch { kapat(); return false; }
        }

        public static DataTable goruntule(String sorgu)
        {
            DataTable tablo = new DataTable();
            try
            {
                baglanti.Open();
                SQLiteCommand mycommand = new SQLiteCommand(baglanti);
                mycommand.CommandText = sorgu;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                tablo.Load(reader);
                reader.Close();
                baglanti.Close();
                return tablo;
            }
            catch { kapat(); return null; }
        }
        public static String sorgula(string sorgu)
        {
            try
            {
                String sql = String.Empty;
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = sorgu;
                command.Connection = baglanti;
                baglanti.Open();
                sql = (string)command.ExecuteScalar().ToString();
                baglanti.Close();
                return sql;
            }
            catch { kapat(); return null; }
        }
        public static void kapat()
        {
            try
            {

            baglanti.Close();
            }
            catch { return; }
        }
    }
}