using InvoiceApp.Models;
using InvoiceApp.DataAccess;

using System;
using System.Globalization;
using System.Windows.Forms;



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
                    Product product = new Product(
                        frm.SelectedProductId,
                        frm.SelectedProductName,
                        frm.SelectedProductPrice
                    );

                    SetProductToRow(rowIndex, product);
                }
            }
        }

        private void SearchProduct(int rowIndex, string searchText, bool searchByCode)
        {
            ProductRepository repository = new ProductRepository();

            Product product = null;

            if (searchByCode)
            {
                if (int.TryParse(searchText, out int productId))
                {
                    product = repository.GetProductById(productId);
                }
            }
            else
            {
                product = repository.GetProductByName(searchText.Trim());
            }

            if (product == null)
            {
                MessageBox.Show("محصولی با این مشخصات پیدا نشد.");
                return;
            }

            SetProductToRow(rowIndex, product);
        }

        private bool ValidateInvoiceItems()
        {
            foreach (DataGridViewRow row in dgvInvoiceItems.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (!int.TryParse(
                    Convert.ToString(row.Cells["colProductCode"].Value),
                    out int productId) || productId <= 0)
                {
                    MessageBox.Show("یکی از ردیف‌ها محصول معتبری ندارد.");
                    return false;
                }

                if (!int.TryParse(
                    Convert.ToString(row.Cells["colQuantity"].Value),
                    out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("تعداد کالا باید عددی بزرگ‌تر از صفر باشد.");
                    return false;
                }

                if (!decimal.TryParse(
                    Convert.ToString(row.Cells["colUnitPrice"].Value),
                    out decimal unitPrice) || unitPrice <= 0)
                {
                    MessageBox.Show("قیمت کالا باید عددی بزرگ‌تر از صفر باشد.");
                    return false;
                }
            }

            return true;
        }

        private void SetProductToRow(int rowIndex, Product product)
        {
            if (product == null)
                return;

            DataGridViewRow row = dgvInvoiceItems.Rows[rowIndex];

            row.Cells["colProductCode"].Value = product.Id;
            row.Cells["colProductName"].Value = product.Name;
            row.Cells["colUnitPrice"].Value = product.Price;

            row.Cells["colQuantity"].Selected = true;
            dgvInvoiceItems.CurrentCell = row.Cells["colQuantity"];
            dgvInvoiceItems.BeginEdit(true);
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

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInvoiceItems())
            {
                return;
            }

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
                PersianCalendar pc = new PersianCalendar();

                string[] dateParts = txtDate.Text.Split('/');

                DateTime invoiceDate = pc.ToDateTime(
                    int.Parse(dateParts[0]),
                    int.Parse(dateParts[1]),
                    int.Parse(dateParts[2]),
                    0, 0, 0, 0
                );

                InvoiceRepository repository = new InvoiceRepository();

                int invoiceId = repository.InsertInvoice(
                    Convert.ToInt32(txtInvoiceNumber.Text),
                    invoiceDate,
                    DateTime.Now.TimeOfDay,
                    txtCustomer.Text,
                    selectedCustomerId
                );

                InvoiceItemRepository itemRepository = new InvoiceItemRepository();

                foreach (DataGridViewRow row in dgvInvoiceItems.Rows)
                {
                    if (row.IsNewRow) continue;

                    int productId = Convert.ToInt32(row.Cells["colProductCode"].Value);
                    int quantity = Convert.ToInt32(row.Cells["colQuantity"].Value);
                    decimal unitPrice = Convert.ToDecimal(row.Cells["colUnitPrice"].Value);
                    decimal amount = Convert.ToDecimal(row.Cells["colAmount"].Value);

                    itemRepository.InsertInvoiceItem(
                        invoiceId,
                        productId,
                        quantity,
                        unitPrice,
                        amount
                    );
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
            
        }

        private void txtCustomer_Click(object sender, EventArgs e)
        {
            using (FrmSelectCustomer frm = new FrmSelectCustomer())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {

                    if (frm.SelectedCustomer != null)
                    {
                        Customer customer = frm.SelectedCustomer;

                        txtCustomer.Text =
                            customer.FirstName + " " + customer.LastName;

                        selectedCustomerId = customer.Id;
                    }

                }
            }
        }





        private bool skipNextCellClick;
        private void dgvInvoiceItems_CellClick(
    object sender,
    DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (skipNextCellClick)
            {
                skipNextCellClick = false;
                return;
            }

            string col = dgvInvoiceItems.Columns[e.ColumnIndex].Name;

            if (col == "colProductCode" || col == "colProductName")
            {
                SelectProduct(e.RowIndex);
            }
        }

        

        private void dgvInvoiceItems_CellValueChanged(object sender,DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string col = dgvInvoiceItems.Columns[e.ColumnIndex].Name;

            if (col != "colQuantity" && col != "colUnitPrice")
                return;

            DataGridViewRow row = dgvInvoiceItems.Rows[e.RowIndex];

            bool validQuantity = int.TryParse(
                Convert.ToString(row.Cells["colQuantity"].Value),
                out int quantity);

            bool validPrice = decimal.TryParse(
                Convert.ToString(row.Cells["colUnitPrice"].Value),
                out decimal unitPrice);

            bool validProductId = int.TryParse(
                Convert.ToString(row.Cells["colProductCode"].Value),
                out int productId);

            if (!validProductId || productId <= 0)
            {
                row.Cells["colAmount"].Value = null;
                CalculateTotalAmount();
                return;
            }

            if (validQuantity && validPrice)
            {
                InvoiceItem item = new InvoiceItem(
                    productId,
                    Convert.ToString(row.Cells["colProductName"].Value),
                    quantity,
                    unitPrice);

                row.Cells["colAmount"].Value = item.Amount;
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

                if (col == "colProductCode")
                {
                    string searchText = Convert.ToString(cell.Value).Trim();
                    SearchProduct(cell.RowIndex, searchText, true);
                }
                else if (col == "colProductName")
                {
                    string searchText = Convert.ToString(cell.Value).Trim();
                    SearchProduct(cell.RowIndex, searchText, false);
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
        private void FrmInvoice_Load(object sender, EventArgs e)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime now = DateTime.Now;

            txtDate.Text =
                $"{pc.GetYear(now):0000}/{pc.GetMonth(now):00}/{pc.GetDayOfMonth(now):00}";

            txtTime.Text = now.ToString("HH:mm:ss");

            InvoiceRepository repository = new InvoiceRepository();

            txtInvoiceNumber.Text =
                repository.GetNextInvoiceNumber().ToString();

            CalculateTotalAmount();
        }

        private void editItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvInvoiceItems.CurrentCell == null)
                return;

            string col = dgvInvoiceItems.Columns[
                dgvInvoiceItems.CurrentCell.ColumnIndex
            ].Name;

            if (col != "colProductCode" && col != "colProductName")
                return;

            dgvInvoiceItems.BeginEdit(true);
        }
    }
}