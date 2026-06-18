# Learning AWS: LedgerLink Deployment Architecture

To understand how your app is running on AWS, let's break down the services we chose, why we chose them, and how they compare to other AWS services.

---

## 1. The Core Services We Used

### 🅰️ AWS Elastic Beanstalk (PaaS - Platform as a Service)
* **What it is:** A service that automatically handles the deployment, provisioning, load balancing, auto-scaling, and health monitoring of your application.
* **Why we chose it:** Instead of manually setting up a Linux server, installing Docker, configuring Nginx, and setting up firewalls, Elastic Beanstalk does it for you. 
* **The "Docker" platform choice:** By choosing Docker, we made our deployment platform-independent. Your .NET application runs inside a lightweight container, making it easy to migrate, update, or run locally.

### 🅱️ AWS RDS (Managed Database Service)
* **What it is:** A service dedicated to running databases (like PostgreSQL, MySQL, SQL Server) in a managed environment.
* **Why we chose it:** You should **never** run your database on the same server as your application in production. If your app crash-loops or the server runs out of memory, your database could get corrupted. RDS handles automatic backups, updates, and keeps the database isolated and secure.

### 🅲 AWS EC2 (Virtual Servers)
* **What it is:** Raw virtual machines in the cloud (Elastic Compute Cloud).
* **Why we chose `t3.micro`:** In AWS Mumbai (`ap-south-1`), `t3.micro` is the standard modern instance type eligible for the **Free Tier**. Using `t2.micro` failed because it is an older generation and no longer covered as free under new AWS accounts in this region.
* **Note:** Elastic Beanstalk automatically created and managed this EC2 instance for us.

---

## 2. Why Not Other AWS Options?

AWS has over 200 services. Here is how our choice compares to the alternatives you might hear about:

| Service Type | Setup Style | Maintenance Effort | Best Use Case |
| :--- | :--- | :--- | :--- |
| **EC2 (IaaS)** | Manual | High | Complete custom control over OS and software |
| **Elastic Beanstalk (PaaS)** | Automated | Low | Standard web applications / Docker containers |
| **Lambda (Serverless)** | No Servers | Very Low | Event-driven functions or lightweight REST APIs |

### 1. Why not raw EC2? (Infrastructure as a Service)
If we chose raw EC2, we would have had to:
1. Rent a blank Linux VM.
2. Manually install SSH keys, Docker, Nginx.
3. Deal with reverse proxies, SSL certificates, and firewall ports.
4. Manually update the server when OS vulnerabilities are found.
* **Verdict:** Good for learning basic Linux administration, but bad for modern production workflows where you want to focus on code.

### 2. Why not AWS Lambda? (Serverless)
AWS Lambda runs your code only when a request comes in (event-driven). 
* **Verdict:** While cheap, .NET MVC applications are stateful, run continuously, and suffer from "cold starts" (delay when starting up from zero). Lambda is better suited for lightweight REST APIs or background tasks.

### 3. Why not AWS ECS / EKS? (Containers / Kubernetes)
ECS (Elastic Container Service) and EKS (Elastic Kubernetes Service) are designed for microservices (running 10+ different containers that talk to each other).
* **Verdict:** Too complex for a single-container MVC application. It requires learning Task Definitions, Target Groups, Application Load Balancers, and complex networking.

---

## 3. How the Traffic Flows (The Lifecycle)

When a user visits your live link, here is what happens:

```
 ┌──────────────┐      DNS       ┌────────────────────────────────┐
 │ User Browser ├───────────────►│     Elastic Beanstalk URL      │
 └──────────────┘                └──────────────┬─────────────────┘
                                                │
                                                ▼ (HTTP Port 80)
 ┌────────────────────────────────────────────────────────────────┐
 │ AWS EC2 Instance (Mumbai)                                      │
 │  ┌─────────────────┐             ┌──────────────────────────┐  │
 │  │  Nginx Server   ├────────────►│ Docker Container         │  │
 │  │  (Reverse Proxy)│             │ (Runs .NET 8.0 on 8080)  │  │
 │  └─────────────────┘             └───────────┬──────────────┘  │
 └──────────────────────────────────────────────┼─────────────────┘
                                                │
                                                ▼ (PostgreSQL Port 5432)
                                 ┌──────────────┴─────────────────┐
                                 │       AWS RDS PostgreSQL       │
                                 └────────────────────────────────┘
```

1. **The Request:** The user hits `http://ledgerlink-live...`
2. **The Route:** AWS routes the request to your **EC2 Instance**.
3. **The Proxy:** Inside the EC2 instance, a web server called **Nginx** receives the request and forwards it to your **Docker Container** running on port 8080.
4. **The App & DB:** The .NET app processes the request, talks to **RDS PostgreSQL** on port 5432 to save or fetch data, and returns the HTML to the user.
