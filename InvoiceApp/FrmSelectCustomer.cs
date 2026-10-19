using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceApp
{
    public partial class FrmSelectCustomer : Form
    {

        public int SelectedCustomerId { get; private set; }
        public string SelectedFirstName { get; private set; }
        public string SelectedLastName { get; private set; }

        public FrmSelectCustomer()
        {
            InitializeComponent();
        }

        private void LoadCustomers(string searchText = "")
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command = Database.CreateStoredProcedureCommand(connection, "GetCustomers"))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(
                    "@SearchText", SqlDbType.NVarChar, 100
                ).Value = searchText;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dgvCustomers.DataSource = table;
                    dgvCustomers.Columns["Id"].Visible = false;

                    dgvCustomers.Columns["FirstName"].HeaderText = "نام";
                    dgvCustomers.Columns["LastName"].HeaderText = "نام خانوادگی";

                    dgvCustomers.Columns["FirstName"].DisplayIndex = 0;
                    dgvCustomers.Columns["LastName"].DisplayIndex = 1;
                }
            }
        }


        private void FrmSelectCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک مشتری انتخاب کنید.");
                return;
            }

            SelectedCustomerId =
                Convert.ToInt32(dgvCustomers.CurrentRow.Cells["Id"].Value);

            SelectedFirstName =
                dgvCustomers.CurrentRow.Cells["FirstName"].Value.ToString();

            SelectedLastName =
                dgvCustomers.CurrentRow.Cells["LastName"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            LoadCustomers(txtSearch.Text.Trim());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult= DialogResult.Cancel;
            this.Close();
        }
    }
}
