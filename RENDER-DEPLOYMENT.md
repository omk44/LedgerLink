# LedgerLink - Render Deployment Guide

Deploy your LedgerLink ASP.NET Core application on Render using Docker.

## 🚀 Why Render?

- ✅ Native Docker support
- ✅ Free PostgreSQL database
- ✅ Automatic SSL/HTTPS
- ✅ Auto-deploy from Git
- ✅ Easy environment variables
- ✅ No credit card required for free tier

## 📋 Prerequisites

1. **GitHub Account** - Code repository
2. **Render Account** - Sign up at https://render.com
3. **Gmail Account** - For email functionality (SMTP) with App Password enabled

## 🎯 Quick Deployment Steps

### Step 1: Prepare Your Repository

1. **Push code to GitHub:**
```bash
cd /home/om/dotnet/LedgerLink
git init
git add .
git commit -m "Initial commit - LedgerLink v2.0"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/LedgerLink.git
git push -u origin main
```

### Step 2: Create PostgreSQL Database on Render

1. Go to https://dashboard.render.com
2. Click **"New +"** → **"PostgreSQL"**
3. Configure:
   - **Name**: `ledgerlink-db`
   - **Database**: `ledgerlink`
   - **User**: `ledgerlink_user`
   - **Region**: Choose closest to you
   - **Plan**: Free (or paid for production)
4. Click **"Create Database"**
5. **Save these values** (from "Info" tab):
   - Internal Database URL
   - External Database URL

### Step 3: Create Web Service on Render

1. Click **"New +"** → **"Web Service"**
2. Connect your GitHub repository
3. Configure:

   **Basic Settings:**
   - **Name**: `ledgerlink-app`
   - **Region**: Same as database
   - **Branch**: `main`
   - **Runtime**: `Docker`
   - **Instance Type**: Free (or paid)

   **Build Settings:**
   - **Dockerfile Path**: `Dockerfile` (auto-detected)

   **Advanced:**
   - **Health Check Path**: `/`

### Step 4: Set Environment Variables

In the Render dashboard, go to your web service → **"Environment"** tab:

Add these variables:

```bash
# Database (use Internal Database URL from Step 2)
ConnectionStrings__DefaultConnection=Host=YOUR_DB_HOST;Port=5432;Database=ledgerlink;Username=ledgerlink_user;Password=YOUR_DB_PASSWORD;SSL Mode=Require

# Email Configuration (Gmail SMTP)
EmailSettings__SmtpHost=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SmtpUsername=your_email@gmail.com
EmailSettings__SmtpPassword=your_gmail_app_password
EmailSettings__SenderEmail=your_email@gmail.com
EmailSettings__SenderName=LedgerLink

# Shop Configuration
ShopSettings__ShopName=Miracle Technoz
ShopSettings__ShopEmail=info@miracletechnoz.com
ShopSettings__ShopPhoneNumber=+919876543210

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

**Important**: For `ConnectionStrings__DefaultConnection`, use the **Internal Database URL** and add `;SSL Mode=Require` at the end.

### Step 4.5: Setup Gmail App Password

To use Gmail SMTP for sending password reset emails:

1. **Enable 2-Step Verification**:
   - Go to: https://myaccount.google.com/security
   - Enable "2-Step Verification"

2. **Generate App Password**:
   - Visit: https://myaccount.google.com/apppasswords
   - Select app: "Mail"
   - Select device: "Other (Custom name)" → Enter "LedgerLink"
   - Click "Generate"
   - Copy the 16-character password (e.g., `abcd efgh ijkl mnop`)

3. **Add to Render Environment Variables**:
   - `EmailSettings__SmtpUsername` = `your_email@gmail.com`
   - `EmailSettings__SmtpPassword` = `abcdefghijklmnop` (remove spaces)

**Note**: App Passwords only work if 2-Step Verification is enabled.

### Step 5: Deploy

1. Click **"Create Web Service"**
2. Render will:
   - Clone your repository
   - Build Docker image
   - Deploy application
   - Assign a URL (e.g., `ledgerlink-app.onrender.com`)

### Step 6: Apply Database Migrations

**Option A: Automatic (Recommended)**

Add this to your Dockerfile before the ENTRYPOINT:

```dockerfile
# Add after COPY --from=publish line
RUN apt-get update && apt-get install -y dotnet-ef

# Before ENTRYPOINT, add:
CMD dotnet ef database update && dotnet LedgerLink.dll
```

**Option B: Manual via Render Shell**

1. Go to your web service → **"Shell"** tab
2. Run:
```bash
cd /app
dotnet ef database update
```

### Step 7: Create First Admin

1. Visit your app URL: `https://your-app.onrender.com`
2. You'll be redirected to admin registration
3. Create your admin account
4. Login and start using!

