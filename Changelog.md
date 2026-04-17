# Changelog

## 2026-04-17

### Task: Price List module

Context:
Added a complete price list system replacing the hardcoded product.Price in order line
creation. Supports customer-specific overrides, classification-based defaults, quantity
tiers, and date-bound validity. PriceResolutionService resolves prices via: customer
assignment → classification list → product.Price fallback with IsBasePriceFallback flag.

Files Affected:
- `src/DMS.Core/PriceLists/PriceList.cs` (new)
- `src/DMS.Core/PriceLists/PriceListItem.cs` (new)
- `src/DMS.Core/PriceLists/PriceListAssignment.cs` (new)
- `src/DMS.Core/PriceLists/PriceResolutionResult.cs` (new)
- `src/DMS.Core/PriceLists/PriceResolutionService.cs` (new)
- `src/DMS.Core/Orders/OrderLine.cs` (IsBasePriceFallback added)
- `src/DMS.Core/Authorization/PermissionNames.cs`
- `src/DMS.Core/Authorization/DMSAuthorizationProvider.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/PriceListConfiguration.cs` (new)
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/PriceListItemConfiguration.cs` (new)
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/PriceListAssignmentConfiguration.cs` (new)
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/OrderLineConfiguration.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/DMSDbContext.cs`
- `src/DMS.Application/PriceLists/` (IPriceListAppService, PriceListAppService, IPriceListAssignmentAppService, PriceListAssignmentAppService, 8 DTOs, MapProfile — all new)
- `src/DMS.Application/Orders/OrderAppService.cs` (PriceResolutionService wired in)
- `src/DMS.Application/Orders/Dto/OrderLineDto.cs` (IsBasePriceFallback added)
- `test/DMS.Tests/PriceLists/PriceList_Tests.cs` (new)

### Feature
- PriceList CRUD with date-bound validity (StartDate, EndDate?) and IsActive toggle
- PriceListItem: quantity-tiered pricing per product (multiple MinQuantity tiers per list)
- PriceListAssignment: customer-specific list override (upsert, unique per customer)
- PriceResolutionService: customer override → classification list → product.Price fallback
- IsBasePriceFallback flag on OrderLine/OrderLineDto when no list matched
- Permissions: Pages.PriceLists (Create, Edit, Delete, Assign)
- 10 integration tests covering resolution hierarchy, tiers, all edge cases — 103 total tests pass

### Migration
- `Added_PriceList_Entities`: creates PriceLists, PriceListItems, PriceListAssignments tables + IsBasePriceFallback column on OrderLines

## 2026-04-17

### Task: Product TaxRate — fix tax bug

Context:
Every order line was created with TaxRate=0 because Product had no TaxRate field.
Added TaxRate to Product entity, DTOs, and EF config. OrderAppService now copies
the product's TaxRate into each order line automatically.

Files Affected:
- `src/DMS.Core/Products/Product.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/ProductConfiguration.cs`
- `src/DMS.Application/Products/Dto/ProductDto.cs`
- `src/DMS.Application/Products/Dto/CreateProductDto.cs`
- `src/DMS.Application/Orders/OrderAppService.cs`
- `src/DMS.EntityFrameworkCore/Migrations/Added_Product_TaxRate.cs` (new)

### Fix
- Added `TaxRate decimal(5,2)` column to Products table (default 0)
- `CreateProductDto` / `UpdateProductDto` now accept `TaxRate` (0–100, validated)
- `ProductDto` exposes `TaxRate`
- `OrderAppService.BuildLinesAsync` copies `product.TaxRate` into each `OrderLine` instead of hardcoding 0
- All 93 tests passing

### Migration
- `Added_Product_TaxRate`: adds nullable-with-default TaxRate column to Products table

### Task: Product AppService tests

Context:
ProductAppService had no test coverage. Added 6 integration tests covering all CRUD paths
plus keyword filter and categoryId filter. All 93 tests pass.

Files Affected:
- `test/DMS.Tests/Products/ProductAppService_Tests.cs` (new)

### Feature
- 6 tests: empty list, create+get (with CategoryName populated), keyword filter, categoryId filter, update, delete

### Task: Category management AppService

Context:
Category entity already existed with EF config but had no service layer. Added full CRUD
AppService with permissions and 6 integration tests. All 87 tests pass.

Files Affected:
- `src/DMS.Application/Categories/ICategoryAppService.cs` (new)
- `src/DMS.Application/Categories/CategoryAppService.cs` (new)
- `src/DMS.Application/Categories/Dto/CategoryDto.cs` (new)
- `src/DMS.Application/Categories/Dto/CreateCategoryDto.cs` (new)
- `src/DMS.Application/Categories/Dto/UpdateCategoryDto.cs` (new)
- `src/DMS.Application/Categories/Dto/PagedCategoryResultRequestDto.cs` (new)
- `src/DMS.Core/Authorization/PermissionNames.cs` (updated)
- `src/DMS.Core/Authorization/DMSAuthorizationProvider.cs` (updated)
- `test/DMS.Tests/Categories/CategoryAppService_Tests.cs` (new)

### Feature
- CategoryAppService: full CRUD (Get, GetAll, Create, Update, Delete)
- GetAll supports keyword filter on Name
- Permissions: Pages.Categories, Pages.Categories.Create, Pages.Categories.Edit, Pages.Categories.Delete
- 6 integration tests: empty list, create+get, keyword filter, update, delete, tenant isolation

## 2026-04-13

### Task: Customer EF Core migration and xUnit tests

Context:
Generated EF Core migration for the Customer entity and wrote 6 xUnit tests covering CRUD operations and keyword filtering. All 6 tests pass.

Files Affected:
- `src/DMS.EntityFrameworkCore/Migrations/20260413083609_Added_Customer_Entity.cs`
- `test/DMS.Tests/Customers/CustomerAppService_Tests.cs`

### Migration
- Added `Added_Customer_Entity` migration creating the `Customers` table with all expected columns, a unique index on `(TenantId, Code)`, and an index on `IsActive`.

### Feature
- Added 6 xUnit tests: `GetAll_Returns_Empty_For_New_Tenant`, `Create_And_Get_Customer`, `GetAll_Filters_By_Keyword_Name`, `GetAll_Filters_By_Keyword_Code`, `Update_Customer`, `Delete_Customer`. All pass.

### Task: Fix redundant TenantId index and document UpdateCustomerDto.IsActive semantics

Context:
Two small cleanup fixes in the Customer module: remove the redundant standalone TenantId index (already covered by the composite unique index prefix) and add an XML doc comment to UpdateCustomerDto.IsActive to make full-replace PUT semantics explicit.

Files Affected:
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/CustomerConfiguration.cs`
- `src/DMS.Application/Customers/Dto/UpdateCustomerDto.cs`

