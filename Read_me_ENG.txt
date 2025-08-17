# LunchSystem

A simple school lunch management system built with **ASP.NET Core**.  
The project was created as a study application to practice **Identity, MVC, Repository pattern, and Clean Architecture**.

---

## Features

- **User management** (Identity, roles: MasterCook, Cook, Admin, regular school user, senior, external customer)
- **CRUD operations for meals** and **weekly menu management**
- **Menu selection by user**
- **Credit system** with support for **discounts** (e.g., seniors, school students/teachers)
- **Tracking of written-off lunches** (via chip/card scanning)
- **History with pagination** – overview of scanned (written-off) lunches per page
- Basic **dashboard** with meals and orders overview
- Architecture inspired by **Clean Architecture**

---

## Solution structure

The solution is a combination of several projects:

- **LunchWeb** – web application (ASP.NET Core MVC)
- **LunchApi** – Web API for kiosk/card reader communication
- **LunchKiosk (WinForms)** – simple desktop app for chip scanning
- **Domain** – entities, enums
- **Application** – business logic, services
- **Infrastructure** – repository implementations, database access (EF Core)
- **Shared** – shared library (DTOs, constants, exceptions)

---

## Technologies used

- **.NET 8 / ASP.NET Core MVC**
- **Entity Framework Core** (SQL Server)
- **ASP.NET Identity** (authentication and roles)
- **AutoMapper** (mapping between entities and ViewModels)
- **Bootstrap / Bootswatch** (UI styling)
- **Repository & Services pattern**

---

## Running the project

1. Clone the repository:
   git clone https://github.com/your-username/LunchSystem.git
   Set your own ConnectionString in the appsettings.json file.
   Apply migrations and create the database