## 🔧 Configuration Details

### Database Connection

**Format for Render PostgreSQL:**
```
Host=YOUR_INSTANCE.postgres.render.com;
Port=5432;
Database=ledgerlink;
Username=ledgerlink_user;
Password=YOUR_PASSWORD;
SSL Mode=Require
```

**Get from Render:**
- Go to your PostgreSQL database
- Copy "Internal Database URL"
- Convert from URL format to connection string format

**Example conversion:**
```
From: postgres://user:pass@host:5432/dbname
To: Host=host;Port=5432;Database=dbname;Username=user;Password=pass;SSL Mode=Require
```

### Port Configuration

Render automatically sets the PORT environment variable. Your Dockerfile already exposes port 8080, which matches the ASPNETCORE_URLS setting.

### Health Checks

Render will check `http://your-app:8080/` every 30 seconds. Make sure your app responds to the root path.

## 📊 Free Tier Limitations

### Render Free Tier:
- ✅ Free web service (spins down after 15 min of inactivity)
- ✅ 750 hours/month
- ✅ Custom domains with SSL
- ⚠️ Cold starts (takes ~30s to wake up)
- ⚠️ 512MB RAM, 0.1 CPU

### Render PostgreSQL Free Tier:
- ✅ 1GB storage
- ✅ 90 days data retention
- ⚠️ Expires after 90 days (upgrade to paid to keep data)

### Recommendations for Production:
- **Starter Plan ($7/month)**: Always-on, no cold starts
- **PostgreSQL Paid ($7/month)**: Unlimited retention, more storage

## 🔄 Auto-Deploy from Git

Once configured, every `git push` triggers automatic deployment:

```bash
# Make changes
git add .
git commit -m "Update feature"
git push origin main

# Render automatically:
# 1. Detects push
# 2. Builds new Docker image
# 3. Deploys updated app
```

## 🌐 Custom Domain

### Add Custom Domain:

1. Go to web service → **"Settings"** → **"Custom Domains"**
2. Click **"Add Custom Domain"**
3. Enter your domain: `ledgerlink.yourdomain.com`
4. Add DNS records (Render provides them):
   ```
   Type: CNAME
   Name: ledgerlink (or @)
   Value: your-app.onrender.com
   ```
5. SSL certificate is automatic and free!

## 🔐 Environment Variables Best Practices

### Required Variables:
```bash
ConnectionStrings__DefaultConnection   # Database connection
EmailSettings__SmtpHost               # SMTP server (smtp.gmail.com)
EmailSettings__SmtpPort               # SMTP port (587)
EmailSettings__SmtpUsername           # Gmail address
EmailSettings__SmtpPassword           # Gmail App Password
EmailSettings__SenderEmail            # From email address
EmailSettings__SenderName             # From name
```

### Optional but Recommended:
```bash
ShopSettings__ShopName
ShopSettings__ShopEmail
ShopSettings__ShopPhoneNumber
```

### Security Tips:
- ✅ Never commit `.env` file
- ✅ Use Render's secret files for sensitive data
- ✅ Rotate API keys regularly
- ✅ Use different values for dev/staging/prod

## 📝 Monitoring & Logs

### View Logs:
1. Go to web service → **"Logs"** tab
2. Real-time log streaming
3. Search and filter logs

### Useful Log Queries:
```bash
# Errors
error

# Database connections
postgres

# Admin logins
AdminLogin

# Email sending
SMTP
```

### Metrics:
1. Go to web service → **"Metrics"** tab
2. View:
   - CPU usage
   - Memory usage
   - Response times
   - HTTP status codes

## 🐛 Troubleshooting

### Issue: Application won't start

**Check:**
1. Build logs for errors
2. Database connection string is correct
3. All required environment variables are set
4. Port 8080 is exposed in Dockerfile

**Solution:**
```bash
# View logs
# Render Dashboard → Your Service → Logs

# Common fixes:
# 1. Update connection string format
# 2. Add SSL Mode=Require for PostgreSQL
# 3. Check ASPNETCORE_URLS=http://+:8080
```

### Issue: Database connection failed

**Check:**
1. PostgreSQL database is running
2. Connection string uses **Internal** database URL
3. SSL Mode is set to "Require"
4. Database credentials are correct

**Solution:**
```bash
# Test connection string format:
Host=YOUR_HOST.postgres.render.com;
Port=5432;
Database=ledgerlink;
Username=ledgerlink_user;
Password=YOUR_PASSWORD;
SSL Mode=Require
```

### Issue: Cold start slow

**Problem:** Free tier spins down after 15 min

