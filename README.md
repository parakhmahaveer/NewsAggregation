# 📰 News Aggregation System

A full-stack console-based News Aggregation System that fetches articles from external APIs, allows user interaction, and includes admin control functionalities. Built with ASP.NET Core Web API and a C# Console Client.

## 📦 Features

### 🔐 Authentication
- **Register/Login** with role-based access (`User`, `Admin`)
- **JWT Authentication** with token renewal handling

### 📰 User Functionality
- **View Headlines** (by category, keywords, or all)
- **Save/Unsave Articles**
- **React (Like/Dislike)** to Articles
- **Report Articles** (manually or via abuse threshold)
- **Notification Preferences**
  - Enable/disable notifications by category
  - Enable/disable notifications by keyword
- **Keyword Management**
  - Add keywords for personalized news
  - Keywords stored and configurable

### 🛠️ Admin Functionality
- **Manage Categories & Keywords**
  - Add categories, assign keywords
  - Soft-delete category/keyword
- **Moderate Articles**
  - View reported articles
  - Manually hide/unhide articles
  - Auto-hide based on report threshold
- **Block Articles**
  - Block based on specific keywords or categories

## 📂 Project Structure

### Server-Side (ASP.NET Core Web API)
- `Controllers/` – Auth, User, Admin, Article, Notification
- `Services/` – Business logic
- `Repositories/` – Data access logic
- `Models/` – DTOs and Entities
- `Authentication/` – JWT logic
- `Data/` – EF Core DbContext and Migrations

### Client-Side (C# Console App)
- `Services/` – Handle login, user/admin operations
- `Menus/` – Role-based CLI menu
- `Models/` – Request/Response DTOs
- `Helpers/` – Token management, input utilities

## ⚙️ Technologies

- ASP.NET Core Web API (.NET 6+)
- Entity Framework Core (Code First)
- C# Console Application
- JWT for Authentication
- SQL Server (or InMemory DB for testing)
- RESTful API structure

## 🚀 How to Run

### Prerequisites
- [.NET 6+ SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server (optional for persistent storage)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-repo/news-aggregator.git

2. **Server Setup**
cd NewsAggregation.API
dotnet ef database update
dotnet run

3. **Client Setup**
cd NewsAggregation.ConsoleClient
dotnet run

Author
Mahaveer Parakh

Acknowledgment
Jitendra
Shivam
Prem



