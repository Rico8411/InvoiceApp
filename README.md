# InvoiceApp

A desktop invoice entry application for Windows, built with **C#** and **Windows Forms** (.NET Framework 4.7.2). It connects to **Microsoft SQL Server** to manage customers, products, and sales invoices. The user interface uses **Persian (Farsi)** labels and **Jalali (Persian) calendar** dates for the invoice date field.

## Features

- **Invoice header** — Auto-generated invoice number, Jalali date, current time, and customer selection
- **Customer picker** — Search and select customers from the database (`FrmSelectCustomer`)
- **Product picker** — Browse and filter products by name; unit price is filled automatically (`FrmSelectProduct`)
- **Line items** — Add rows with product code, name, quantity, unit price, and computed line amount
- **Keyboard workflow** — Press **Enter** in the grid to move between product selection, quantity, and unit price, then start a new line
- **Persistence** — Save the invoice header and all line items via SQL Server stored procedures

## Requirements

| Component | Version / notes |
|-----------|-----------------|
| OS | Windows |
| .NET | .NET Framework **4.7.2** or later |
| IDE (optional) | Visual Studio 2017+ with desktop development workload |
| Database | **SQL Server** (local or remote) with database **`InvoiceAppDB`** |

## Database

The app expects a SQL Server database named **`InvoiceAppDB`**. Connection settings (used throughout the project) are:

- **Server:** `.` (local default instance)
- **Authentication:** Windows (`Trusted_Connection=True`)
- **Trust server certificate:** enabled

You can centralize this string in `InvoiceApp/Database.cs` or in `App.config` if you refactor connection usage.

### Stored procedures

The application calls these stored procedures (definitions are not included in this repository—you must create them on your server):

| Procedure | Purpose |
|-----------|---------|
| `GetNextInvoiceNumber` | Returns the next invoice number (scalar) |
| `GetProducts` | Returns products with columns `Id`, `Name`, `Price` |
| `SearchCustomers` | Parameter `@SearchText` (NVARCHAR); returns customers with `Id`, `FirstName`, `LastName` |
| `InsertInvoice` | Parameters `@InvoiceNumber`, `@InvoiceDate`, `@CustomerName`, `@CustomerId`, `@InvoiceTime`; returns new invoice `Id` |
| `InsertInvoiceItem` | Parameters `@InvoiceId`, `@ProductId`, `@Quantity`, `@UnitPrice`, `@Amount` |

Ensure tables and procedures match these names and parameter contracts before running the app.

## Getting started

1. **Clone the repository**

   ```bash
   git clone https://github.com/<your-org>/InvoiceApp.git
   cd InvoiceApp
   ```

2. **Prepare SQL Server** — Create `InvoiceAppDB`, seed customers and products, and deploy the stored procedures listed above.

3. **Open the solution** — Open `InvoiceApp.sln` in Visual Studio.

4. **Build and run** — Set `InvoiceApp` as the startup project, then build and run (F5). The main form is `FrmInvoice`.

If the connection fails, verify SQL Server is running, the database exists, and your Windows account has access. Update the connection string if you use a named instance or remote server.

## Project structure

```
InvoiceApp/
├── InvoiceApp.sln          # Visual Studio solution
├── InvoiceApp/
│   ├── Program.cs            # Application entry point
│   ├── FrmInvoice.cs         # Main invoice form
│   ├── FrmSelectCustomer.cs  # Customer search/selection dialog
│   ├── FrmSelectProduct.cs   # Product selection dialog
│   ├── Product.cs            # Product model
│   ├── Database.cs           # Shared SQL connection helper
│   └── App.config
├── LICENSE.txt               # MIT License
└── README.md
```

## Usage (summary)

1. On load, the form sets today’s **Jalali date**, current **time**, and the **next invoice number**.
2. Click the **customer** field to open the customer dialog; search and confirm your selection.
3. In the grid, click **product code** or **product name** (or use Enter) to pick a product.
4. Enter **quantity** and **unit price**; the **amount** column updates automatically.
5. Click **Save** to persist the invoice (requires a customer and at least one line item).

Validation messages and many UI strings are in Persian.

## License

This project is licensed under the [MIT License](LICENSE.txt).
