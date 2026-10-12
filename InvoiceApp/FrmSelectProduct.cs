using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
namespace InvoiceApp
{
    public partial class FrmSelectProduct : Form
    {
        public int SelectedProductId { get; private set; }
        public string SelectedProductName { get; private set; }
        public decimal SelectedProductPrice { get; private set; }
        private DataTable productsTable;
        public FrmSelectProduct()
        {
            InitializeComponent();
        }

        private void LoadProducts()
        {
            string connectionString =
                "Server=.;Database=InvoiceAppDB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                SqlCommand command =
                    new SqlCommand("GetProducts", connection);

                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter =
                    new SqlDataAdapter(command);

                productsTable = new DataTable();

                adapter.Fill(productsTable);

                dgvProducts.DataSource = productsTable;
            }

            dgvProducts.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvProducts.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProducts.MultiSelect = false;
            dgvProducts.ReadOnly = true;

            dgvProducts.Columns["Id"].HeaderText = "کد کالا";
            dgvProducts.Columns["Name"].HeaderText = "نام کالا";
            dgvProducts.Columns["Price"].HeaderText = "قیمت";
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
           
            this.Close();


        }

        private void FrmSelectProduct_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک کالا انتخاب کنید.");
                return;
            }

            SelectedProductId =
                Convert.ToInt32(dgvProducts.CurrentRow.Cells["Id"].Value);

            SelectedProductName =
                dgvProducts.CurrentRow.Cells["Name"].Value.ToString();

            SelectedProductPrice =
                Convert.ToDecimal(dgvProducts.CurrentRow.Cells["Price"].Value);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string text = txtSearch.Text.Trim()
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("*", "[*]");

            productsTable.DefaultView.RowFilter =
                string.IsNullOrEmpty(text)
                    ? ""
                    : "Name LIKE '%" + text + "%'";
        }
    }
}
