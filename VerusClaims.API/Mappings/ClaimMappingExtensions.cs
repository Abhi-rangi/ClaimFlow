using VerusClaims.API.DTOs;
using VerusClaims.API.Models;

namespace VerusClaims.API.Mappings;

public static class ClaimMappingExtensions
{
    public static ClaimDto ToDto(this Claim claim)
    {
        return new ClaimDto
        {
            Id = claim.Id,
            ClaimNumber = claim.ClaimNumber,
            ClaimantName = claim.ClaimantName,
            ClaimantEmail = claim.ClaimantEmail,
            ClaimantPhone = claim.ClaimantPhone,
            ClaimAmount = claim.ClaimAmount,
            IncidentDate = claim.IncidentDate,
            FiledDate = claim.FiledDate,
            Status = claim.Status,
            Description = claim.Description,
            Notes = claim.Notes,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt
        };
    }

    public static Claim ToEntity(this CreateClaimRequest request)
    {
        return new Claim
        {
            ClaimNumber = request.ClaimNumber ?? string.Empty,
            ClaimantName = request.ClaimantName,
            ClaimantEmail = request.ClaimantEmail,
            ClaimantPhone = request.ClaimantPhone,
            ClaimAmount = request.ClaimAmount,
            IncidentDate = request.IncidentDate,
            FiledDate = request.FiledDate,
            Status = request.Status,
            Description = request.Description,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromRequest(this Claim claim, UpdateClaimRequest request)
    {
        claim.ClaimantName = request.ClaimantName;
        claim.ClaimantEmail = request.ClaimantEmail;
        claim.ClaimantPhone = request.ClaimantPhone;
        claim.ClaimAmount = request.ClaimAmount;
        claim.IncidentDate = request.IncidentDate;
        claim.FiledDate = request.FiledDate;
        claim.Status = request.Status;
        claim.Description = request.Description;
        claim.Notes = request.Notes;
        claim.UpdatedAt = DateTime.UtcNow;
    }
}

