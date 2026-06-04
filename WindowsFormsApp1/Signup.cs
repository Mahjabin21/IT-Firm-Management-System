using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace WindowsFormsApp1
{
    public partial class Signup : Form
    {
        public Signup()
        {
            InitializeComponent();
        }


        //email
        private void label2_Click(object sender, EventArgs e)
        {

        }
        //confirmpassword
        private void label4_Click(object sender, EventArgs e)
        {

        }
        //password
        private void label3_Click(object sender, EventArgs e)
        {

        }
        //username
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //client panel
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //employee panel
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        //client

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }


        /*private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();

            radioButton1.Checked = false;
            radioButton2.Checked = false;
 

            panel1.Visible = false;
            panel2.Visible = false;
        }*/


        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string email = textBox2.Text.Trim();
            string password = textBox3.Text.Trim();
            string confirmPassword = textBox4.Text.Trim();

            string role = "";

            if (radioButton1.Checked)
                role = "Client";
            else if (radioButton2.Checked)
                role = "Employee";
            
            else
            {
                MessageBox.Show("Please select a role.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["ITfirm_dbb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // Insert into Users
                    string userSql = @"INSERT INTO Users (Username, Password, Role, Email)
                               VALUES (@Username, @Password, @Role, @Email);
                               SELECT SCOPE_IDENTITY();";

                    SqlCommand userCmd = new SqlCommand(userSql, conn, tran);
                    userCmd.Parameters.AddWithValue("@Username", username);
                    userCmd.Parameters.AddWithValue("@Password", password);
                    userCmd.Parameters.AddWithValue("@Role", role);
                    userCmd.Parameters.AddWithValue("@Email", email);

                    int userId = Convert.ToInt32(userCmd.ExecuteScalar());

                    // Insert minimal record into Employees or Clients
                    if (role == "Employee")
                    {
                        string empSql = @"INSERT INTO Employee (UserID) VALUES (@UserID)";
                        SqlCommand empCmd = new SqlCommand(empSql, conn, tran);
                        empCmd.Parameters.AddWithValue("@UserID", userId);
                        empCmd.ExecuteNonQuery();
                    }
                    else if (role == "Client")
                    {
                        string clientSql = @"INSERT INTO Clients (UserID, Email) VALUES (@UserID, @Email)";
                        SqlCommand clientCmd = new SqlCommand(clientSql, conn, tran);
                        clientCmd.Parameters.AddWithValue("@UserID", userId);
                        clientCmd.Parameters.AddWithValue("@Email", email);
                        clientCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    //MessageBox.Show("Signup completed successfully!");

                    // Redirect to login
                    Login login = new Login();
                    login.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
        }
        private void label6_Click(object sender, EventArgs e)
        {
        }
        private void label7_Click(object sender, EventArgs e)
        {
        }
        private void label9_Click(object sender, EventArgs e)
        {
        }
        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Homepage hp = new Homepage();
            hp.Show();
            this.Hide();
        }
    }
}

