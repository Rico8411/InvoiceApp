using InvoiceApp.DataAccess;
using System;
using System.Windows.Forms;
using InvoiceApp.Models;



namespace InvoiceApp
{
    public partial class FrmSelectCustomer : Form
    {

        public Customer SelectedCustomer { get; private set; }


        public FrmSelectCustomer()
        {
            InitializeComponent();
        }

        private void LoadCustomers(string searchText = "")
        {
            CustomerRepository repository = new CustomerRepository();

            dgvCustomers.DataSource = repository.GetCustomers(searchText);

            dgvCustomers.Columns["Id"].Visible = false;

            dgvCustomers.Columns["FirstName"].HeaderText = "نام";
            dgvCustomers.Columns["LastName"].HeaderText = "نام خانوادگی";

            dgvCustomers.Columns["FirstName"].DisplayIndex = 0;
            dgvCustomers.Columns["LastName"].DisplayIndex = 1;
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

            int id = Convert.ToInt32(
                dgvCustomers.CurrentRow.Cells["Id"].Value);

            string firstName =
                dgvCustomers.CurrentRow.Cells["FirstName"].Value?.ToString() ?? "";

            string lastName =
                dgvCustomers.CurrentRow.Cells["LastName"].Value?.ToString() ?? "";

            SelectedCustomer = new Customer(id, firstName, lastName);

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
