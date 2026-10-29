
namespace InvoiceApp.Models
{
    public class InvoiceItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Amount
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        public InvoiceItem(
            int productId,
            string productName,
            int quantity,
            decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
