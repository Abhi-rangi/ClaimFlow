# Database Setup

This folder contains SQL Server database scripts and stored procedures for the Claims Management Platform.

## Setup Instructions

1. **Create the Database** (if not using Entity Framework migrations):
   ```sql
   CREATE DATABASE ClaimsManagementDB;
   GO
   ```

2. **Run Entity Framework Migrations** (Recommended):
   ```bash
   cd Claims.API
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Create Stored Procedures**:
   Execute the `StoredProcedures.sql` file in SQL Server Management Studio or Azure Data Studio:
   ```bash
   sqlcmd -S localhost -d ClaimsManagementDB -i StoredProcedures.sql
   ```

## Stored Procedures

- `sp_GetClaimsByStatusWithSummary` - Get claims by status with summary statistics
- `sp_GetClaimsSummaryByDateRange` - Get claims summary grouped by status within a date range
- `sp_UpdateClaimStatus` - Update claim status with optional notes
- `sp_GetHighValueClaims` - Get claims above a specified threshold amount

## Connection String

Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClaimsManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server Express:
```
Server=localhost\\SQLEXPRESS;Database=ClaimsManagementDB;Trusted_Connection=True;TrustServerCertificate=True;
```

