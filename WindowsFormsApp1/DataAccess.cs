using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class DataAccess
    {
        private readonly string connStr;

        public DataAccess()
        {
            // Make sure this matches your App.config key
            connStr = ConfigurationManager.ConnectionStrings["ITfirm_dbb"]?.ConnectionString;

            if (string.IsNullOrEmpty(connStr))
            {
                MessageBox.Show("Connection string 'ITfirm_dbb' not found in App.config.");
            }
        }

        public DataTable ExecuteQueryTable(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters?.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query failed:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        public int ExecuteUpdateQuery(string sql, params SqlParameter[] parameters)
        {
            int affectedRows = -1;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters?.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    affectedRows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return affectedRows;
        }
        public int ExecuteInsertQuery(string sql, params SqlParameter[] parameters)
        {
            return ExecuteUpdateQuery(sql, parameters); // reuse update method
        }

        public object ExecuteScalarQuery(string sql, params SqlParameter[] parameters)
        {
            object result = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters?.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scalar query failed:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test failed:\n{ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}
