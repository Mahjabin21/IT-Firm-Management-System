using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class EmployeeDashboard : Form
    {
        DataAccess da = new DataAccess();
        private int employeeId;
        private int userId;


        public EmployeeDashboard(int employeeId, int userId)
        {
            InitializeComponent();
            this.employeeId = employeeId;
            this.userId = userId;

            string employeeName = GetEmployeeName(employeeId);
            label2.Text = $"Welcome, {employeeName}";

            // Hide the old "View Details" button in the designer without deleting it
            button7.Visible = false;
        }
        private string GetEmployeeName(int employeeId)
        {
            string name = "";
            string sql = "SELECT FirstName FROM Employee WHERE EmployeeID=@ID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@ID", employeeId));

            if (dt != null && dt.Rows.Count > 0)
            {
                string firstName = dt.Rows[0]["FirstName"].ToString();
                
                name = firstName ; 
            }

            return name;
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private string currentTable = "";

        // Show projects immediately when button pressed
        private void button1_Click(object sender, EventArgs e)
        {
            currentTable = "Project";
            ShowProjects();
        }

        // Show salary immediately when button pressed
        private void button3_Click(object sender, EventArgs e)
        {
            currentTable = "Salary";
            ShowSalary();
        }

        // Show assets immediately when button pressed
        private void button2_Click(object sender, EventArgs e)
        {
            currentTable = "Assets";
            ShowAssets();
        }

        //add ticket (expects Tickets view or will still insert)
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("No row selected to add ticket description.");
                return;
            }

            string description = dataGridView1.CurrentRow.Cells["Description"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please enter a description before adding a ticket.");
                return;
            }

            string sql = "INSERT INTO Tickets (Description, Status, UserID) VALUES (@Desc, 'Pending', @UserID)";
            int rowsAffected = da.ExecuteInsertQuery(sql,
                new SqlParameter("@Desc", description),
                new SqlParameter("@UserID", userId));

            if (rowsAffected > 0)
            {
                ShowTickets();
            }
        }

        // Deprecated: button7 used to call ShowTable(currentTable). Remove usage or keep for compatibility.
        private void button7_Click(object sender, EventArgs e)
        {
            // Kept empty to avoid null references if designer still wires this event.
            // Use the dedicated buttons (button1, button2, button3, button8, button9) to show data.
        }

        private void button8_Click(object sender, EventArgs e)
        {
            currentTable = "Tickets";
            ShowTickets();
        }

        // Replaced ShowTable(string) with specific methods per data type:
        private void ShowProjects()
        {
            string sql = "SELECT ProjectID, ProjectName, Status, ClientID FROM Project WHERE EmployeeID=@EmployeeID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@EmployeeID", employeeId));

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("ProjectID"))
                    dataGridView1.Columns["ProjectID"].Visible = false;

                dataGridView1.Columns["ProjectName"].ReadOnly = true;
                dataGridView1.Columns["ClientID"].ReadOnly = true;
                dataGridView1.Columns["Status"].ReadOnly = false;
            }
            else
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("No projects found for this employee.");
            }
        }

        private void ShowSalary()
        {
            string sql = "SELECT SalaryID, BaseSalary, BonusAmount FROM Salary WHERE EmployeeID=@EmployeeID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@EmployeeID", employeeId));

            if (dt != null)
            {
                dt.Columns.Add("Total", typeof(decimal));
                foreach (DataRow row in dt.Rows)
                {
                    decimal baseSalary = Convert.ToDecimal(row["BaseSalary"]);
                    decimal bonus = Convert.ToDecimal(row["BonusAmount"]);
                    row["Total"] = baseSalary + bonus;
                }

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No salary records found for this employee.");
                }
                else
                {
                    if (dataGridView1.Columns.Contains("SalaryID"))
                        dataGridView1.Columns["SalaryID"].Visible = false;

                    dataGridView1.Columns["BaseSalary"].ReadOnly = true;
                    dataGridView1.Columns["BonusAmount"].ReadOnly = true;
                    dataGridView1.Columns["Total"].ReadOnly = true;
                }
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void ShowAssets()
        {
            string sql = "SELECT AssetID, AssetName, Status FROM Assets WHERE EmployeeID=@EmployeeID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@EmployeeID", employeeId));

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("AssetID"))
                    dataGridView1.Columns["AssetID"].Visible = false;

                dataGridView1.Columns["AssetName"].ReadOnly = true;
                dataGridView1.Columns["Status"].ReadOnly = true;
            }
            else
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("No assets found for this employee.");
            }
        }

        private void ShowTickets()
        {
            string sql = "SELECT TicketID, Description, Status FROM Tickets WHERE UserID=@UserID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@UserID", userId));

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("TicketID"))
                    dataGridView1.Columns["TicketID"].Visible = false;

                dataGridView1.Columns["Description"].ReadOnly = false;
                dataGridView1.Columns["Status"].ReadOnly = true;
            }
            else
            {
                // Create an empty table with the same columns so user can add a new description row
                DataTable emptyDt = new DataTable();
                emptyDt.Columns.Add("TicketID", typeof(int));
                emptyDt.Columns.Add("Description", typeof(string));
                emptyDt.Columns.Add("Status", typeof(string));

                dataGridView1.DataSource = emptyDt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("TicketID"))
                    dataGridView1.Columns["TicketID"].Visible = false;

                dataGridView1.Columns["Description"].ReadOnly = false;
                dataGridView1.Columns["Status"].ReadOnly = true;
            }
        }

        private void ShowEmployeeProfile()
        {
            string sql = "SELECT FirstName, LastName, Designation, JoiningDate FROM Employee WHERE EmployeeID=@EmployeeID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@EmployeeID", employeeId));

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.Columns["FirstName"].ReadOnly = false;
                dataGridView1.Columns["LastName"].ReadOnly = false;
                dataGridView1.Columns["Designation"].ReadOnly = false;
                dataGridView1.Columns["JoiningDate"].ReadOnly = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (row.Cells["FirstName"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["FirstName"].Value?.ToString()))
                        {
                            row.Cells["FirstName"].Value = "";
                        }

                        if (row.Cells["LastName"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["LastName"].Value?.ToString()))
                        {
                            row.Cells["LastName"].Value = "";
                        }

                        if (row.Cells["Designation"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["Designation"].Value?.ToString()))
                        {
                            row.Cells["Designation"].Value = "";
                        }
                    }
                }
            }
            else
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("No profile found for this employee.");
            }
        }

        //update
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            if (currentTable == "Project")
            {
                int projectId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProjectID"].Value);
                string status = dataGridView1.CurrentRow.Cells["Status"].Value?.ToString();

                string sql = "UPDATE Project SET Status=@Status WHERE ProjectID=@ID";
                int rowsAffected = da.ExecuteUpdateQuery(sql,
                    new SqlParameter("@Status", status),
                    new SqlParameter("@ID", projectId));

                if (rowsAffected > 0)
                {
                    ShowProjects();
                }
            }
            else if (currentTable == "Tickets")
            {
                int ticketId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["TicketID"].Value);
                string description = dataGridView1.CurrentRow.Cells["Description"].Value?.ToString();

                string sql = "UPDATE Tickets SET Description=@Desc WHERE TicketID=@ID";
                int rowsAffected = da.ExecuteUpdateQuery(sql,
                    new SqlParameter("@Desc", description),
                    new SqlParameter("@ID", ticketId));

                if (rowsAffected > 0)
                {
                    ShowTickets();
                }
            }
            else if (currentTable == "Employee")
            {
                string firstName = dataGridView1.CurrentRow.Cells["FirstName"].Value?.ToString();
                string lastName = dataGridView1.CurrentRow.Cells["LastName"].Value?.ToString();
                string designation = dataGridView1.CurrentRow.Cells["Designation"].Value?.ToString();

                string sql = "UPDATE Employee SET FirstName=@FName, LastName=@LName, Designation=@Desig WHERE EmployeeID=@ID";
                int rowsAffected = da.ExecuteUpdateQuery(sql,
                    new SqlParameter("@FName", firstName),
                    new SqlParameter("@LName", lastName),
                    new SqlParameter("@Desig", designation),
                    new SqlParameter("@ID", employeeId));

                if (rowsAffected > 0)
                {
                    ShowEmployeeProfile();
                }
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            currentTable = "Employee";
            ShowEmployeeProfile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login logout = new Login();
            logout.Show();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (button10.Image != null)
            { // Remove image if already set
                button10.Image = null;
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select a Button Image";
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        button10.Image = Image.FromFile(ofd.FileName);
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }
    }
}


