namespace VerusClaims.API.DTOs;

public class CreateClaimRequest
{
    public string? ClaimNumber { get; set; }
    public string ClaimantName { get; set; } = string.Empty;
    public string ClaimantEmail { get; set; } = string.Empty;
    public string ClaimantPhone { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
    public DateTime IncidentDate { get; set; }
    public DateTime FiledDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

