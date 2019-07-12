using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace cons_test_maker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        mysql_db db = new mysql_db();


        public class mysql_db
        {
            MySqlConnection con;
            private string ConnectionString;


            public mysql_db()
            {
                ConnectionString = "server=127.0.0.1;user=root;database=db_tntnets;password=++++++;charset=utf8; Allow Zero Datetime=true;";
                Connect();
            }

            public void Connect()
            {
                try
                {
                    con = new MySqlConnection();

                    //ConnectionString = "server=\"" + ConfigurationManager.AppSettings["mysql_host"] + "\";user=" +
                    //                    ConfigurationManager.AppSettings["mysql_login"] + ";database=" + ConfigurationManager.AppSettings["mysql_db"] + ";password=" + ConfigurationManager.AppSettings["mysql_pass"] + ";charset=utf8; Allow Zero Datetime=true;";

                    con.ConnectionString = ConnectionString;
                    MySqlConnectionStringBuilder mysqlCSB = new MySqlConnectionStringBuilder();
                    mysqlCSB.ConnectionString = ConnectionString;
                    con.Open();
                }
                catch (Exception exception)
                {
                    write_log("Не удалось подключиться к mysql серверу");
                }
            }

            void write_log(string s)
            {

            }

            public void SqlQuery(string sql, string message)
            {
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, con);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        //MessageBox.Show(message);
                    }

                    catch (Exception ex)
                    {
                        write_log(ex.Message);
                    }
                }
            }

            public string SqlQueryWithResult(string sql)
            {
                DataTable dt = Get_DataTable(sql);
                if (dt == null)
                    return "0";
                else
                {
                    if (dt.Rows.Count != 0)
                        return dt.Rows[0][0].ToString();
                    else
                        return "0";
                }
            }

            public DataTable Get_DataTable(string queryString)
            {
                DataTable dt = new DataTable();
                MySqlCommand com = new MySqlCommand(queryString, con);

                try
                {
                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return dt;
            }

            public DataTable GetPhones(string id, string num, string base_name)
            {
                string q = @"SELECT phone from " + base_name + " WHERE state='0' LIMIT 0, " + num.ToString();
                return Get_DataTable(q);
            }


            public string[] GetPhones(int num, string base_name)
            {
                DataTable dt = GetPhones("", num.ToString(), base_name);

                string[] readText = new string[dt.Rows.Count];

                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string phone = Convert.ToString(row["phone"]);
                    readText[i] = phone;
                    //MessageBox.Show(phone);
                    SqlQuery("UPDATE " + base_name + " SET state=1 WHERE phone='" + phone + "'", "");
                    i++;
                }

                return readText;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click_1(null, null);
            Random rnd = new Random();
            int value = rnd.Next(100000, 999999);
            textBox2.Text = value.ToString();

            DateTime dt1 = DateTime.Now;
            DateTime dt2 = DateTime.Now.AddDays(7);

            textBox9.Text = dt1.ToString("dd-MM-yyyy");
            textBox10.Text = dt2.ToString("dd-MM-yyyy");
        }
        
        private void MakeBat(string login, string pass,bool pause)
        {
            textBox_batnik.Text += "net user " + login + " " + pass + " /add\r\n";
            textBox_batnik.Text += "net localgroup \"Пользователи удаленного рабочего стола\" " + login + " /add\r\n";

            if (pause==true)
             textBox_batnik.Text += "pause\r\n";

            File.WriteAllText("makeuser.bat", textBox_batnik.Text, Encoding.GetEncoding(866));
        }
               

        private void button1_Click_1(object sender, EventArgs e)
        {
            db.Connect();
            dataGridView1.DataSource = db.Get_DataTable("SELECT * from tab_clients");

            try
            {
                string s = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[2].Value.ToString();
                string t = s.Substring(s.Length - 2, 2);
                textBox1.Text = "test05" + (Convert.ToInt32(t) + 1).ToString();
                //MessageBox.Show(t);
            }
            catch (Exception ex)
            {

            }
            Random rnd = new Random();
            int value = rnd.Next(100000, 999999);
            textBox2.Text = value.ToString();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string sql = "INSERT INTO `tab_clients` VALUES (NULL, NOW(),'" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" +
            textBox5.Text + "','" + textBox6.Text + "','" + textBox7.Text + "','" + textBox8.Text + "','qwe','" + textBox9.Text + "','" + textBox10.Text + "','0','tnt','tnt','500');";
            textBox_sql.Text = sql;

            MessageBox.Show(sql);
            db.SqlQuery(sql, "");

            MakeBat(textBox1.Text, textBox2.Text, true);
            System.Diagnostics.Process.Start("makeuser.bat");
            button1_Click_1(null, null);
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                string logn = dataGridView1.Rows[i].Cells[2].Value.ToString();
                string pswd = dataGridView1.Rows[i].Cells[3].Value.ToString();

                if (logn.IndexOf(",") > 0)
                {
                    string[] strArr = logn.Split(',');
                    for (int j = 0; j < strArr.Length; j++)
                    {
                        //MessageBox.Show(strArr[j]);
                        MakeBat(strArr[j], pswd, false);
                    }
                }
                else
                {
                    MakeBat(logn, pswd, false);
                    //MessageBox.Show(logn + ":" + pswd);
                }                

            }
        }
    }
}