### Fix
- Removed redundant `builder.HasIndex(c => c.TenantId)` from `CustomerConfiguration`; the composite unique index `(TenantId, Code)` already covers TenantId-only seeks via index prefix.

### Improvement
- Added XML doc comment to `UpdateCustomerDto.IsActive` documenting full-replace PUT semantics: client must always send the complete object; `IsActive=false` deactivates the customer.

## 2026-04-13

### Task: Add Company entity with CRUD

Context:
Implement Company as a business entity within a tenant. One tenant can have many companies.
Full CRUD exposed via ABP's auto-controller REST API with permission-based access control and keyword search.

Files Affected:
- API (ABP auto-controller)
- Service (`DMS.Application`)
- Database (`DMS.EntityFrameworkCore`)

### Feature
- Added `Company` entity (`DMS.Core/Companies/Company.cs`) extending `FullAuditedEntity<int>` with `IMustHaveTenant` for automatic tenant isolation and soft-delete. Fields: Name, TaxNumber, Address, Phone, Email, IsActive.
- Added `Pages.Companies`, `Pages.Companies.Create`, `Pages.Companies.Edit`, `Pages.Companies.Delete` permissions to `PermissionNames.cs` and `DMSAuthorizationProvider.cs`.
- Added `CompanyDto`, `CreateCompanyDto`, `UpdateCompanyDto`, `PagedCompanyResultRequestDto`, and `CompanyMapProfile` under `DMS.Application/Companies/Dto/`.
- Implemented `CompanyAppService` extending `AsyncCrudAppService` with keyword filter on Name; auto-exposed as REST API via ABP's `CreateControllersForAppServices`.
- Added `CompanyConfiguration` (Fluent API) and `DbSet<Company>` to `DMSDbContext`.
- Generated EF Core migration `Added_Company_Entity` creating `Companies` table with all columns and indexes on `TenantId` and `IsActive`.
- Added 6 xUnit tests covering GetAll, Create, keyword filter, Update, soft-Delete, and tenant data isolation.

## 2026-04-13

### Task: Fix RouteItem auditing, UpdateRouteItemDto, and redundant TenantId index

Context:
Three targeted fixes in the Route module: upgrade RouteItem to FullAuditedEntity for soft-delete and audit trail support; introduce UpdateRouteItemDto to distinguish create vs update semantics for route items; remove the redundant standalone TenantId index from RouteConfiguration already covered by the composite index prefix.

Files Affected:
- `src/DMS.Core/Routes/RouteItem.cs`
- `src/DMS.Application/Routes/Dto/UpdateRouteItemDto.cs`
- `src/DMS.Application/Routes/Dto/UpdateRouteDto.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/RouteConfiguration.cs`

