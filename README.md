# LedgerLink - Digital Ledger Management System

## Demo

![Demo_Video](https://youtu.be/7z4iqupO4BA?si=ftecDmpzImNlgg2I)

## Overview

LedgerLink is a comprehensive digital ledger management system designed for small to medium-sized businesses to efficiently manage customer accounts, track transactions, handle payments, and implement promotional discounts. Built with ASP.NET Core, this application simplifies the traditional paper-based ledger system with a modern, secure, and user-friendly digital solution.

## Features

### Customer Management
- Create and manage customer profiles with contact information
- Generate unique QR codes for each customer for quick identification
- Track customer balances and transaction history

### Product Management
- Maintain a catalog of products with pricing information
- Track product inventory and sales statistics
- Easily update product details and pricing

### Transaction Processing
- Record sales transactions with multiple payment options
- Support for both cash and credit transactions
- Apply festival-based discounts automatically
- Generate detailed receipts for customers

### Payment Tracking
- Record and track customer payments
- Automatically update customer balances
- Send payment confirmation emails to customers

### Discount & Festival Management
- Create and manage seasonal festivals and special events
- Configure discount rules based on various parameters
- Apply automatic discounts during festival periods

### Dashboard & Reporting
- Comprehensive dashboard with key business metrics
- Visual representation of sales and payment trends
- Quick access to outstanding balances and recent transactions

## Project Workflow

1. **Authentication Flow**
   - User accesses the application and is redirected to the login page
   - After successful authentication, user is directed to the dashboard
   - Session-based authentication maintains the user's logged-in state

2. **Customer Management Flow**
   - Admin creates a new customer with contact details
   - System generates a unique QR code for the customer
   - Admin can view, edit, or delete customer information
   - Customer balances are automatically updated based on transactions and payments

3. **Transaction Processing Flow**
   - Admin scans customer QR code or selects customer from the list
   - System displays customer details and transaction history
   - Admin adds products to the transaction with quantities
   - System automatically calculates applicable discounts based on active festivals
   - Admin finalizes the transaction as cash or credit
   - System updates customer balance and generates a receipt

4. **Payment Processing Flow**
   - Admin selects a customer with outstanding balance
   - Admin records payment amount and payment method
   - System updates customer balance and generates a payment receipt
   - Email notification can be sent to the customer

5. **Festival and Discount Management Flow**
   - Admin creates festivals with start and end dates
   - Admin configures discount rules associated with festivals
   - System automatically applies discounts during active festivals
   - Discounts can be based on purchase amount, customer balance, or other criteria

## API Endpoints

### Authentication
- `GET /Account/Login` - Display login form
- `POST /Account/Login` - Process login credentials
- `GET /Account/Logout` - Log out the current user

### Dashboard
- `GET /Dashboard/Index` - Display dashboard with optional date filtering

### Customer Management
- `GET /Customer/Index` - List all customers
- `GET /Customer/Create` - Display customer creation form
- `POST /Customer/Create` - Create a new customer
- `GET /Customer/Edit/{id}` - Display customer edit form
- `POST /Customer/Edit/{id}` - Update customer information
- `GET /Customer/Delete/{id}` - Display customer deletion confirmation
- `POST /Customer/Delete/{id}` - Delete a customer
- `GET /Customer/ShowQrCode/{id}` - Display customer QR code

### Product Management
- `GET /Product/Index` - List all products
- `GET /Product/Create` - Display product creation form
- `POST /Product/Create` - Create a new product
- `GET /Product/Edit/{id}` - Display product edit form
- `POST /Product/Edit/{id}` - Update product information
- `GET /Product/Delete/{id}` - Display product deletion confirmation
- `POST /Product/Delete/{id}` - Delete a product

### Transaction Management
- `GET /Transaction/Scan` - Display QR code scanner
- `POST /Transaction/ProcessScan` - Process scanned QR code
- `GET /Transaction/CustomerDetails/{id}` - Display customer details for transaction
- `POST /Transaction/AddItem` - Add item to customer transaction
- `POST /Transaction/AddPayment` - Record payment for customer
- `GET /Transaction/ShowReceipt` - Display transaction or payment receipt
- `POST /Transaction/CalculateDiscount` - Calculate applicable discounts

### Festival Management
- `GET /Festival/Index` - List all festivals
- `GET /Festival/Create` - Display festival creation form
- `POST /Festival/Create` - Create a new festival
- `GET /Festival/Edit/{id}` - Display festival edit form
- `POST /Festival/Edit/{id}` - Update festival information
- `GET /Festival/Delete/{id}` - Display festival deletion confirmation
- `POST /Festival/Delete/{id}` - Delete a festival

### Discount Rule Management
- `GET /DiscountRule/Index/{festivalId}` - List discount rules for a festival
- `GET /DiscountRule/Create/{festivalId}` - Display discount rule creation form
- `POST /DiscountRule/Create` - Create a new discount rule
- `GET /DiscountRule/Edit/{id}` - Display discount rule edit form
- `POST /DiscountRule/Edit` - Update discount rule information
- `GET /DiscountRule/Delete/{id}` - Display discount rule deletion confirmation
- `POST /DiscountRule/Delete/{id}` - Delete a discount rule

## Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Frontend**: Bootstrap, jQuery, HTML5, CSS3
- **Authentication**: Session-based authentication
- **Email Service**: SendGrid integration
- **QR Code Generation**: QRCoder library
- **Pagination**: X.PagedList

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- PostgreSQL database server
- Visual Studio 2022 or any compatible IDE

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/LedgerLink.git
```

2. Navigate to the project directory
```bash
cd LedgerLink
```

3. Update the connection string in `appsettings.json` to point to your PostgreSQL database
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ledgerlink;Username=yourusername;Password=yourpassword"
}
```

4. Apply database migrations
```bash
dotnet ef database update
```

5. Run the application
```bash
dotnet run
```

6. Access the application at `https://localhost:5001`

### Default Login
- Username: admin
- Password: password

**Note**: Please change the default credentials in production.

## Configuration

The application can be configured through the `appsettings.json` file:

```json
{
  "ShopSettings": {
    "ShopName": "Your Shop Name",
    "AppName": "LedgerLink",
    "Currency": "₹"
  },
  "EmailSettings": {
    "SendGridApiKey": "YOUR_SENDGRID_API_KEY",
    "SenderEmail": "your-email@example.com",
    "SenderName": "Your Shop Name"
  }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Bootstrap](https://getbootstrap.com/)
- [QRCoder](https://github.com/codebude/QRCoder)
- [SendGrid](https://sendgrid.com/)
- [X.PagedList](https://github.com/dncuug/X.PagedList)