# Docker Deployment Guide

## 📋 Quick Start

All Docker-related files are in this folder for easy organization.

### 1. Setup Environment

The `.env` file has been pre-configured with your Gmail SMTP credentials:

```bash
SMTP_USERNAME=omakapdiya34@gmail.com
SMTP_PASSWORD=kvcztqarmwllfryp
```

**Important**: The `.env` file is git-ignored for security. Never commit credentials to git.

### 2. Build and Start

```bash
# From the docker folder
docker-compose build
docker-compose up -d
```

### 3. Access Application

Open your browser: **http://localhost:8080**

### 4. Create Admin Account

On first visit, you'll be redirected to create an admin account.

## 📁 Files in This Folder

- **docker-compose.yml** - Orchestrates app and PostgreSQL containers
- **Dockerfile** - Multi-stage build for the application
- **.dockerignore** - Files to exclude from Docker build
- **.env** - Environment variables (your SMTP credentials are here)
- **.env.example** - Template for environment variables

## 🔧 Common Commands

**Note**: Run these commands from the `docker` folder.

### Start Services
```bash
docker-compose up -d
```

### Stop Services
```bash
docker-compose down
```

### View Logs
```bash
# All services
docker-compose logs -f

# Just the app
docker-compose logs -f app

# Just database
docker-compose logs -f postgres
```

### Restart Services
```bash
docker-compose restart app
```

### Check Status
```bash
docker-compose ps
```

### Access Database
```bash
docker-compose exec postgres psql -U ledgerlink_user -d LedgerLink
```

### Rebuild After Code Changes
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## 🔐 Email Configuration

Your Gmail SMTP is already configured in `.env`:

- **SMTP Host**: smtp.gmail.com
- **SMTP Port**: 587
- **Username**: omakapdiya34@gmail.com
- **Password**: kvcztqarmwllfryp (App Password)

This will be used for:
- Password reset emails
- Admin notifications

## 🗄️ Database

PostgreSQL 16 is configured in `docker-compose.yml`:

- **Host**: localhost (from host machine) or `postgres` (from app container)
- **Port**: 5432
- **Database**: LedgerLink
- **Username**: ledgerlink_user
- **Password**: Set in `.env` (default: SecurePassword123!)

### Backup Database
```bash
docker-compose exec postgres pg_dump -U ledgerlink_user LedgerLink > backup_$(date +%Y%m%d).sql
```

### Restore Database
```bash
docker-compose exec -T postgres psql -U ledgerlink_user -d LedgerLink < backup.sql
```

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Check what's using port 8080
sudo lsof -i :8080

# Or change port in docker-compose.yml
ports:
  - "8081:8080"  # Use 8081 instead
```

### Database Connection Issues
```bash
# Check postgres is running
docker-compose ps postgres

# Check postgres logs
docker-compose logs postgres

# Restart database
docker-compose restart postgres
```

### Application Won't Start
```bash
# Check app logs
docker-compose logs app

# Common fix: restart app after postgres is fully ready
docker-compose restart app
```

### Clean Everything and Start Fresh
```bash
# WARNING: This deletes all data
docker-compose down -v
docker-compose up -d
```

## 🔄 Updates and Maintenance

### Update Application Code
```bash
# From project root
cd /home/om/dotnet/LedgerLink
git pull

# Rebuild and restart
cd docker
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### View Resource Usage
```bash
docker stats
```

### Clean Up Unused Images
```bash
docker system prune -a
```

## 🌐 Production Deployment

For cloud deployment (Render, Railway, AWS, etc.):

1. Push code to GitHub
2. Use Render's Docker deployment
3. Set environment variables in Render dashboard
4. See [RENDER-DEPLOYMENT.md](../RENDER-DEPLOYMENT.md) for detailed guide

## 📚 Architecture

```
┌─────────────────────────────────────┐
│     Docker Network (ledgerlink)     │
│                                     │
│  ┌──────────────────────────────┐  │
│  │   PostgreSQL 16 Container    │  │
│  │   - Port: 5432               │  │
│  │   - Volume: postgres_data    │  │
│  │   - Health checks enabled    │  │
│  └──────────────────────────────┘  │
│              ↑                       │
│              │ (internal)           │
│              ↓                       │
│  ┌──────────────────────────────┐  │
│  │   LedgerLink App Container   │  │
│  │   - ASP.NET Core 8.0         │  │
│  │   - Port: 8080 → 8080        │◄─┼─ http://localhost:8080
│  │   - Gmail SMTP configured    │  │
│  └──────────────────────────────┘  │
│                                     │
└─────────────────────────────────────┘
```

## 🔒 Security Notes

1. **Never commit `.env`** - It's already in .gitignore
2. **Change DB_PASSWORD** - Use a strong password in production
3. **Use App Passwords** - Gmail App Password, not regular password
4. **HTTPS in Production** - Render/Railway provide automatic SSL
5. **Regular backups** - Backup database regularly

## ✅ Pre-configured Settings

The following are already set up for you:

- ✅ Gmail SMTP with your email (omakapdiya34@gmail.com)
- ✅ App Password configured
- ✅ PostgreSQL database
- ✅ Docker networking
- ✅ Volume persistence for database
- ✅ Health checks
- ✅ Auto-restart policies

You can start using Docker immediately with:

```bash
cd docker
docker-compose up -d
```

Then visit http://localhost:8080 and create your admin account!

---

**Need help?** Check:
- [Main README](../README.md)
- [Render Deployment Guide](../RENDER-DEPLOYMENT.md)
- Docker logs: `docker-compose logs -f`
