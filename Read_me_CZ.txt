# LunchSystem

Jednoduchý systém pro správu školních obědů postavený na **ASP.NET Core**.  
Projekt vznikl jako studijní aplikace pro procvičení **Identity, MVC, Repository patternu a čisté architektury**.

---

## Funkce

- **Správa uživatelů** (Identity, role: MasterKuchařka, Kuchařka, Admin, běžný uživatel školy, senior, externí zákazník)
- **CRUD jídel** a **nastavení týdenního menu**
- **Výběr menu uživatelem**
- **Kreditní systém** a podpora **slev** (např. senioři, žáci/učitelé školy)
- **Evidence odepsaných obědů** (přes čip/kartu)
- **Historie a pagination** – přehled pípnutí (odepsaných obědů) po stránkách
- Základní **dashboard** s přehledem jídel a objednávek
- Architektura inspirovaná **Clean Architecture**

---

Struktura řešení

## Projekt je kombinací více částí:

- **LunchWeb** – webová aplikace (ASP.NET Core MVC)
- **LunchApi** – Web API pro komunikaci s kioskem/čtečkou
- **LunchKiosk (WinForms)** – jednoduchá desktopová aplikace pro pípnutí čipem
- **Domain** – entity, Enumy
- **Application** – business logika, služby
- **Infrastructure** – implementace repository, databázový přístup (EF Core)
- **Shared** – společná knihovna (DTO, konstanty, výjimky)

---

## Použité technologie

- **.NET 8 / ASP.NET Core MVC**
- **Entity Framework Core** (SQL Server)
- **ASP.NET Identity** (autentizace a role)
- **AutoMapper** (mapování mezi entitami a ViewModelem)
- **Bootstrap / Bootswatch** (UI stylování)
- **Repository & Services pattern**

---

## Spuštění projektu

1. Naklonujte repozitář:
   git clone https://github.com/tvoje-jmeno/LunchSystem.git
   Nastavte vlastní ConnectionString v souboru appsettings.json.
   Spusťte migraci a vytvořte databázi
