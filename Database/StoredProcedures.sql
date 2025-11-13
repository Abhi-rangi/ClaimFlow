-- Stored Procedures for Claims Management Platform
-- SQL Server Database

USE ClaimsManagementDB;
GO

-- Stored Procedure: Get Claims by Status with Summary
CREATE OR ALTER PROCEDURE sp_GetClaimsByStatusWithSummary
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.Id,
        c.ClaimNumber,
        c.ClaimantName,
        c.ClaimantEmail,
        c.ClaimantPhone,
        c.ClaimAmount,
        c.IncidentDate,
        c.FiledDate,
        c.Status,
        c.Description,
        c.Notes,
        c.CreatedAt,
        c.UpdatedAt
    FROM Claims c
    WHERE c.Status = @Status
    ORDER BY c.FiledDate DESC;
    
    -- Return summary statistics
    SELECT 
        @Status AS Status,
        COUNT(*) AS TotalClaims,
        SUM(ClaimAmount) AS TotalAmount,
        AVG(ClaimAmount) AS AverageAmount
    FROM Claims
    WHERE Status = @Status;
END
GO

-- Stored Procedure: Get Claims Summary by Date Range
CREATE OR ALTER PROCEDURE sp_GetClaimsSummaryByDateRange
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Status,
        COUNT(*) AS ClaimCount,
        SUM(ClaimAmount) AS TotalAmount,
        AVG(ClaimAmount) AS AverageAmount,
        MIN(ClaimAmount) AS MinAmount,
        MAX(ClaimAmount) AS MaxAmount
    FROM Claims
    WHERE FiledDate BETWEEN @StartDate AND @EndDate
    GROUP BY Status
    ORDER BY Status;
END
GO

-- Stored Procedure: Update Claim Status
CREATE OR ALTER PROCEDURE sp_UpdateClaimStatus
    @ClaimId INT,
    @NewStatus NVARCHAR(50),
    @Notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Claims
    SET 
        Status = @NewStatus,
        Notes = ISNULL(@Notes, Notes),
        UpdatedAt = GETUTCDATE()
    WHERE Id = @ClaimId;
    
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'Claim status updated successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'Claim not found' AS Message;
END
GO

-- Stored Procedure: Get High Value Claims
CREATE OR ALTER PROCEDURE sp_GetHighValueClaims
    @Threshold DECIMAL(18,2) = 10000.00
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.Id,
        c.ClaimNumber,
        c.ClaimantName,
        c.ClaimAmount,
        c.Status,
        c.FiledDate,
        DATEDIFF(DAY, c.FiledDate, GETUTCDATE()) AS DaysSinceFiled
    FROM Claims c
    WHERE c.ClaimAmount >= @Threshold
    ORDER BY c.ClaimAmount DESC;
END
GO

