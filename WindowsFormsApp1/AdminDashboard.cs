using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class AdminDashboard : Form
    {
        DataAccess da = new DataAccess();
        public AdminDashboard()
        {
            InitializeComponent();

        }
        //add button
        private void button1_Click(object sender, EventArgs e)
        {
            if (currentTable == "Salary")
            {
                SalaryManagement form = new SalaryManagement();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ShowSalary(); // refresh grid after adding
                }
            }

            else if (currentTable == "Invoices")
            {
                InvoiceManagement form = new InvoiceManagement();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ShowInvoices(); // refresh grid after adding
                }

            }
            else if (currentTable == "Users")
            {
                Signup form = new Signup();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ShowUsers(); // refresh grid after adding
                }
            }


            else
            {
                // MessageBox.Show("Add operation is only supported for Salary, Tickets, and Users tables.");
            }
        }
        //delete
        private void button2_Click(object sender, EventArgs e) // Delete button
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the ID from the first cell of the selected row
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);

                string sql = "";

                if (currentTable == "Employee")
                    sql = "DELETE FROM Employee WHERE EmployeeID = @ID";
                else if (currentTable == "Clients")
                    sql = "DELETE FROM Clients WHERE ClientID = @ID";
                else if (currentTable == "Project")
                    sql = "DELETE FROM Project WHERE ProjectID = @ID";
                else if (currentTable == "Assets")
                    sql = "DELETE FROM Assets WHERE AssetID = @ID";
                else if (currentTable == "Invoices")
                    sql = "DELETE FROM Invoices WHERE InvoiceID = @ID";
                else if (currentTable == "Tickets")
                {
                    int ticketId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["TicketID"].Value);
                    string status = dataGridView1.SelectedRows[0].Cells["Status"].Value?.ToString();

                    if (status == "Solved")
                    {
                        sql = "DELETE FROM Tickets WHERE TicketID=@ID";
                        int rows = da.ExecuteUpdateQuery(sql, new SqlParameter("@ID", ticketId));

                        if (rows > 0)
                        {
                            // MessageBox.Show("Solved ticket deleted successfully!");
                            ShowTickets();
                        }
                        else
                        {
                            MessageBox.Show("Delete failed. Ticket may not exist.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Only tickets with status 'Solved' can be deleted.");
                    }
                }



                else if (currentTable == "Salary")
                    sql = "DELETE FROM Salary WHERE SalaryID = @ID";
                else if (currentTable == "Users")
                    sql = "DELETE FROM Users WHERE UserID = @ID";

                if (!string.IsNullOrEmpty(sql))
                {
                    int rows = da.ExecuteUpdateQuery(sql, new SqlParameter("@ID", id));

                    if (rows > 0)
                    {
                        //MessageBox.Show($"{currentTable} record deleted successfully!");
                        RefreshCurrentView(); // refresh grid
                    }
                    else
                    {
                        MessageBox.Show("Delete failed. Record may not exist.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Login logout = new Login();
            logout.Show();
            this.Hide();
        }
        //update button
        private void button11_Click(object sender, EventArgs e) // Update button
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                int rows = 0;

                // Salary update logic
                if (currentTable == "Salary")
                {
                    int id = Convert.ToInt32(row.Cells["SalaryID"].Value);
                    decimal baseSalary = Convert.ToDecimal(row.Cells["BaseSalary"].Value);
                    decimal bonusAmount = Convert.ToDecimal(row.Cells["BonusAmount"].Value);
                    int employeeId = Convert.ToInt32(row.Cells["EmployeeID"].Value);

                    string sql = "UPDATE Salary SET BaseSalary=@BaseSalary, BonusAmount=@BonusAmount, EmployeeID=@EmployeeID WHERE SalaryID=@ID";
                    rows = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@BaseSalary", baseSalary),
                        new SqlParameter("@BonusAmount", bonusAmount),
                        new SqlParameter("@EmployeeID", employeeId),
                        new SqlParameter("@ID", id));
                }

                // Ticket update logic
                else if (currentTable == "Tickets")
                {
                    int ticketId = Convert.ToInt32(row.Cells["TicketID"].Value);
                    string status = row.Cells["Status"].Value?.ToString(); // Update status only

                    string sql = "UPDATE Tickets SET Status=@Status WHERE TicketID=@ID";
                    rows = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@Status", status),
                        new SqlParameter("@ID", ticketId)); // Optional feedback for admin
                    if (status == "Solved")
                    {
                        //MessageBox.Show("Ticket marked as Solved. It will no longer appear in Admin view."); 
                    }
                }

                else if (currentTable == "Clients")
                {
                    int clientId = Convert.ToInt32(row.Cells["ClientID"].Value);
                    string companyName = row.Cells["CompanyName"].Value.ToString();
                    string contactNumber = row.Cells["ContactNumber"].Value.ToString();
                    string email = row.Cells["Email"].Value.ToString();
                    string sql = "UPDATE Clients SET CompanyName=@CompanyName, ContactNumber=@ContactNumber, Email=@Email WHERE ClientID=@ID";
                    rows = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@CompanyName", companyName),
                        new SqlParameter("@ContactNumber", contactNumber),
                        new SqlParameter("@Email", email),
                        new SqlParameter("@ID", clientId));
                }
                else if (currentTable == "Project")
                {
                    int projectId = Convert.ToInt32(row.Cells["ProjectID"].Value);
                    int employeeId = (row.Cells["EmployeeID"].Value == null || row.Cells["EmployeeID"].Value == DBNull.Value
                                      || string.IsNullOrWhiteSpace(row.Cells["EmployeeID"].Value.ToString()))
                        ? 0 : Convert.ToInt32(row.Cells["EmployeeID"].Value);

                    if (employeeId > 0) // only update if admin assigned an employee
                    {
                        string sql = "UPDATE Project SET EmployeeID=@EmployeeID WHERE ProjectID=@ID";
                        rows = da.ExecuteUpdateQuery(sql,
                            new SqlParameter("@EmployeeID", employeeId),
                            new SqlParameter("@ID", projectId));
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid EmployeeID to assign.");
                    }
                }


                else if (currentTable == "Assets")
                {
                    int assetId = (row.Cells["AssetID"].Value == null || row.Cells["AssetID"].Value == DBNull.Value)
                        ? 0 : Convert.ToInt32(row.Cells["AssetID"].Value);

                    string assetName = row.Cells["AssetName"].Value?.ToString();
                    string status = row.Cells["Status"].Value?.ToString();
                    int employeeId = (row.Cells["EmployeeID"].Value == null || row.Cells["EmployeeID"].Value == DBNull.Value
                                      || string.IsNullOrWhiteSpace(row.Cells["EmployeeID"].Value.ToString()))
                        ? 0 : Convert.ToInt32(row.Cells["EmployeeID"].Value);

                    if (assetId == 0) // INSERT new asset
                    {
                        string sql = "INSERT INTO Assets (AssetName, Status, EmployeeID) VALUES (@AssetName, @Status, @EmployeeID)";
                        rows = da.ExecuteInsertQuery(sql,
                            new SqlParameter("@AssetName", assetName),
                            new SqlParameter("@Status", status),
                            new SqlParameter("@EmployeeID", employeeId));
                    }
                    else // UPDATE existing asset
                    {
                        string sql = "UPDATE Assets SET AssetName=@AssetName, Status=@Status, EmployeeID=@EmployeeID WHERE AssetID=@ID";
                        rows = da.ExecuteUpdateQuery(sql,
                            new SqlParameter("@AssetName", assetName),
                            new SqlParameter("@Status", status),
                            new SqlParameter("@EmployeeID", employeeId),
                            new SqlParameter("@ID", assetId));
                    }
                }

                else if (currentTable == "Employee")
                {
                    int employeeId = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                    string joiningDate = row.Cells["JoiningDate"].Value?.ToString();

                    // Only update JoiningDate
                    string sql = "UPDATE Employee SET JoiningDate=@JoinDate WHERE EmployeeID=@ID";
                    rows = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@JoinDate", joiningDate),
                        new SqlParameter("@ID", employeeId));

                    if (rows > 0)
                    {
                        //MessageBox.Show("Employee joining date updated successfully!");
                        ShowEmployee();
                    }
                    else
                    {
                        MessageBox.Show("Update failed.");
                    }

                }

                // Result handling
                if (rows > 0)
                {
                    //MessageBox.Show($"{currentTable} record updated successfully!");
                    RefreshCurrentView(); // refresh grid from DB
                }
                else
                {
                    MessageBox.Show("Update failed.");
                }
            }

            else if (currentTable == "Invoices")
            {
                var row = dataGridView1.SelectedRows[0];

                int invoiceId = Convert.ToInt32(row.Cells["InvoiceID"].Value);

                int amount = 0;
                int.TryParse(row.Cells["Amount"].Value?.ToString(), out amount);

                string paymentStatus = row.Cells["PaymentStatus"].Value?.ToString();

                DateTime issueDate = DateTime.TryParse(row.Cells["IssueDate"].Value?.ToString(), out var tmpIssue) ? tmpIssue : DateTime.Now;
                DateTime dueDate = DateTime.TryParse(row.Cells["DueDate"].Value?.ToString(), out var tmpDue) ? tmpDue : DateTime.Now;

                int projectId = 0;
                int.TryParse(row.Cells["ProjectID"].Value?.ToString(), out projectId);

                int clientId = 0;
                int.TryParse(row.Cells["ClientID"].Value?.ToString(), out clientId);

                int partialPayment = 0;
                int.TryParse(row.Cells["PartialPaymentAmount"].Value?.ToString(), out partialPayment);

                string sql = @"UPDATE Invoices 
                   SET Amount=@Amount, PaymentStatus=@PaymentStatus, IssueDate=@IssueDate, 
                       DueDate=@DueDate, ProjectID=@ProjectID, ClientID=@ClientID, 
                       PartialPaymentAmount=@PartialPayment 
                   WHERE InvoiceID=@ID";

                int rows = da.ExecuteUpdateQuery(sql,
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@PaymentStatus", paymentStatus),
                    new SqlParameter("@IssueDate", issueDate),
                    new SqlParameter("@DueDate", dueDate),
                    new SqlParameter("@ProjectID", projectId),
                    new SqlParameter("@ClientID", clientId),
                    new SqlParameter("@PartialPayment", partialPayment),
                    new SqlParameter("@ID", invoiceId));
            }

        }



        private string currentTable = "Employee"; // default view

        // Sidebar buttons set the current table and immediately show the view
        private void button3_Click(object sender, EventArgs e) // Employee
        {
            currentTable = "Employee";
            ShowEmployee();
        }

        private void button4_Click(object sender, EventArgs e) // Client
        {
            currentTable = "Clients";
            ShowClients();
        }

        private void button5_Click(object sender, EventArgs e) // Project
        {
            currentTable = "Project";
            ShowProjects();
        }

        private void button6_Click(object sender, EventArgs e) // Assets
        {
            currentTable = "Assets";
            ShowAssets();
        }

        private void button7_Click(object sender, EventArgs e) // Invoices
        {
            currentTable = "Invoices";
            ShowInvoices();
        }

        private void button8_Click(object sender, EventArgs e) // Tickets
        {
            currentTable = "Tickets";
            ShowTickets();
        }

        private void button9_Click(object sender, EventArgs e) // Salary
        {
            currentTable = "Salary";
            ShowSalary();
        }

        private void button13_Click(object sender, EventArgs e) // Users
        {
            currentTable = "Users";
            ShowUsers();
        }

        // Optional: keep View Details button in sync (no-op or refresh)
        private void button12_Click(object sender, EventArgs e)
        {
            // If you want the old View Details button to still work:
            RefreshCurrentView();
        }

        // Reusable per-view show methods (replaced generic ShowTable)
        private void ShowProjects()
        {
            string sql = "SELECT ProjectID, ProjectName, ClientID, EmployeeID, Status FROM Project";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Hide PK
                if (dataGridView1.Columns.Contains("ProjectID"))
                    dataGridView1.Columns["ProjectID"].Visible = true;

                // ProjectName + ClientID are read-only (set by client)
                dataGridView1.Columns["ProjectName"].ReadOnly = true;
                dataGridView1.Columns["ClientID"].ReadOnly = true;

                // EmployeeID editable (admin assigns employee)
                dataGridView1.Columns["EmployeeID"].ReadOnly = false;

                // Status read-only (employee updates later)
                dataGridView1.Columns["Status"].ReadOnly = true;
            }
            else
            {
                MessageBox.Show("No projects found.");
            }
        }

        private void ShowTickets()
        {
            string sql = "SELECT TicketID, Description, Status, UserID FROM Tickets WHERE Status <> 'Solved'";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("TicketID"))
                    dataGridView1.Columns["TicketID"].Visible = false;

                dataGridView1.Columns["Description"].ReadOnly = true;
                dataGridView1.Columns["Status"].ReadOnly = false;
                dataGridView1.Columns["UserID"].ReadOnly = true;
            }
            else
            {
                MessageBox.Show("No active tickets found.");
            }
        }

        private void ShowAssets()
        {
            string sql = "SELECT AssetID, AssetName, Status, EmployeeID FROM Assets";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Allow adding new rows
                dataGridView1.AllowUserToAddRows = true;

                // Hide PK
                if (dataGridView1.Columns.Contains("AssetID"))
                    dataGridView1.Columns["AssetID"].Visible = false;

                // Admin can edit these
                dataGridView1.Columns["AssetName"].ReadOnly = false;
                dataGridView1.Columns["Status"].ReadOnly = false;
                dataGridView1.Columns["EmployeeID"].ReadOnly = false;
            }
            else
            {
                MessageBox.Show("No assets found.");
            }
        }

        private void ShowEmployee()
        {
            string sql = "SELECT EmployeeID, FirstName, LastName, Designation, JoiningDate FROM Employee";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Hide PK
                if (dataGridView1.Columns.Contains("EmployeeID"))
                    dataGridView1.Columns["EmployeeID"].Visible = true;

                // Lock all fields except JoiningDate
                dataGridView1.Columns["FirstName"].ReadOnly = true;
                dataGridView1.Columns["LastName"].ReadOnly = true;
                dataGridView1.Columns["Designation"].ReadOnly = true;

                // Admin can edit JoiningDate
                dataGridView1.Columns["JoiningDate"].ReadOnly = false;
            }
            else
            {
                MessageBox.Show("No employees found.");
            }
        }

        private void ShowInvoices()
        {
            string sql = "SELECT InvoiceID, Amount, PaymentStatus, IssueDate, DueDate, ProjectID, ClientID, PartialPaymentAmount FROM Invoices";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.Columns["InvoiceID"].ReadOnly = true;
                dataGridView1.Columns["ProjectID"].ReadOnly = true;
                dataGridView1.Columns["ClientID"].ReadOnly = true;
            }
            else
            {
                MessageBox.Show("No invoices found.");
            }
        }

        private void ShowSalary()
        {
            string sql = "SELECT SalaryID, BaseSalary, BonusAmount, EmployeeID FROM Salary";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("SalaryID"))
                    dataGridView1.Columns["SalaryID"].Visible = false;

                dataGridView1.Columns["BaseSalary"].ReadOnly = false;
                dataGridView1.Columns["BonusAmount"].ReadOnly = false;
                dataGridView1.Columns["EmployeeID"].ReadOnly = false;
            }
            else
            {
                MessageBox.Show("No salary records found.");
            }
        }

        private void ShowUsers()
        {
            string sql = "SELECT UserID, Username, Role, Email FROM Users";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("UserID"))
                    dataGridView1.Columns["UserID"].Visible = false;

                dataGridView1.Columns["Username"].ReadOnly = false;
                dataGridView1.Columns["Role"].ReadOnly = false;
                dataGridView1.Columns["Email"].ReadOnly = false;
            }
            else
            {
                MessageBox.Show("No users found.");
            }
        }

        private void ShowClients()
        {
            string sql = "SELECT ClientID, CompanyName, ContactNumber, Email FROM Clients";
            DataTable dt = da.ExecuteQueryTable(sql);

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("ClientID"))
                    dataGridView1.Columns["ClientID"].Visible = false;

                dataGridView1.Columns["CompanyName"].ReadOnly = false;
                dataGridView1.Columns["ContactNumber"].ReadOnly = false;
                dataGridView1.Columns["Email"].ReadOnly = false;
            }
            else
            {
                MessageBox.Show("No clients found.");
            }
        }

        // refresh helper
        private void RefreshCurrentView()
        {
            if (currentTable == "Project") ShowProjects();
            else if (currentTable == "Tickets") ShowTickets();
            else if (currentTable == "Assets") ShowAssets();
            else if (currentTable == "Employee") ShowEmployee();
            else if (currentTable == "Invoices") ShowInvoices();
            else if (currentTable == "Salary") ShowSalary();
            else if (currentTable == "Users") ShowUsers();
            else if (currentTable == "Clients") ShowClients();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

}