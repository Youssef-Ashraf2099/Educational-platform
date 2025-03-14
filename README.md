# Project Overview
In this project, we design and implement a database schema for a gamified and personalized educational platform. The goal is to support a dynamic and adaptable learning environment that personalizes content delivery and interactions based on learners’ unique characteristics, such as personality, emotional state, physical and mental health, and background. The platform also aims to enhance engagement through gamification elements like badges, quests, and leaderboards. The platform’s core focus is to provide a scientifically grounded and pedagogically sound learning experience, aligning with the principles of adaptive learning and character computing. By combining learning theory with data-driven personalization, the database will act as the backbone, managing learners’ data, course content, learning interactions, and gamification features, while enabling fine-grained analytics and data tracking.

## Setting up the Project on Local PC

### Step 1: Install ASP.NET 9
To run this project, you need to have ASP.NET 9 installed on your local machine. You can download it from the official Microsoft website and follow the installation instructions.

### Step 2: Setup Database on Local PC
1. Install SQL Server on your local machine.
2. Create a new database named `CourseStation`.

### Step 3: Change Configuration Files
1. Update the `appsettings.json` file in the `Course station/Course station/` directory with your local database server details:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CourseStation;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```
2. Update the `ApplicationDbContext.cs` file in the `Course station/Course station/Models/` directory to use the connection string from the configuration file:
    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
    ```

### Step 4: Install Necessary Dependencies
Run the following commands to install the required dependencies:

```sh
dotnet add package System.Net.Http
dotnet add package Newtonsoft.Json
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Session
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
 ```
### Step 5:Transform sql code to C# Models
```
dotnet ef dbcontext scaffold "YourConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models
```
### Step 6: Build and Run the project and ENJOOOY

```
dotnet restore
dotnet build

dotnet run
 ```
## in case if you want to update any model

```
dotnet ef migrations add <MigrationName>
 # Make sure to replace the `<MigrationName>` placeholder with a meaningful name when you run the command or according to the file you are updating.
dotnet ef database update
```
