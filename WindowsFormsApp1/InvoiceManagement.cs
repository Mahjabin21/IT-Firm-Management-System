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
    public partial class InvoiceManagement : Form
    {
        private readonly DataAccess da = new DataAccess();
        public InvoiceManagement()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate amount (now int)
            if (!int.TryParse(textBox1.Text, out int amount))
            {
                MessageBox.Show("Please enter a valid whole number amount.");
                return;
            }

            // Validate projectId
            if (!int.TryParse(textBox3.Text, out int projectId))
            {
                MessageBox.Show("Please enter a valid Project ID.");
                return;
            }

            // Validate clientId
            if (!int.TryParse(textBox2.Text, out int clientId))
            {
                MessageBox.Show("Please enter a valid Client ID.");
                return;
            }

            string paymentStatus = comboBox1.SelectedItem.ToString();

            DateTime issueDate = dateTimePicker1.Value;
            DateTime dueDate = dateTimePicker2.Value;

            string sql = @"INSERT INTO Invoices 
    (Amount, PaymentStatus, IssueDate, DueDate, ProjectID, ClientID) 
    VALUES (@Amount, @PaymentStatus, @IssueDate, @DueDate, @ProjectID, @ClientID)";

            int rows = da.ExecuteInsertQuery(sql,
                new SqlParameter("@ProjectID", projectId),
                new SqlParameter("@ClientID", clientId),
                new SqlParameter("@IssueDate", issueDate),
                new SqlParameter("@DueDate", dueDate),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@PaymentStatus", paymentStatus));

            if (rows > 0)
            {
                MessageBox.Show("Invoice added successfully!");
                this.DialogResult = DialogResult.OK; // refresh parent grid
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to add invoice.");
            }
        }




        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
