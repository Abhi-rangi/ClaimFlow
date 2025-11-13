using VerusClaims.API.Models;

namespace VerusClaims.API.Services;

public interface IClaimService
{
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> GetClaimByIdAsync(int id);
    Task<Claim?> GetClaimByClaimNumberAsync(string claimNumber);
    Task<IEnumerable<Claim>> GetClaimsByStatusAsync(string status);
    Task<Claim> CreateClaimAsync(Claim claim);
    Task<Claim?> UpdateClaimAsync(int id, Claim claim);
    Task<bool> DeleteClaimAsync(int id);
}

