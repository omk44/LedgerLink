# LedgerLink - AWS Deployment Guide

Deploy your LedgerLink ASP.NET Core application on AWS using Elastic Beanstalk (easiest) or App Runner.

## 🚀 Deployment Options

| Option | Ease | Cost | Best For |
|--------|------|------|----------|
| **Elastic Beanstalk** | ⭐⭐⭐ (Easy) | $5-15/mo | Quick setup, auto-scaling |
| **App Runner** | ⭐⭐⭐ (Easy) | $7-20/mo | Simple Docker deploys |
| **ECS + RDS** | ⭐⭐ (Moderate) | $10-30/mo | More control, containers |
| **EC2 + RDS** | ⭐ (Complex) | $5-30/mo | Manual, full control |

**Recommended for beginners: Elastic Beanstalk**

---

## 📋 Option 1: AWS Elastic Beanstalk (Easiest)

### Prerequisites
- AWS account (logged in)
- EB CLI installed: `pip install awsebcli`
- Git repository pushed to GitHub

### Step 1: Create RDS PostgreSQL Database

1. Go to **AWS Console** → **RDS** → **Create Database**
2. Configure:
   - **Engine**: PostgreSQL 15
   - **DB Instance Class**: `db.t3.micro` (free tier eligible)
   - **Allocated Storage**: 20 GB
   - **DB Name**: `ledgerlink`
   - **Master Username**: `postgres`
   - **Master Password**: (create strong password)
   - **Public Accessibility**: Yes
   - **Backup Retention Period**: 7 days (free tier)
   - **VPC**: Default
   - **Database Port**: 5432

3. Click **Create Database** and wait (5-10 min)
4. Copy the **Endpoint** (e.g., `ledgerlink-db.xxxxx.us-east-1.rds.amazonaws.com`)

### Step 2: Create Security Group for RDS

1. Go to **RDS** → Your database → **VPC Security Groups**
2. Click the security group
3. Go to **Inbound Rules** → **Edit Inbound Rules**
4. **Add Rule**:
   - Type: PostgreSQL
   - Protocol: TCP
   - Port: 5432
   - Source: 0.0.0.0/0 (or your IP for security)
5. **Save Rules**

### Step 3: Initialize Elastic Beanstalk

```bash
cd /home/om/dotnet/LedgerLink

# Initialize EB app
eb init -p "Docker running on 64bit Amazon Linux 2" --region us-east-1

# Create environment
eb create ledgerlink-env --envvars ASPNETCORE_ENVIRONMENT=Production
```

### Step 4: Configure Environment Variables in EB

```bash
# Set environment variables
eb setenv \
  ConnectionStrings__DefaultConnection="Host=YOUR_RDS_ENDPOINT;Port=5432;Database=ledgerlink;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true" \
  ASPNETCORE_ENVIRONMENT=Production \
  ASPNETCORE_URLS=http://+:80 \
  ShopSettings__ShopName="Miracle Technoz" \
  ShopSettings__ShopEmail="info@miracletechnoz.com" \
  ShopSettings__ShopPhoneNumber="+919876543210" \
  EmailSettings__SmtpHost=smtp.gmail.com \
  EmailSettings__SmtpPort=587 \
  EmailSettings__SmtpUsername=your-gmail@gmail.com \
  EmailSettings__SmtpPassword=your-gmail-app-password
```

### Step 5: Deploy

```bash
# First deployment
eb deploy

# View logs
eb logs

# Open in browser
eb open
```

### Step 6: Apply Database Migrations

```bash
# SSH into EB instance and run migrations
eb ssh

# Inside EB instance:
cd /var/app/current
export ConnectionStrings__DefaultConnection="Host=..."  # Copy from EB env vars
dotnet ef database update
exit
```

---

## 📋 Option 2: AWS App Runner (Simplest)

App Runner auto-builds from GitHub without needing EB CLI.

### Step 1: Create RDS Database (Same as Above)

### Step 2: Push to GitHub

```bash
git push origin main
```

### Step 3: Create App Runner Service

1. Go to **AWS Console** → **App Runner** → **Create Service**
2. **Source**:
   - Source: GitHub
   - Connect to GitHub (authorize)
   - Repository: Select your LedgerLink repo
   - Branch: `main`
