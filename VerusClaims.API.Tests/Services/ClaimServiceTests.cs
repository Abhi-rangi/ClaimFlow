using Microsoft.EntityFrameworkCore;
using VerusClaims.API.Data;
using VerusClaims.API.Models;
using VerusClaims.API.Services;
using Xunit;

namespace VerusClaims.API.Tests.Services;

public class ClaimServiceTests
{
    private VerusClaimsDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<VerusClaimsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new VerusClaimsDbContext(options);
    }

    [Fact]
    public async Task CreateClaimAsync_ShouldCreateClaim_WithValidData()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);
        var claim = new Claim
        {
            ClaimNumber = "CLM-20240101-1234",
            ClaimantName = "John Doe",
            ClaimantEmail = "john@example.com",
            ClaimantPhone = "555-1234",
            ClaimAmount = 5000.00m,
            IncidentDate = DateTime.UtcNow.AddDays(-30),
            FiledDate = DateTime.UtcNow,
            Status = "Pending",
            Description = "Test claim"
        };

        // Act
        var result = await service.CreateClaimAsync(claim);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CLM-20240101-1234", result.ClaimNumber);
        Assert.Equal("John Doe", result.ClaimantName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetClaimByIdAsync_ShouldReturnClaim_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);
        var claim = new Claim
        {
            ClaimNumber = "CLM-20240101-1234",
            ClaimantName = "John Doe",
            ClaimantEmail = "john@example.com",
            ClaimAmount = 5000.00m,
            IncidentDate = DateTime.UtcNow,
            FiledDate = DateTime.UtcNow,
            Status = "Pending"
        };
        await service.CreateClaimAsync(claim);

        // Act
        var result = await service.GetClaimByIdAsync(claim.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(claim.Id, result.Id);
        Assert.Equal("John Doe", result.ClaimantName);
    }

    [Fact]
    public async Task GetClaimByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);

        // Act
        var result = await service.GetClaimByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateClaimAsync_ShouldUpdateClaim_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);
        var claim = new Claim
        {
            ClaimNumber = "CLM-20240101-1234",
            ClaimantName = "John Doe",
            ClaimantEmail = "john@example.com",
            ClaimAmount = 5000.00m,
            IncidentDate = DateTime.UtcNow,
            FiledDate = DateTime.UtcNow,
            Status = "Pending"
        };
        var created = await service.CreateClaimAsync(claim);

        // Act
        created.Status = "Approved";
        created.ClaimAmount = 6000.00m;
        var result = await service.UpdateClaimAsync(created.Id, created);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Approved", result.Status);
        Assert.Equal(6000.00m, result.ClaimAmount);
    }

    [Fact]
    public async Task DeleteClaimAsync_ShouldReturnTrue_WhenClaimExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);
        var claim = new Claim
        {
            ClaimNumber = "CLM-20240101-1234",
            ClaimantName = "John Doe",
            ClaimantEmail = "john@example.com",
            ClaimAmount = 5000.00m,
            IncidentDate = DateTime.UtcNow,
            FiledDate = DateTime.UtcNow,
            Status = "Pending"
        };
        var created = await service.CreateClaimAsync(claim);

        // Act
        var result = await service.DeleteClaimAsync(created.Id);

        // Assert
        Assert.True(result);
        var deleted = await service.GetClaimByIdAsync(created.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetClaimsByStatusAsync_ShouldReturnFilteredClaims()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new ClaimService(context);

        var pendingClaim = new Claim
        {
            ClaimNumber = "CLM-001",
            ClaimantName = "John Doe",
            ClaimantEmail = "john@example.com",
            ClaimAmount = 5000.00m,
            IncidentDate = DateTime.UtcNow,
            FiledDate = DateTime.UtcNow,
            Status = "Pending"
        };

        var approvedClaim = new Claim
        {
            ClaimNumber = "CLM-002",
            ClaimantName = "Jane Smith",
            ClaimantEmail = "jane@example.com",
            ClaimAmount = 3000.00m,
            IncidentDate = DateTime.UtcNow,
            FiledDate = DateTime.UtcNow,
            Status = "Approved"
        };

        await service.CreateClaimAsync(pendingClaim);
        await service.CreateClaimAsync(approvedClaim);

        // Act
        var pendingClaims = await service.GetClaimsByStatusAsync("Pending");

        // Assert
        Assert.Single(pendingClaims);
        Assert.Equal("CLM-001", pendingClaims.First().ClaimNumber);
    }
}