**Solutions:**
1. **Upgrade to Starter plan** ($7/month) - Always on
2. **Keep alive service** - Ping your app every 10 minutes
3. **Accept the delay** - First request takes ~30s

### Issue: 90-day database expiration

**Problem:** Free PostgreSQL expires after 90 days

**Solutions:**
1. **Backup before expiry:**
   ```bash
   # Download backup from Render dashboard
   ```
2. **Upgrade to paid plan** ($7/month)
3. **Migrate to external database** (AWS RDS, etc.)

### Issue: Migrations not applied

**Solution:**
```bash
# Manual migration via Render Shell
cd /app
dotnet ef database update

# Or add to Dockerfile CMD
CMD dotnet ef database update && dotnet LedgerLink.dll
```

## 🔄 Updating Your Application

### Method 1: Git Push (Recommended)
```bash
git add .
git commit -m "Update application"
git push origin main
# Render auto-deploys
```

### Method 2: Manual Deploy
1. Render Dashboard → Your Service
2. Click **"Manual Deploy"** → **"Deploy latest commit"**

### Method 3: Redeploy
1. Click **"Manual Deploy"** → **"Clear build cache & deploy"**
2. Useful for dependency updates

## 📊 Database Backup

### Manual Backup:
1. Render Dashboard → PostgreSQL database
2. Click **"Backups"** tab (paid plans only)
3. Or use pg_dump:

```bash
# From Render Shell
pg_dump $DATABASE_URL > backup.sql

# Or from local machine (use External URL)
pg_dump -h YOUR_HOST.postgres.render.com -U ledgerlink_user -d ledgerlink > backup.sql
```

### Automated Backups:
- Available on paid PostgreSQL plans ($7/month)
- Daily automatic backups
- Point-in-time recovery

## 💰 Cost Estimation

### Free Setup (Development/Testing):
- **Web Service**: Free (with cold starts)
- **PostgreSQL**: Free (90 days)
- **SSL**: Free
- **Custom Domain**: Free
- **Total**: $0/month

### Production Setup:
- **Web Service Starter**: $7/month
- **PostgreSQL Starter**: $7/month
- **SSL**: Free
- **Custom Domain**: Free
- **Total**: $14/month

### Enterprise Setup:
- **Web Service Pro**: $25/month
- **PostgreSQL Standard**: $20/month
- **Total**: $45/month

## 🎯 Complete Render Configuration

### render.yaml (Optional)

Create `render.yaml` in project root for Infrastructure as Code:

```yaml
services:
  - type: web
    name: ledgerlink-app
    env: docker
    dockerfilePath: ./Dockerfile
    healthCheckPath: /
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://+:8080
      - key: ConnectionStrings__DefaultConnection
        fromDatabase:
          name: ledgerlink-db
          property: connectionString
      - key: EmailSettings__SmtpHost
        value: smtp.gmail.com
      - key: EmailSettings__SmtpPort
        value: 587
      - key: EmailSettings__SmtpUsername
        sync: false
      - key: EmailSettings__SmtpPassword
        sync: false
      - key: EmailSettings__SenderEmail
        value: your_email@gmail.com
      - key: ShopSettings__ShopName
        value: Miracle Technoz

databases:
  - name: ledgerlink-db
    databaseName: ledgerlink
    user: ledgerlink_user
    plan: free
```

Deploy with:
```bash
# Commit render.yaml
git add render.yaml
git commit -m "Add Render configuration"
git push origin main

# Render will use this configuration
```

## 📚 Additional Resources

- **Render Docs**: https://render.com/docs
- **Docker Deployment**: https://render.com/docs/docker
- **PostgreSQL**: https://render.com/docs/databases
- **Environment Variables**: https://render.com/docs/environment-variables

## 🎉 Success Checklist

- [ ] GitHub repository created and pushed
- [ ] Render account created
- [ ] PostgreSQL database created on Render
- [ ] Web service created on Render
- [ ] Environment variables configured
- [ ] Database migrations applied
- [ ] First admin account created
- [ ] Application accessible via URL
- [ ] Email functionality tested
- [ ] Custom domain configured (optional)

## 🆘 Support

**Render Support:**
- Community: https://community.render.com
- Discord: https://discord.gg/render
- Email: support@render.com

**Application Issues:**
- Check Render logs
- Review [DOCKER-DEPLOYMENT.md](DOCKER-DEPLOYMENT.md)
- Review [CHANGELOG.md](CHANGELOG.md)

---

**Platform**: Render.com  
**Deployment Type**: Docker  
**Database**: PostgreSQL  
**SSL**: Automatic (Let's Encrypt)  
**Cost**: Free tier available, Production from $14/month