### Fix
- Changed `RouteItem` to extend `FullAuditedEntity<int>` instead of `Entity<int>`, enabling soft-delete and CreationTime/LastModificationTime audit columns.
- Added `UpdateRouteItemDto` with optional `Id` (null = new item, set = existing item) alongside `CustomerId`, `OrderIndex`, and `PlannedDurationMinutes`.
- Updated `UpdateRouteDto.Items` type from `List<CreateRouteItemDto>` to `List<UpdateRouteItemDto>`.
- Removed redundant `builder.HasIndex(r => r.TenantId)` from `RouteConfiguration`; the composite index `(TenantId, AssignedUserId)` already covers TenantId-only seeks via index prefix.

### Task: Route and RouteItem entities, permissions, DTOs, AppService, EF config

Context:
Implement the Route and RouteItem domain entities with full CRUD app service, permission declarations, DTOs, EF Core configurations, and DbContext registration. ActivateAsync is a placeholder pending the Visit entity (Task 7).

Files Affected:
- `src/DMS.Core/Routes/RouteStatus.cs`
- `src/DMS.Core/Routes/Route.cs`
- `src/DMS.Core/Routes/RouteItem.cs`
- `src/DMS.Core/Authorization/PermissionNames.cs`
- `src/DMS.Core/Authorization/DMSAuthorizationProvider.cs`
- `src/DMS.Application/Routes/Dto/RouteItemDto.cs`
- `src/DMS.Application/Routes/Dto/CreateRouteItemDto.cs`
- `src/DMS.Application/Routes/Dto/RouteDto.cs`
- `src/DMS.Application/Routes/Dto/CreateRouteDto.cs`
- `src/DMS.Application/Routes/Dto/UpdateRouteDto.cs`
- `src/DMS.Application/Routes/Dto/PagedRouteResultRequestDto.cs`
- `src/DMS.Application/Routes/Dto/RouteMapProfile.cs`
- `src/DMS.Application/Routes/IRouteAppService.cs`
- `src/DMS.Application/Routes/RouteAppService.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/RouteConfiguration.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/Configurations/RouteItemConfiguration.cs`
- `src/DMS.EntityFrameworkCore/EntityFrameworkCore/DMSDbContext.cs`

### Feature
- Added `RouteStatus` enum (Draft, Active, Completed) in `DMS.Core/Routes/`.
- Added `Route` entity extending `FullAuditedEntity<int>` with `IMustHaveTenant`; fields: Name, AssignedUserId, PlannedDate, Status, Notes, Items collection.
- Added `RouteItem` entity extending `Entity<int>` with `IMustHaveTenant`; fields: RouteId, CustomerId, OrderIndex, PlannedDurationMinutes.
- Added `Pages.Routes`, `Pages.Routes.Create`, `Pages.Routes.Edit`, `Pages.Routes.Delete` permissions.
- Added full DTO set: `RouteDto`, `CreateRouteDto`, `UpdateRouteDto`, `RouteItemDto`, `CreateRouteItemDto`, `PagedRouteResultRequestDto`, `RouteMapProfile`.
- Implemented `RouteAppService` extending `AsyncCrudAppService` with keyword/userId/status/date-range filtering; `ActivateAsync` is a `NotImplementedException` placeholder until Task 7.
- Added `RouteConfiguration` and `RouteItemConfiguration` (Fluent API) with indexes and cascade-delete; registered in `DMSDbContext`.

### Task: Route EF Core migration and xUnit tests

Context:
Generated EF Core migration for the Route and RouteItem entities and wrote 6 xUnit tests covering CRUD operations, keyword filtering, and assigned-user filtering. All 6 tests pass.

Files Affected:
- `src/DMS.EntityFrameworkCore/Migrations/20260413084801_Added_Route_Entity.cs`
- `test/DMS.Tests/Routes/RouteAppService_Tests.cs`

### Migration
- Added `Added_Route_Entity` migration creating `Routes` table (Id, TenantId, Name, AssignedUserId, PlannedDate, Status, Notes, full audit/soft-delete columns) and `RouteItems` table (Id, TenantId, RouteId, CustomerId, OrderIndex, PlannedDurationMinutes, full audit/soft-delete columns).
- FK from `RouteItems.RouteId` → `Routes.Id` with cascade delete.
- Unique index on `(RouteId, OrderIndex)` in RouteItems; composite index on `(TenantId, AssignedUserId)` and index on `PlannedDate` in Routes.

### Feature
- Added 6 xUnit tests: `GetAll_Returns_Empty_For_New_Tenant`, `Create_And_Get_Route`, `GetAll_Filters_By_Keyword`, `GetAll_Filters_By_AssignedUser`, `Update_Route`, `Delete_Route`. All pass.

