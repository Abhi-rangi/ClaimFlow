using Microsoft.EntityFrameworkCore;
using VerusClaims.API.Data;
using VerusClaims.API.Models;

namespace VerusClaims.API.Services;

public class ClaimService : IClaimService
{
    private readonly VerusClaimsDbContext _context;

    public ClaimService(VerusClaimsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        // Soft delete filter is applied automatically via HasQueryFilter
        return await _context.Claims
            .OrderByDescending(c => c.FiledDate)
            .ToListAsync();
    }

    public async Task<Claim?> GetClaimByIdAsync(int id)
    {
        return await _context.Claims.FindAsync(id);
    }

    public async Task<Claim?> GetClaimByClaimNumberAsync(string claimNumber)
    {
        return await _context.Claims
            .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
    }

    public async Task<IEnumerable<Claim>> GetClaimsByStatusAsync(string status)
    {
        return await _context.Claims
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.FiledDate)
            .ToListAsync();
    }

    public async Task<Claim> CreateClaimAsync(Claim claim)
    {
        if (string.IsNullOrWhiteSpace(claim.ClaimNumber))
        {
            claim.ClaimNumber = GenerateClaimNumber();
        }

        claim.CreatedAt = DateTime.UtcNow;
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim?> UpdateClaimAsync(int id, Claim claim)
    {
        var existingClaim = await _context.Claims.FindAsync(id);
        if (existingClaim == null)
        {
            return null;
        }

        existingClaim.ClaimantName = claim.ClaimantName;
        existingClaim.ClaimantEmail = claim.ClaimantEmail;
        existingClaim.ClaimantPhone = claim.ClaimantPhone;
        existingClaim.ClaimAmount = claim.ClaimAmount;
        existingClaim.IncidentDate = claim.IncidentDate;
        existingClaim.FiledDate = claim.FiledDate;
        existingClaim.Status = claim.Status;
        existingClaim.Description = claim.Description;
        existingClaim.Notes = claim.Notes;
        existingClaim.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingClaim;
    }

    public async Task<bool> DeleteClaimAsync(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null || claim.IsDeleted)
        {
            return false;
        }

        // Soft delete - EF will handle this via the SaveChanges override
        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateClaimNumber()
    {
        var prefix = "CLM";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"{prefix}-{timestamp}-{random}";
    }
}

