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



namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim(); // Username textbox
            string password = textBox2.Text.Trim(); // Password textbox

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["ITfirm_dbb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // ✅ Only select Role + UserID from Users
                string sql = "SELECT Role, UserID FROM Users WHERE Username=@u AND Password=@p";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string role = dt.Rows[0]["Role"].ToString();
                    int userId = Convert.ToInt32(dt.Rows[0]["UserID"]);

                    if (role == "Employee")
                    {
                        // Lookup EmployeeID from Employees table
                        string empSql = "SELECT EmployeeID FROM Employee WHERE UserID=@UserID";
                        SqlCommand empCmd = new SqlCommand(empSql, conn);
                        empCmd.Parameters.AddWithValue("@UserID", userId);
                        object empResult = empCmd.ExecuteScalar();
                        int employeeId = empResult == null ? 0 : Convert.ToInt32(empResult);

                        EmployeeDashboard ed = new EmployeeDashboard(employeeId, userId);
                        ed.Show();
                        this.Hide();
                    }
                    else if (role == "Client")
                    {
                        // Lookup ClientID from Clients table
                        string clientSql = "SELECT ClientID FROM Clients WHERE UserID=@UserID";
                        SqlCommand clientCmd = new SqlCommand(clientSql, conn);
                        clientCmd.Parameters.AddWithValue("@UserID", userId);
                        object clientResult = clientCmd.ExecuteScalar();
                        int clientId = clientResult == null ? 0 : Convert.ToInt32(clientResult);

                        ClientDashboard cd = new ClientDashboard(clientId, userId);
                        cd.Show();
                        this.Hide();
                    }
                    else if (role == "Admin")
                    {
                        AdminDashboard ad = new AdminDashboard();
                        ad.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Wrong Username or Password!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Homepage hp = new Homepage();       
            hp.Show();
            this.Hide();
        }
    }
}