### Task: Visit DTOs, IVisitAppService, VisitAppService, and RouteAppService.ActivateAsync

Context:
Implement the full Visit application layer: DTOs for CRUD, check-in/check-out, skip, photo upload, and offline sync; IVisitAppService interface; VisitAppService with geofencing logic and Haversine distance calculation; and a complete RouteAppService.ActivateAsync that creates Planned Visit records for each RouteItem when a route is activated.

Files Affected:
- `src/DMS.Application/Visits/Dto/VisitPhotoDto.cs`
- `src/DMS.Application/Visits/Dto/VisitDto.cs`
- `src/DMS.Application/Visits/Dto/CreateVisitDto.cs`
- `src/DMS.Application/Visits/Dto/UpdateVisitDto.cs`
- `src/DMS.Application/Visits/Dto/PagedVisitResultRequestDto.cs`
- `src/DMS.Application/Visits/Dto/CheckInDto.cs`
- `src/DMS.Application/Visits/Dto/CheckOutDto.cs`
- `src/DMS.Application/Visits/Dto/SkipVisitDto.cs`
- `src/DMS.Application/Visits/Dto/UploadVisitPhotoDto.cs`
- `src/DMS.Application/Visits/Dto/SyncVisitDto.cs`
- `src/DMS.Application/Visits/Dto/SyncVisitResultDto.cs`
- `src/DMS.Application/Visits/Dto/VisitMapProfile.cs`
- `src/DMS.Application/Visits/IVisitAppService.cs`
- `src/DMS.Application/Visits/VisitAppService.cs`
- `src/DMS.Application/Routes/RouteAppService.cs`

### Feature
- Added full DTO set for Visit: `VisitDto`, `VisitPhotoDto`, `CreateVisitDto`, `UpdateVisitDto`, `PagedVisitResultRequestDto`, `CheckInDto`, `CheckOutDto`, `SkipVisitDto`, `UploadVisitPhotoDto`, `SyncVisitDto`, `SyncVisitResultDto`, `VisitMapProfile`.
- Implemented `IVisitAppService` extending `IAsyncCrudAppService` with `CheckInAsync`, `CheckOutAsync`, `SkipAsync`, `UploadPhotoAsync`, `SyncVisitsAsync`.
- Implemented `VisitAppService` with: status-transition enforcement, geofencing check on check-in using Haversine distance + `GpsEnforcement` setting (None/Warn/Block), duration calculation on check-out, disk-based photo upload to `wwwroot/uploads/visits/{tenantId}/{visitId}/`, and offline sync with ExternalId deduplication.
- Implemented `RouteAppService.ActivateAsync`: sets route status to Active and bulk-inserts a `Planned` Visit per RouteItem ordered by `OrderIndex`.

### Task: Visit application layer — VisitAppService, RouteAppService.ActivateAsync, tests

Context:
Implement the Visit application layer (the last P0 MVP module). VisitAppService provides full CRUD plus
GPS-aware check-in/check-out with geofencing, skip-with-reason, Base64 photo upload, and offline sync
with ExternalId deduplication. RouteAppService.ActivateAsync (previously a stub) now transitions a
Draft route to Active and bulk-inserts Planned Visit records for every RouteItem in order.

Files Affected:
- `src/DMS.Application/Visits/Dto/` (11 DTO files)
- `src/DMS.Application/Visits/IVisitAppService.cs`
- `src/DMS.Application/Visits/VisitAppService.cs`
- `src/DMS.Application/Routes/RouteAppService.cs`
- `test/DMS.Tests/Visits/VisitAppService_Tests.cs`

### Feature
- Added `VisitAppService` with `CheckInAsync` (status guard + optional geofencing via Haversine distance), `CheckOutAsync` (duration calculation), `SkipAsync` (requires reason), `UploadPhotoAsync` (Base64 → disk), `SyncVisitsAsync` (offline dedup via ExternalId).
- Implemented `RouteAppService.ActivateAsync`: transitions route Draft→Active, creates one `Planned` Visit per RouteItem ordered by `OrderIndex`.
- Added 8 xUnit tests covering all core visit workflows; all 32 tests in the suite pass.

### Task: Add tenant isolation test to CustomerAppService_Tests

Context:
Add a test that verifies the TenantId is stamped correctly on a newly created Customer and that querying with a different TenantId returns no results, confirming proper tenant isolation at the DB level.

Files Affected:
- `test/DMS.Tests/Customers/CustomerAppService_Tests.cs`

### Feature
- Added `Tenant_Isolation_Customer_TenantId_Stamped_Correctly` test: creates a customer, bypasses EF query filters to read the raw row, asserts `TenantId` matches `AbpSession.TenantId`, and asserts a cross-tenant query returns nothing. All 7 CustomerAppService tests pass.
