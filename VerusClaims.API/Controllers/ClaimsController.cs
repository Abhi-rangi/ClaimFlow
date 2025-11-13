using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerusClaims.API.DTOs;
using VerusClaims.API.Mappings;
using VerusClaims.API.Models;
using VerusClaims.API.Services;

namespace VerusClaims.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
// TODO: Re-enable authorization in production
// [Authorize] // Require authentication for all endpoints
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;
    private readonly ILogger<ClaimsController> _logger;

    public ClaimsController(IClaimService claimService, ILogger<ClaimsController> logger)
    {
        _claimService = claimService;
        _logger = logger;
    }

    /// <summary>
    /// Get all claims
    /// </summary>
    /// <returns>List of claims</returns>
    [HttpGet]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireViewerRole")]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetClaims()
    {
        _logger.LogInformation("Getting all claims");
        var claims = await _claimService.GetAllClaimsAsync();
        var claimDtos = claims.Select(c => c.ToDto());
        return Ok(claimDtos);
    }

    /// <summary>
    /// Get a claim by ID
    /// </summary>
    /// <param name="id">Claim ID</param>
    /// <returns>Claim details</returns>
    [HttpGet("{id}")]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireViewerRole")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetClaim(int id)
    {
        _logger.LogInformation("Getting claim with ID {ClaimId}", id);
        var claim = await _claimService.GetClaimByIdAsync(id);
        if (claim == null)
        {
            _logger.LogWarning("Claim with ID {ClaimId} not found", id);
            return NotFound();
        }
        return Ok(claim.ToDto());
    }

    /// <summary>
    /// Get claims by status
    /// </summary>
    /// <param name="status">Claim status</param>
    /// <returns>List of claims with the specified status</returns>
    [HttpGet("status/{status}")]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireViewerRole")]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetClaimsByStatus(string status)
    {
        _logger.LogInformation("Getting claims with status {Status}", status);
        var claims = await _claimService.GetClaimsByStatusAsync(status);
        var claimDtos = claims.Select(c => c.ToDto());
        return Ok(claimDtos);
    }

    /// <summary>
    /// Create a new claim
    /// </summary>
    /// <param name="request">Claim creation request</param>
    /// <returns>Created claim</returns>
    [HttpPost]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireCSRRole")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> CreateClaim([FromBody] CreateClaimRequest request)
    {
        _logger.LogInformation("Creating new claim for claimant {ClaimantName}", request.ClaimantName);
        
        var claim = request.ToEntity();
        var createdClaim = await _claimService.CreateClaimAsync(claim);
        _logger.LogInformation("Created claim with ID {ClaimId} and number {ClaimNumber}", 
            createdClaim.Id, createdClaim.ClaimNumber);
        
        return CreatedAtAction(nameof(GetClaim), new { id = createdClaim.Id, version = "1.0" }, 
            createdClaim.ToDto());
    }

    /// <summary>
    /// Update an existing claim
    /// </summary>
    /// <param name="id">Claim ID</param>
    /// <param name="request">Claim update request</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireAdjusterRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClaim(int id, [FromBody] UpdateClaimRequest request)
    {
        _logger.LogInformation("Updating claim with ID {ClaimId}", id);
        
        var existingClaim = await _claimService.GetClaimByIdAsync(id);
        if (existingClaim == null)
        {
            _logger.LogWarning("Claim with ID {ClaimId} not found for update", id);
            return NotFound();
        }

        existingClaim.UpdateFromRequest(request);
        var updatedClaim = await _claimService.UpdateClaimAsync(id, existingClaim);
        if (updatedClaim == null)
        {
            _logger.LogWarning("Failed to update claim with ID {ClaimId}", id);
            return NotFound();
        }

        _logger.LogInformation("Successfully updated claim with ID {ClaimId}", id);
        return NoContent();
    }

    /// <summary>
    /// Delete a claim (Admin only)
    /// </summary>
    /// <param name="id">Claim ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    // TODO: Re-enable authorization in production
    // [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClaim(int id)
    {
        _logger.LogInformation("Deleting claim with ID {ClaimId}", id);
        var result = await _claimService.DeleteClaimAsync(id);
        if (!result)
        {
            _logger.LogWarning("Claim with ID {ClaimId} not found for deletion", id);
            return NotFound();
        }

        _logger.LogInformation("Successfully deleted claim with ID {ClaimId}", id);
        return NoContent();
    }
}

