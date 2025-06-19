## Connect with Me

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Profile-blue)](https://www.linkedin.com/in/spyros-ponaris-913a6937/)

# 🧱 WPF Clean Architecture - Master Detail App

This project is a modern WPF desktop application using **Clean Architecture** principles. It showcases a basic **Customer–Order** master-detail interface with proper layering, validation, and MVVM patterns.

---

## ✨ Features

- ✅ 4-Tier Clean Architecture:
  - **Domain**: Entities, Interfaces
  - **Application**: DTOs, Services, Validators
  - **Infrastructure**: EF Core (SQLite), Mappings, DbContext
  - **UI**: WPF (MVVM), CommunityToolkit.Mvvm

- 🧩 Master-Detail pattern: Customers and their Orders
- 📦 SQLite with `DbContextFactory` (safe for DI + design-time)
- 🧪 FluentValidation for input rules
- 💡 RelayCommand + ObservableObject via CommunityToolkit
- 🪟 Dialogs for Add/Edit Customers

---

## 🛠️ Tech Stack

- [.NET 9.0](https://dotnet.microsoft.com/)
- WPF (.NET Desktop)
- Entity Framework Core
- SQLite
- CommunityToolkit.Mvvm
- FluentValidation

---

## 🚀 Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/your-username/WpfAppCleanArchitecture.git
cd WpfAppCleanArchitecture
---
2. Run EF Core migration (once)
---
bash

dotnet ef migrations add InitialCreate -s WpfAppCleanArchitecture
dotnet ef database update -s WpfAppCleanArchitecture
If dotnet ef is not recognized, run: dotnet tool install --global dotnet-ef

---
3. Run the app
---
dotnet run --project WpfAppCleanArchitecture
📂 Project Structure

WpfAppCleanArchitecture/
│
├── Domain/                  # Entities, Interfaces
├── Application/             # DTOs, Services, Validators
├── Infrastructure/          # EF Core, SQLite, Mappings
├── UI/                      # WPF views, ViewModels
│   ├── Views/
│   └── ViewModels/
├── Dialogs/                 # WPF modal dialogs
├── appsettings.json         # Connection string
└── Program.cs               # Host builder, DI setup
✍️ Example: Add Customer
Opens a dialog with CustomerDialogViewModel

Validates Name field (required, length)

Saves to database and updates ObservableCollection
