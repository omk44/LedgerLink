# LedgerLink - Digital Ledger Management System

## Demo

- **LIVE DEMO**: https://ledgerlink-app.onrender.com
- DEMO VIDEO: https://youtu.be/7z4iqupO4BA?si=ftecDmpzImNlgg2I
- github Repo: https://github.com/omk44/LedgerLink

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



## Contributors & Project Development

This LedgerLink project was collaboratively developed by a dedicated team of three developers, each bringing specialized expertise to create a comprehensive digital ledger management system.

### 👨‍💻 **Contributors**

- **Om**: Festival management, Discount rules, Transaction controller, Email sender process
- **Meet**: Customer CRUD operations, Authentication system,Scanner process,
- **Jewel**: Product management, UI/UX design

### 🏗️ **Project Development**

**Team Collaboration**: The LedgerLink project was developed by a team of three contributors, each focusing on specific modules and functionality to create a comprehensive digital ledger management system.

### 🛠️ **Technology Stack Used**
- **Backend**: ASP.NET Core MVC, Entity Framework Core
- **Database**: PostgreSQL with Npgsql provider
- **Frontend**: Razor Pages, Bootstrap 5, Custom CSS/SCSS
- **Additional Libraries**: X.PagedList, QRCoder, SendGrid, Twilio

### 🚀 **Key Achievements**
- Successfully implemented a complete MVC architecture with clear separation of concerns
- Created a scalable and maintainable codebase following industry best practices
- Delivered a fully functional business management system with modern UI/UX
- Integrated multiple third-party services (email, SMS, QR code generation)
- Implemented robust authentication, session management, and data validation


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
- **Authentication**: Session-based authentication with BCrypt password hashing
- **Email Service**: Gmail SMTP via MailKit/MimeKit
- **QR Code Generation**: QRCoder library
- **Pagination**: X.PagedList
- **Deployment**: Docker & Docker Compose

## 🚀 Quick Start (Docker - Recommended)

### Prerequisites
- Docker & Docker Compose installed
- Gmail account with App Password enabled

### Setup

1. Clone the repository
```bash
git clone https://github.com/yourusername/LedgerLink.git
cd LedgerLink
```

2. Navigate to docker folder
```bash
cd docker
```

3. Review the `.env` file (pre-configured)
```bash
cat .env  # Your Gmail SMTP is already configured
```

4. Start with Docker
```bash
docker-compose up -d
```

5. Access the application
```
http://localhost:8080
```

6. Create your first admin account through the web interface

### View Logs
```bash
cd docker
docker-compose logs -f app
```

### Stop Services
```bash
cd docker
docker-compose down
```

**📚 Detailed Docker Guide**: See [docker/README.md](docker/README.md)

---

## Getting Started (Traditional Installation)

### Prerequisites
- .NET 8.0 SDK or later
- PostgreSQL database server
- Visual Studio 2022 or any compatible IDE

### Prerequisites (Traditional)
- .NET 8.0 SDK or later
- PostgreSQL database server
- Visual Studio 2022 or any compatible IDE
- Gmail account with App Password

### Installation Steps

1. Clone the repository
```bash
git clone https://github.com/yourusername/LedgerLink.git
cd LedgerLink
```

2. Update `appsettings.json` with your database and email settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ledgerlink;Username=yourusername;Password=yourpassword"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-gmail-app-password"
  }
}
```

3. Apply database migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

5. Access the application at `https://localhost:5001`

6. Create admin account through web interface

**Note**: Admin credentials are now stored securely in the database with BCrypt hashing.

## Configuration

The application can be configured through `appsettings.json` or environment variables:

```json
{
  "ShopSettings": {
    "ShopName": "Your Shop Name",
    "AppName": "LedgerLink",
    "ShopEmail": "info@yourshop.com",
    "ShopPhoneNumber": "+1234567890"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-gmail-app-password",
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Your Shop Name"
  }
}
```

**For Docker**: Use the `.env` file in the `docker` folder instead.


## License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📦 Deployment Options

- **Docker** (Recommended): See [docker/README.md](docker/README.md)
- **Cloud Platforms**: See [RENDER-DEPLOYMENT.md](RENDER-DEPLOYMENT.md)
- **Traditional**: Follow "Getting Started (Traditional Installation)" above

## Acknowledgements

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Bootstrap](https://getbootstrap.com/)
- [QRCoder](https://github.com/codebude/QRCoder)
- [MailKit](https://github.com/jstedfast/MailKit) & [MimeKit](https://github.com/jstedfast/MimeKit)
- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)
- [X.PagedList](https://github.com/dncuug/X.PagedList)
- [Docker](https://www.docker.com/)
