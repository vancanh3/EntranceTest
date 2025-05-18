## 🚀 **Setup and Run**
### **Clone the repository**
```bash
git clone https://github.com/vancanh3/EntranceTest.git
cd your-repo-name

### **Configure the database connection string**
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=YourDatabase;User Id=sa;Password=your_password;"
}

### **Apply migration**
dotnet ef migrations add MigrationName --project AuthenticationAPI
dotnet ef database update --project AuthenticationAPI
