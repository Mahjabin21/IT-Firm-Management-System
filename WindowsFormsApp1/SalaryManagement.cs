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
    public partial class SalaryManagement : Form
    {
        private readonly DataAccess da = new DataAccess();
        public SalaryManagement()
        {
            InitializeComponent();
        }
        //employee id
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
        //bonus amount
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //base salary
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        //employee id
        private void label3_Click(object sender, EventArgs e)
        {
        }
        //bonus amount
        private void label2_Click(object sender, EventArgs e)
        {
        }
        //bonus amount
        private void label1_Click(object sender, EventArgs e)
        {
        }
        //add button
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Read values from textboxes
                decimal baseSalary = Convert.ToDecimal(textBox1.Text);   // BaseSalary
                decimal bonusAmount = Convert.ToDecimal(textBox2.Text);  // BonusAmount
                int employeeId = Convert.ToInt32(textBox3.Text);         // EmployeeID

                // SQL insert
                string sql = "INSERT INTO Salary (BaseSalary, BonusAmount, EmployeeID) VALUES (@BaseSalary, @BonusAmount, @EmployeeID)";
                int rows = da.ExecuteInsertQuery(sql,
                    new SqlParameter("@BaseSalary", baseSalary),
                    new SqlParameter("@BonusAmount", bonusAmount),
                    new SqlParameter("@EmployeeID", employeeId));

                if (rows > 0)
                {
                    MessageBox.Show("Salary record added successfully!");
                    this.DialogResult = DialogResult.OK; // notify AdminDashboard
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to add salary record.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        //cancel button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
