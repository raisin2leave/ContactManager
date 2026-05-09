# Contact Manager API

## Author
- Ivanna Fedoseenko
- 15636

---

# Project Description

Contact Manager API is a CRM-style backend application built with ASP.NET Core using Clean Architecture principles.

The application allows authenticated users to manage contacts and notes, while administrators can manage users, roles, and permissions.

---

# Architecture

The solution follows Clean Architecture and is divided into layers:

- AppCore
- Infrastructure
- WebAPI
- UnitTests

---

# Implemented Features

## Authentication & Authorization
- JWT authentication
- Refresh tokens
- ASP.NET Identity
- Role-based authorization
- Admin-only endpoints

---

## User Management (Admin API)
- Get all users
- Block / unblock users
- Activate / deactivate users
- Assign / remove roles

---

## Contacts Module
- Create contacts
- Update contacts
- Delete contacts
- Get contact by ID
- Pagination support
- Ownership rules:
  - only creator or administrator can modify/delete

---

## Notes Module
- Add notes to contacts
- Get notes for contact
- Delete notes

---

## Value Object
### EmailAddress
- validation
- parsing
- formatting

---

# Database
- Entity Framework Core
- SQLite
- Code-first migrations

---

# Seeder
The project contains seeders for:
- roles
- administrator account
- sample contacts
- sample users

---

# Testing

## Unit Tests
- EmailAddress validation
- EmailAddress parsing
- Domain extraction
- Repository logic

---

# Test Credentials

## Administrator
Email:
```text
admin@crm.pl
```

Password:
```text
Admin@123!
```

---

# Tech Stack
- ASP.NET Core 8 Web API
- Entity Framework Core
- SQLite
- ASP.NET Identity
- JWT Authentication
- AutoMapper
- xUnit

---

# Running the Project

## Apply migrations
```bash
dotnet ef database update --project Infrastructure --startup-project WebAPI
```

## Run API
```bash
dotnet run --project WebAPI
```

Default API URL:
```text
http://localhost:5022
```

---

# Running Tests

```bash
dotnet test
```

---

# Repository

```text
https://github.com/raisin2leave/ContactManager.git
```
