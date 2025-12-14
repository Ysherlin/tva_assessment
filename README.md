# TVA Assessment API

## 📌 Application Summary

The **TVA Assessment API** is a layered ASP.NET Core Web API that manages:

- **Persons** – Customer identity records  
- **Accounts** – Financial accounts linked to persons  
- **Transactions** – Financial movements on accounts  

The system enforces strict business rules:

- Duplicate persons are not allowed
- Duplicate account numbers are not allowed
- Transactions cannot be posted to closed accounts
- Accounts cannot be closed when the balance is not zero
- Transaction balances automatically update account balances
- All exceptions are centrally handled with consistent API responses

The solution follows **Clean Architecture principles** with clear separation of concerns between:

- API Layer (Controllers)
- Application Layer (DTOs, Services, Interfaces)
- Domain Layer (Entities)
- Infrastructure Layer (EF Core, Repositories)
- Middleware (Exception Handling)
- Test Project (Unit Testing)

---

## 🧱 Architecture Overview

The application follows a **layered architecture**:

- **API Layer** – Exposes REST endpoints
- **Application Layer** – Contains business rules and validation
- **Domain Layer** – Core business entities
- **Infrastructure Layer** – Database and EF Core implementation
- **Middleware** – Centralized exception handling
- **Test Layer** – Isolated unit testing of business logic

### 📐 Architecture Diagram

![Architecture Diagram](documentation/diagrams/architecture.png)

---

## 🧩 Class Diagram

The class diagram illustrates the relationships between:

- Controllers
- Services
- Repositories
- DTOs
- Domain Entities

![Class Diagram](documentation/diagrams/class.png)

---

## 🗄️ Entity Relationship Diagram (ERD)

The ERD shows the database structure and relationships between:

- Person  
- Account  
- AccountStatus  
- Transaction  

![ER Diagram](documentation/diagrams/erd.png)

---

## 🧪 Testing Strategy

Unit testing is implemented using:

- **NUnit 3** – Test framework  
- **Moq** – Dependency mocking  
- **FluentAssertions** – Readable assertions  

Core business logic is verified through:

- `PersonServiceTests`
- `AccountServiceTests`
- `TransactionServiceTests`
- `ExceptionHandlingMiddlewareTests`

All critical business rules are fully validated.

---

## 🔗 API Testing (Postman)

A complete Postman collection and environment are provided for manual API testing.

### 📁 Postman Files

- **Collection:**  
  `documentation/postman/tva_assessment.postman_collection.json`

- **Environment:**  
  `documentation/postman/tva_assessment.postman_environment.json`

### ✅ How to Use Postman

1. Open **Postman**
2. Click **Import**
3. Import both:
   - The **collection**
   - The **environment**
4. Select the environment
5. Start testing the endpoints

---

## 🖥️ Frontend Application (Angular)

In addition to the API, the solution includes a **frontend Angular Single Page Application (SPA)**.

### 🔹 Frontend Features

- Person search with paging
- Create, edit, and delete persons
- View accounts per person
- Create, edit, close, and reopen accounts
- View and create transactions per account
- Navigation between related entities
- UI validation aligned with backend business rules

### 🔹 Frontend Architecture

- Standalone Angular components
- Angular Signals for state management
- Services for API communication
- Typed models aligned with backend DTOs
- Router-based navigation between pages

The frontend consumes the API via HTTP and mirrors backend rules to ensure a consistent user experience.

---

## ⚙️ Technologies Used

### Backend
- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- NUnit 3
- Moq
- FluentAssertions

### Frontend
- Angular
- TypeScript
- Angular Signals
- Angular Router
- Angular Forms
- Angular Material (Date Picker)

### Tooling
- Draw.io (Diagrams)
- Postman (API Testing)

---

## 👤 Developer Information

**Developer:** Ysherlin Govender  
**Cell:** 078 701 2565  
**Email:** ysherlin.govender@gmail.com  

---