3. **Build Settings**:
   - Runtime: Docker
   - Dockerfile Path: `docker/Dockerfile`
4. **Service Name**: `ledgerlink`
5. **Resource Configuration**:
   - CPU: 0.25 vCPU
   - Memory: 512 MB
   - Instance Count: 1
6. **Environment Variables**:
   ```
   ConnectionStrings__DefaultConnection=Host=YOUR_RDS_ENDPOINT;Port=5432;Database=ledgerlink;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:8080
   EmailSettings__SmtpHost=smtp.gmail.com
   EmailSettings__SmtpPort=587
   EmailSettings__SmtpUsername=your-gmail@gmail.com
   EmailSettings__SmtpPassword=your-gmail-app-password
   ```
7. Click **Create & Deploy**

### Step 4: Apply Migrations

```bash
# SSH via AWS Systems Manager
# Or run locally before deployment:
cd /home/om/dotnet/LedgerLink
dotnet ef database update --connection "Host=YOUR_RDS_ENDPOINT;Port=5432;Database=ledgerlink;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
```

---

## 🔗 Connection String Format

Replace placeholders with actual values from RDS:

```
Host=ledgerlink-db.xxxxx.us-east-1.rds.amazonaws.com;Port=5432;Database=ledgerlink;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

**Example**:
```
Host=ledgerlink-db.c9akcaq32.us-east-1.rds.amazonaws.com;Port=5432;Database=ledgerlink;Username=postgres;Password=MySecure123Password;SSL Mode=Require;Trust Server Certificate=true
```

---

## 💰 Estimated AWS Costs

| Service | Free Tier | After Free Tier |
|---------|-----------|-----------------|
| **RDS PostgreSQL** | 750 hrs/month × 1 yr | ~$15–25/mo |
| **Elastic Beanstalk** | Included (EC2 charges apply) | ~$8–15/mo |
| **App Runner** | 125,000 requests/day | ~$7–15/mo |
| **Data Transfer** | 100 GB/month × 12 mo | ~$0.09 per GB after |

**Total (Year 1 free tier)**: $0 (if within limits)  
**Total (After free tier)**: $30–50/mo depending on traffic

---

## 🔧 Post-Deployment

### Test Your App

```bash
# Get the App Runner/EB URL from AWS Console
# Example: https://ledgerlink.xxxxx.awsapprunner.com

# Visit: https://your-app-url
# You should see the login page in English
# Click language selector to test Hindi/Gujarati
```

### Monitor Logs

**Elastic Beanstalk**:
```bash
eb logs --all
```

**App Runner**:
- Go to AWS Console → App Runner → Your Service → Logs

### Update and Redeploy

```bash
# Make changes locally
git add .
git commit -m "Update feature"
git push origin main

# App Runner auto-deploys
# For Elastic Beanstalk:
eb deploy
```

---

## ⚠️ Important Notes

1. **SSL/HTTPS**: AWS automatically provides HTTPS via CloudFront/ALB
2. **Health Check**: Make sure your app responds to `/health` endpoint (already configured)
3. **Migrations**: Run once before or after first deployment
4. **Email**: Requires Gmail App Password (not regular password)
5. **Free Tier**: RDS free tier only lasts 12 months from account creation

---

## 🚨 Troubleshooting

### "Connection refused" on RDS
- Check security group allows your EB/App Runner instance
- Verify RDS endpoint is correct
- Ensure SSL Mode settings match

### "Health check failed"
- Check if `/health` endpoint is responding
- View logs: `eb logs` or App Runner Logs
- Ensure `ASPNETCORE_URLS=http://+:80` or `:8080`

### "Cannot connect to database after migration"
- Verify ConnectionStrings__DefaultConnection env variable is set
- Check log output for SQL connection errors
- Try connecting with `psql` locally first

---

## 📚 AWS Resources

- [Elastic Beanstalk Docs](https://docs.aws.amazon.com/elasticbeanstalk/)
- [App Runner Docs](https://docs.aws.amazon.com/apprunner/)
- [RDS PostgreSQL Docs](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/USER_PostgreSQL.html)
- [EB CLI Docs](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/eb-cli3.html)
