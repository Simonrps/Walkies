# Walkies 🐾

**An application to connect dog owners and dog walkers.**

Walkies is a cross-platform mobile application developed as a final year project for the 
Bachelor of Science in Applied Computing at ATU Donegal. The application allows dog owners 
to connect with local dog walkers, book walking services, and track their dog's walk in 
real time — providing a more affordable and flexible alternative to traditional dog day care.

---

## Author
- **Name:** Simon Mulroy
- **Student Number:** L00196459
- **Supervisor:** Lusungu Mwasinga / Pauric Dawson
- **Module:** PROJ_IT805 - LY_ICSWD_B: Project (2025/2026)

---

## Technology Stack

| Layer | Technology |
|---|---|
| Frontend | .NET MAUI (C#, XAML) |
| Architecture | MVVM (Model-View-ViewModel) |
| Backend | ASP.NET Core Web API |
| Database | SQL Server / LocalDB |
| ORM | Entity Framework Core |
| Authentication | JWT (JSON Web Tokens) |
| Testing | xUnit.v3, Moq |
| CI/CD | GitHub Actions |

---

## Prerequisites

To run this project you will need the following installed:

- Visual Studio 2026
- .NET 10.0 SDK
- SQL Server Express or LocalDB (included with Visual Studio)
- Android Emulator (for mobile testing) or Windows Machine target

---

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/Simonrps/Walkies.git
cd Walkies
```

### 2. Set Up the Database
The database is created automatically using Entity Framework Core migrations.

- Open the solution in Visual Studio
- Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
- Set the default project to **Walkies.API**
- Run the following command:
```bash
Update-Database
```

This will create all the required database tables automatically.

### 3. Configure the API URL
- Open **Walkies.MAUI → Services → ApiService.cs**
- Update the base URL to match your local machine if needed:
```csharp
private const string BaseUrl = "http://localhost:5000/api/";
```

### 4. Run the Application
- Right-click **Walkies.API** → Set as Startup Project → Press **F5**
- Once the API is running, right-click **Walkies.MAUI** → Set as Startup Project
- Select **Windows Machine** as the target platform
- Press **F5**

---

## Project Structure
```
Walkies.sln
├── Walkies.API                  # ASP.NET Core Web API
│   ├── Controllers              # API endpoints
│   ├── Data                     # DbContext and migrations
│   ├── DTOs                     # Data Transfer Objects
│   ├── Models                   # Database entity classes
│   ├── Repositories             # Data access layer
│   └── Services                 # Business logic layer
│
├── Walkies.MAUI                 # .NET MAUI Frontend
│   ├── Models                   # Client-side models
│   ├── Pages                    # XAML pages
│   ├── PageModels               # Page-level ViewModels
│   ├── Services                 # API communication layer
│   ├── ViewModels               # MVVM ViewModels
│   └── Views                    # Additional XAML views
│
└── Walkies.Tests                # xUnit test project
    └── (Unit tests for API and ViewModels)
```

---

## Running the Tests

- Open **Test Explorer** (Test → Test Explorer)
- Click **Run All Tests**
- All tests should pass with a green tick ✅

---

## CI/CD Pipeline

This project uses **GitHub Actions** for continuous integration. The pipeline runs automatically on every push and pull request to the master branch. It performs the following steps:

1. Restore NuGet dependencies
2. Build the API project
3. Run all unit tests

Pipeline status is visible in the **Actions** tab of this repository.

---

## Features

- 🔐 Secure registration and login with JWT authentication
- 👤 Role-based access for Dog Owners and Dog Walkers
- 🐶 Dog profile management for owners
- 📅 Walk request posting and booking workflow
- 📍 Geo-location search for nearby walkers
- 🗺️ Real-time GPS tracking during active walks
- 💬 In-app messaging between owners and walkers
- 📆 Availability scheduling for walkers
- 💳 Simulated payment confirmation

---

## License

This project was developed for academic purposes at ATU Donegal.
