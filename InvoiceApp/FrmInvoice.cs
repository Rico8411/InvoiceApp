using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;


namespace InvoiceApp
{
    public partial class FrmInvoice : Form
    {
        public FrmInvoice()
        {
            InitializeComponent();
        }

        private int selectedCustomerId;




        private void SelectProduct(int rowIndex)
        {
            using (FrmSelectProduct frm = new FrmSelectProduct())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    DataGridViewRow row = dgvInvoiceItems.Rows[rowIndex];

                    row.Cells["colProductCode"].Value = frm.SelectedProductId;
                    row.Cells["colProductName"].Value = frm.SelectedProductName;
                    row.Cells["colUnitPrice"].Value = frm.SelectedProductPrice;

                    dgvInvoiceItems.CurrentCell = row.Cells["colQuantity"];
                    dgvInvoiceItems.BeginEdit(true);
                }
            }
        }




        private int GetNextInvoiceNumber()
        {
            using (SqlConnection connection = Database.OpenConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand("GetNextInvoiceNumber", connection);

                command.CommandType = CommandType.StoredProcedure;

                object result = command.ExecuteScalar();

                return Convert.ToInt32(result);
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection connection = Database.OpenConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand("GetProducts", connection);

                command.CommandType = CommandType.StoredProcedure;

                List<Product> products = new List<Product>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            Convert.ToDecimal(reader["Price"])
                        );

                        products.Add(product);
                    }
                }


            }
        }
        private int InsertInvoice()
        {
            using (SqlConnection connection = Database.OpenConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand("InsertInvoice", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(
                    "@InvoiceNumber",
                    Convert.ToInt32(txtInvoiceNumber.Text));

                PersianCalendar pc = new PersianCalendar();

                string[] dateParts = txtDate.Text.Split('/');

                int year = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int day = int.Parse(dateParts[2]);

                DateTime invoiceDate = pc.ToDateTime(
                    year, month, day, 0, 0, 0, 0
                );

                command.Parameters.Add("@InvoiceDate", SqlDbType.Date).Value =
                    invoiceDate.Date;

                command.Parameters.AddWithValue(
                    "@CustomerName",
                    txtCustomer.Text);

                command.Parameters.AddWithValue(
                    "@CustomerId",
                    selectedCustomerId);

                command.Parameters.AddWithValue(
                    "@InvoiceTime",
                    DateTime.Now.TimeOfDay);


                object result = command.ExecuteScalar();

                return Convert.ToInt32(result);
            }
        }
        private void InsertInvoiceItem(
            int invoiceId,
            int productId,
            int quantity,
            decimal unitPrice,
            decimal amount)
        {
            using (SqlConnection connection = Database.OpenConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand("InsertInvoiceItem", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@InvoiceId", invoiceId);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@UnitPrice", unitPrice);
                command.Parameters.AddWithValue("@Amount", amount);

                command.ExecuteNonQuery();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            PersianCalendar pc = new PersianCalendar();

            DateTime now = DateTime.Now;

            txtDate.Text =
                $"{pc.GetYear(now):0000}/{pc.GetMonth(now):00}/{pc.GetDayOfMonth(now):00}";

            txtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            txtInvoiceNumber.Text = GetNextInvoiceNumber().ToString();

            CalculateTotalAmount();

        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }



        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (selectedCustomerId <= 0)
            {
                MessageBox.Show(
                    "لطفاً ابتدا مشتری را انتخاب کنید.",
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            if (dgvInvoiceItems.Rows.Count <= 1)
            {
                MessageBox.Show("حداقل یک کالا به فاکتور اضافه کنید.");
                return;
            }


            try
            {
                int invoiceId = InsertInvoice();

                foreach (DataGridViewRow row in dgvInvoiceItems.Rows)
                {
                    if (row.IsNewRow) continue;

                    int productId = Convert.ToInt32(row.Cells["colProductCode"].Value);
                    int quantity = Convert.ToInt32(row.Cells["colQuantity"].Value);
                    decimal unitPrice = Convert.ToDecimal(row.Cells["colUnitPrice"].Value);
                    decimal amount = Convert.ToDecimal(row.Cells["colAmount"].Value);

                    InsertInvoiceItem(invoiceId, productId, quantity, unitPrice, amount);
                }

                MessageBox.Show("فاکتور با موفقیت ثبت شد.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت فاکتور: " + ex.Message);
            }
        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {
            FrmSelectCustomer frm = new FrmSelectCustomer();
            frm.ShowDialog();
        }

        private void txtCustomer_Click(object sender, EventArgs e)
        {
            using (FrmSelectCustomer frm = new FrmSelectCustomer())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtCustomer.Text =
                        frm.SelectedFirstName + " " + frm.SelectedLastName;


                    selectedCustomerId = frm.SelectedCustomerId;
                }
            }
        }




        private void txtDate_TextChanged(object sender, EventArgs e)
        {

        }

        private bool skipNextCellClick;
        private void dgvInvoiceItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (skipNextCellClick)
            {
                skipNextCellClick = false;
                return;
            }

            string col = dgvInvoiceItems.Columns[e.ColumnIndex].Name;

            if (col == "colProductCode" || col == "colProductName")
                SelectProduct(e.RowIndex);
        }

        private void dgvInvoiceItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvInvoiceItems.Columns[e.ColumnIndex].Name;
            if (col != "colQuantity" && col != "colUnitPrice") return;

            DataGridViewRow row = dgvInvoiceItems.Rows[e.RowIndex];

            if (int.TryParse(Convert.ToString(row.Cells["colQuantity"].Value), out int quantity) &&
                decimal.TryParse(Convert.ToString(row.Cells["colUnitPrice"].Value), out decimal unitPrice))
            {
                row.Cells["colAmount"].Value = quantity * unitPrice;
            }
            else
            {
                row.Cells["colAmount"].Value = null;
            }
            CalculateTotalAmount();
        }

        private void dgvInvoiceItems_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotalAmount();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool gridActive = dgvInvoiceItems.Focused || dgvInvoiceItems.IsCurrentCellInEditMode;

            if (keyData == Keys.Enter && gridActive && dgvInvoiceItems.CurrentCell != null)
            {
                dgvInvoiceItems.EndEdit();

                DataGridViewCell cell = dgvInvoiceItems.CurrentCell;
                string col = dgvInvoiceItems.Columns[cell.ColumnIndex].Name;
                DataGridViewRow row = dgvInvoiceItems.Rows[cell.RowIndex];

                if (col == "colProductCode" || col == "colProductName")
                {
                    SelectProduct(cell.RowIndex);
                }
                else if (col == "colQuantity")
                {
                    dgvInvoiceItems.CurrentCell = row.Cells["colUnitPrice"];
                    dgvInvoiceItems.BeginEdit(true);
                }
                else if (col == "colUnitPrice")
                {
                    if (row.Cells["colProductCode"].Value == null)
                    {
                        MessageBox.Show("لطفاً کالا را انتخاب کنید.");
                        return true;
                    }

                    if (!int.TryParse(Convert.ToString(row.Cells["colQuantity"].Value), out _))
                    {
                        MessageBox.Show("تعداد را وارد کنید.");
                        return true;
                    }

                    if (!decimal.TryParse(Convert.ToString(row.Cells["colUnitPrice"].Value), out _))
                    {
                        MessageBox.Show("فی کالا نامعتبر است.");
                        return true;
                    }


                    int newRowIndex = dgvInvoiceItems.NewRowIndex;
                    if (newRowIndex >= 0)
                    {
                        dgvInvoiceItems.CurrentCell =
                            dgvInvoiceItems.Rows[newRowIndex].Cells["colProductCode"];
                    }
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }
        private void CalculateTotalAmount()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvInvoiceItems.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (decimal.TryParse(
                    Convert.ToString(row.Cells["colAmount"].Value),
                    out decimal amount))
                {
                    total += amount;
                }
            }

            txtTotalAmount.Text = total.ToString("N0");
        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {

        }



        private void dgvInvoiceItems_CellMouseDown(
            object sender,
            DataGridViewCellMouseEventArgs e)
        {
            skipNextCellClick = (e.Button == MouseButtons.Right);

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvInvoiceItems.ClearSelection();

                dgvInvoiceItems.Rows[e.RowIndex].Selected = true;

                dgvInvoiceItems.CurrentCell =
                    dgvInvoiceItems.Rows[e.RowIndex].Cells[e.ColumnIndex];

                contextMenuStripItems.Show(
                    Cursor.Position
                );
            }
        }

        private void contextMenuStripItems_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void deleteItemToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            if (dgvInvoiceItems.CurrentRow == null ||
                dgvInvoiceItems.CurrentRow.IsNewRow)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "آیا از حذف این کالا مطمئن هستید؟",
                "تأیید حذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                dgvInvoiceItems.Rows.Remove(
                    dgvInvoiceItems.CurrentRow
                );

                CalculateTotalAmount();
            }
        }
    }
}