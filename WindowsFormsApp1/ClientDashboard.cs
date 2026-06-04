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
    public partial class ClientDashboard : Form
    {
        DataAccess da = new DataAccess();
        private int clientId;
        private int userId;

        public ClientDashboard(int clientId, int userId)
        {
            InitializeComponent();
            this.clientId = clientId;
            this.userId = userId;

            // Show company name like EmployeeDashboard does
            string companyName = GetClientName(clientId);
            label2.Text = $"Welcome, {companyName}";

            // Remove manual event subscription: Designer already wires this.
            // dataGridView1.CellContentClick += dataGridView1_CellContentClick;

            // hide old external view-details button if still present
            if (this.Controls.ContainsKey("button8"))
                button8.Visible = false;
        }

        private string GetClientName(int clientId)
        {
            if (clientId <= 0) return string.Empty;

            string name = string.Empty;
            string sql = "SELECT CompanyName FROM Clients WHERE ClientID=@ID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@ID", clientId));

            if (dt != null && dt.Rows.Count > 0)
            {
                var value = dt.Rows[0]["CompanyName"];
                if (value != DBNull.Value && value != null)
                {
                    name = value.ToString().Trim();
                }
            }

            return name;
        }

        private string currentTable = "Project";

        // Now each button shows its view immediately (no separate "View Details" button)
        private void button1_Click(object sender, EventArgs e)
        {
            currentTable = "Project";
            ShowProjects();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            currentTable = "Clients";
            ShowClientProfile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentTable = "Invoices";
            ShowInvoices();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentTable = "Tickets";
            ShowTickets();
        }

        // keep handler present if designer wired it; it's no longer needed to trigger ShowTable
        private void button8_Click_1(object sender, EventArgs e)
        {
            // intentionally left empty — navigation buttons show data immediately now
        }

        // --- Grid helper: add/remove an "Add" button column (one per grid) ---
        private const string AddButtonColumnName = "btnAdd";

        private void AddGridAddButton(string text = "Add")
        {
            // avoid duplicates
            if (dataGridView1.Columns.Contains(AddButtonColumnName)) return;

            var btnCol = new DataGridViewButtonColumn
            {
                Name = AddButtonColumnName,
                HeaderText = "",
                Text = text,
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Standard,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
            dataGridView1.Columns.Add(btnCol);
        }

        private void RemoveGridAddButtonIfExists()
        {
            if (dataGridView1.Columns.Contains(AddButtonColumnName))
                dataGridView1.Columns.Remove(AddButtonColumnName);
        }

        // Per-view show methods (replaced generic ShowTable)
        private void ShowProjects()
        {
            string sql = "SELECT ProjectID, ProjectName, EmployeeID, Status FROM Project WHERE ClientID=@ClientID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@ClientID", clientId));

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = true;

                if (dataGridView1.Columns.Contains("ProjectID"))
                    dataGridView1.Columns["ProjectID"].Visible = false;

                if (dataGridView1.Columns.Contains("ProjectName"))
                    dataGridView1.Columns["ProjectName"].ReadOnly = false;

                if (dataGridView1.Columns.Contains("EmployeeID"))
                    dataGridView1.Columns["EmployeeID"].ReadOnly = true;

                if (dataGridView1.Columns.Contains("Status"))
                    dataGridView1.Columns["Status"].ReadOnly = true;

                // show Add button inside grid so user can insert the current row
                RemoveGridAddButtonIfExists();
                AddGridAddButton("Add");
            }
            else
            {
                dataGridView1.DataSource = null;
                RemoveGridAddButtonIfExists();
            }
        }

        private void ShowInvoices()
        {
            string sql = @"SELECT InvoiceID, Amount, PaymentStatus, IssueDate, DueDate,
                   PartialPaymentAmount, Comments 
            FROM Invoices WHERE ClientID=@ClientID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@ClientID", clientId));

            if (dt != null)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = true;

                if (dataGridView1.Columns.Contains("InvoiceID"))
                    dataGridView1.Columns["InvoiceID"].Visible = false;

                if (dataGridView1.Columns.Contains("Amount"))
                    dataGridView1.Columns["Amount"].ReadOnly = true;
                if (dataGridView1.Columns.Contains("PaymentStatus"))
                    dataGridView1.Columns["PaymentStatus"].ReadOnly = true;
                if (dataGridView1.Columns.Contains("IssueDate"))
                    dataGridView1.Columns["IssueDate"].ReadOnly = true;
                if (dataGridView1.Columns.Contains("DueDate"))
                    dataGridView1.Columns["DueDate"].ReadOnly = true;

                if (dataGridView1.Columns.Contains("PartialPaymentAmount"))
                    dataGridView1.Columns["PartialPaymentAmount"].ReadOnly = false;
                if (dataGridView1.Columns.Contains("Comments"))
                    dataGridView1.Columns["Comments"].ReadOnly = false;

                // IMPORTANT: no in-grid Add button for Invoices per your request
                RemoveGridAddButtonIfExists();
            }
            else
            {
                dataGridView1.DataSource = null;
                RemoveGridAddButtonIfExists();
            }
        }

        private void ShowTickets()
        {
            string sql = "SELECT TicketID, Description, Status FROM Tickets WHERE UserID=@USERID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@USERID", userId));

            // Clear existing binding/columns so Add button won't accumulate
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = true;

                if (dataGridView1.Columns.Contains("TicketID"))
                    dataGridView1.Columns["TicketID"].Visible = false;

                if (dataGridView1.Columns.Contains("Description"))
                    dataGridView1.Columns["Description"].ReadOnly = false;
                if (dataGridView1.Columns.Contains("Status"))
                    dataGridView1.Columns["Status"].ReadOnly = true;

                // Normalize empty description cells
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow && dataGridView1.Columns.Contains("Description"))
                    {
                        if (row.Cells["Description"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["Description"].Value?.ToString()))
                        {
                            row.Cells["Description"].Value = "";
                        }
                    }
                }

                // Add single Add button column
                RemoveGridAddButtonIfExists();
                AddGridAddButton("Add Ticket");
            }
            else
            {
                // Empty table so user can add a ticket
                DataTable emptyDt = new DataTable();
                emptyDt.Columns.Add("TicketID", typeof(int));
                emptyDt.Columns.Add("Description", typeof(string));
                emptyDt.Columns.Add("Status", typeof(string));

                dataGridView1.DataSource = emptyDt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = true;

                if (dataGridView1.Columns.Contains("TicketID"))
                    dataGridView1.Columns["TicketID"].Visible = false;

                if (dataGridView1.Columns.Contains("Description"))
                    dataGridView1.Columns["Description"].ReadOnly = false;
                if (dataGridView1.Columns.Contains("Status"))
                    dataGridView1.Columns["Status"].ReadOnly = true;

                RemoveGridAddButtonIfExists();
                AddGridAddButton("Add Ticket");
            }
        }

        private void ShowClientProfile()
        {
            string sql = "SELECT ClientID, CompanyName, ContactNumber, Email FROM Clients WHERE ClientID=@ClientID";
            DataTable dt = da.ExecuteQueryTable(sql, new SqlParameter("@ClientID", clientId));

            if (dt != null && dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns.Contains("ClientID"))
                    dataGridView1.Columns["ClientID"].Visible = false;

                if (dataGridView1.Columns.Contains("CompanyName"))
                    dataGridView1.Columns["CompanyName"].ReadOnly = false;
                if (dataGridView1.Columns.Contains("ContactNumber"))
                    dataGridView1.Columns["ContactNumber"].ReadOnly = false;
                if (dataGridView1.Columns.Contains("Email"))
                    dataGridView1.Columns["Email"].ReadOnly = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (dataGridView1.Columns.Contains("CompanyName") &&
                            (row.Cells["CompanyName"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["CompanyName"].Value?.ToString())))
                        {
                            row.Cells["CompanyName"].Value = "";
                        }

                        if (dataGridView1.Columns.Contains("ContactNumber") &&
                            (row.Cells["ContactNumber"].Value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(row.Cells["ContactNumber"].Value?.ToString())))
                        {
                            row.Cells["ContactNumber"].Value = "";
                        }
                    }
                }

                // show Add button in profile view — treat it as "Save" for convenience
                RemoveGridAddButtonIfExists();
                AddGridAddButton("Save Profile");
            }
            else
            {
                dataGridView1.DataSource = null;
                RemoveGridAddButtonIfExists();
            }
        }

        // existing Insert/Update handler retained — operates on the currently shown view
        private void button7_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var row = dataGridView1.CurrentRow;
            int rowsAffected = 0;

            if (currentTable == "Project")
            {
                int projectId = (row.Cells["ProjectID"].Value == null || row.Cells["ProjectID"].Value == DBNull.Value)
                    ? 0 : Convert.ToInt32(row.Cells["ProjectID"].Value);

                string projectName = row.Cells["ProjectName"].Value == null ? "" : row.Cells["ProjectName"].Value.ToString();

                if (projectId == 0) // INSERT new project by client
                {
                    string sql = @"INSERT INTO Project (ProjectName, ClientID, EmployeeID, Status) 
                           VALUES (@ProjectName, @ClientID, NULL, 'Pending')";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@ProjectName", projectName),
                        new SqlParameter("@ClientID", clientId));
                }
                else
                {
                    MessageBox.Show("Project name cannot be changed after creation. Please contact Admin.");
                }
            }
            else if (currentTable == "Invoices")
            {
                int invoiceId = (row.Cells["InvoiceID"].Value == null || row.Cells["InvoiceID"].Value == DBNull.Value)
                    ? 0 : Convert.ToInt32(row.Cells["InvoiceID"].Value);

                string comments = row.Cells["Comments"].Value == null ? "" : row.Cells["Comments"].Value.ToString();
                decimal partialPayment = (row.Cells["PartialPaymentAmount"].Value == null || row.Cells["PartialPaymentAmount"].Value == DBNull.Value
                                          || string.IsNullOrWhiteSpace(row.Cells["PartialPaymentAmount"].Value.ToString()))
                    ? 0 : Convert.ToDecimal(row.Cells["PartialPaymentAmount"].Value);

                if (invoiceId == 0) // INSERT new invoice
                {
                    string sql = @"INSERT INTO Invoices (ClientID, ProjectID, Amount, PaymentStatus, IssueDate, DueDate, PartialPaymentAmount, Comments)
                       VALUES (@ClientID, NULL, 0, 'Unpaid', GETDATE(), DATEADD(day,30,GETDATE()), @Partial, @Comments)";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@ClientID", clientId),
                        new SqlParameter("@Partial", partialPayment),
                        new SqlParameter("@Comments", comments));
                }
                else // UPDATE existing invoice
                {
                    string sql = "UPDATE Invoices SET Comments=@Comments, PartialPaymentAmount=@Partial WHERE InvoiceID=@ID";
                    rowsAffected = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@Comments", comments),
                        new SqlParameter("@Partial", partialPayment),
                        new SqlParameter("@ID", invoiceId));
                }
            }

            else if (currentTable == "Tickets")
            {
                int ticketId = (row.Cells["TicketID"].Value == null || row.Cells["TicketID"].Value == DBNull.Value)
                    ? 0 : Convert.ToInt32(row.Cells["TicketID"].Value);

                string description = row.Cells["Description"].Value == null ? "" : row.Cells["Description"].Value.ToString();
                string status = row.Cells["Status"].Value == null ? "" : row.Cells["Status"].Value.ToString();

                if (ticketId == 0) // INSERT new ticket
                {
                    string sql = "INSERT INTO Tickets (Description, Status, UserID) VALUES (@Description, 'Pending', @UserID)";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@Description", description),
                        new SqlParameter("@UserID", userId));
                }
                else // UPDATE existing ticket (status only)
                {
                    string sql = "UPDATE Tickets SET Status=@Status WHERE TicketID=@ID";
                    rowsAffected = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@Status", status),
                        new SqlParameter("@ID", ticketId));
                }
            }

            else if (currentTable == "Clients") // Profile
            {
                string companyName = row.Cells["CompanyName"].Value == null ? "" : row.Cells["CompanyName"].Value.ToString();
                string contactNumber = row.Cells["ContactNumber"].Value == null ? "" : row.Cells["ContactNumber"].Value.ToString();
                string email = row.Cells["Email"].Value == null ? "" : row.Cells["Email"].Value.ToString();

                string sql = "UPDATE Clients SET CompanyName=@CompanyName, ContactNumber=@ContactNumber, Email=@Email WHERE ClientID=@ID";
                rowsAffected = da.ExecuteUpdateQuery(sql,
                    new SqlParameter("@CompanyName", companyName),
                    new SqlParameter("@ContactNumber", contactNumber),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@ID", clientId));
            }

            if (rowsAffected > 0)
            {
                // refresh current view
                if (currentTable == "Project") ShowProjects();
                else if (currentTable == "Invoices") ShowInvoices();
                else if (currentTable == "Tickets") ShowTickets();
                else if (currentTable == "Clients") ShowClientProfile();
            }
            else
            {
                MessageBox.Show("Operation failed.");
            }
        }

        // Handle clicks on the "Add" button embedded in the grid
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var dgv = (DataGridView)sender;
            if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                dgv.Columns[e.ColumnIndex].Name == AddButtonColumnName)
            {
                var row = dgv.Rows[e.RowIndex];
                if (row.IsNewRow)
                {
                    // new-row: user entered values in the new-row placeholder
                }

                int rowsAffected = 0;

                if (currentTable == "Project")
                {
                    string projectName = GetStringCellValue(row, "ProjectName");
                    if (string.IsNullOrWhiteSpace(projectName))
                    {
                        MessageBox.Show("Enter a project name before adding.");
                        return;
                    }

                    string sql = @"INSERT INTO Project (ProjectName, ClientID, EmployeeID, Status) 
                                   VALUES (@ProjectName, @ClientID, NULL, 'Pending')";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@ProjectName", projectName),
                        new SqlParameter("@ClientID", clientId));
                }
                else if (currentTable == "Tickets")
                {
                    string description = GetStringCellValue(row, "Description");
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        MessageBox.Show("Enter a description before adding.");
                        return;
                    }

                    string sql = "INSERT INTO Tickets (Description, Status, UserID) VALUES (@Description, 'Pending', @UserID)";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@Description", description),
                        new SqlParameter("@UserID", userId));
                }
                else if (currentTable == "Invoices")
                {
                    // Invoices do NOT have an Add button per the requested change,
                    // but keep the logic in case you re-enable it later.
                    decimal partial = GetDecimalCellValue(row, "PartialPaymentAmount");
                    string comments = GetStringCellValue(row, "Comments");

                    string sql = @"INSERT INTO Invoices (ClientID, ProjectID, Amount, PaymentStatus, IssueDate, DueDate, PartialPaymentAmount, Comments)
                                   VALUES (@ClientID, NULL, 0, 'Unpaid', GETDATE(), DATEADD(day,30,GETDATE()), @Partial, @Comments)";
                    rowsAffected = da.ExecuteInsertQuery(sql,
                        new SqlParameter("@ClientID", clientId),
                        new SqlParameter("@Partial", partial),
                        new SqlParameter("@Comments", comments));
                }
                else if (currentTable == "Clients")
                {
                    // In profile view treat Add button as "Save Profile" (UPDATE)
                    string companyName = GetStringCellValue(row, "CompanyName");
                    string contactNumber = GetStringCellValue(row, "ContactNumber");
                    string email = GetStringCellValue(row, "Email");

                    string sql = "UPDATE Clients SET CompanyName=@CompanyName, ContactNumber=@ContactNumber, Email=@Email WHERE ClientID=@ID";
                    rowsAffected = da.ExecuteUpdateQuery(sql,
                        new SqlParameter("@CompanyName", companyName),
                        new SqlParameter("@ContactNumber", contactNumber),
                        new SqlParameter("@Email", email),
                        new SqlParameter("@ID", clientId));
                }
                else
                {
                    MessageBox.Show("Add action is not supported for this view.");
                }

                if (rowsAffected > 0)
                {
                    // refresh current view
                    if (currentTable == "Project") ShowProjects();
                    else if (currentTable == "Tickets") ShowTickets();
                    else if (currentTable == "Invoices") ShowInvoices();
                    else if (currentTable == "Clients") ShowClientProfile();
                }
            }
        }

        private string GetStringCellValue(DataGridViewRow row, string columnName)
        {
            if (row == null || string.IsNullOrWhiteSpace(columnName)) return string.Empty;
            if (!dataGridView1.Columns.Contains(columnName)) return string.Empty;
            var v = row.Cells[columnName].Value;
            return (v == null || v == DBNull.Value) ? string.Empty : v.ToString().Trim();
        }

        private decimal GetDecimalCellValue(DataGridViewRow row, string columnName)
        {
            if (row == null || string.IsNullOrWhiteSpace(columnName)) return 0m;
            if (!dataGridView1.Columns.Contains(columnName)) return 0m;
            var v = row.Cells[columnName].Value;
            decimal d;
            if (v == null || v == DBNull.Value) return 0m;
            return decimal.TryParse(v.ToString(), out d) ? d : 0m;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login logout = new Login();
            logout.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Image != null)
            {
                // Remove image if already set
                button5.Image = null;
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select a Button Image";
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        button5.Image = Image.FromFile(ofd.FileName);
                    }
                }
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pppp(object sender, DataGridViewColumnEventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}