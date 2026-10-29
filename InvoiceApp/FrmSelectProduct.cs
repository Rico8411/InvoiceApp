using InvoiceApp.DataAccess;
using InvoiceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InvoiceApp
{
    public partial class FrmSelectProduct : Form
    {
        public int SelectedProductId { get; private set; }
        public string SelectedProductName { get; private set; }
        public decimal SelectedProductPrice { get; private set; }

        private List<Product> products;
        public FrmSelectProduct()
        {
            InitializeComponent();
        }

        private void LoadProducts()
        {
            ProductRepository repository = new ProductRepository();

            products = repository.GetProducts();

            dgvProducts.DataSource = products;

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
            string text = txtSearch.Text.Trim();

            List<Product> filteredProducts = new List<Product>();

            foreach (Product product in products)
            {
                if (product.Name.IndexOf(
                    text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredProducts.Add(product);
                }
            }

            dgvProducts.DataSource = filteredProducts;
        }
    }
}
