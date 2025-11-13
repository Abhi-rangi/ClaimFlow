using FluentValidation;
using VerusClaims.API.DTOs;

namespace VerusClaims.API.Validators;

public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    private static readonly string[] ValidStatuses = { "Pending", "Under Review", "Approved", "Denied", "Closed" };

    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.ClaimantName)
            .NotEmpty().WithMessage("Claimant name is required")
            .MaximumLength(200).WithMessage("Claimant name must not exceed 200 characters");

        RuleFor(x => x.ClaimantEmail)
            .NotEmpty().WithMessage("Claimant email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters");

        RuleFor(x => x.ClaimantPhone)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .Matches(@"^[\d\s\-\(\)\+]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.ClaimantPhone));

        RuleFor(x => x.ClaimAmount)
            .GreaterThan(0).WithMessage("Claim amount must be greater than zero")
            .LessThanOrEqualTo(10000000).WithMessage("Claim amount must not exceed 10,000,000");

        RuleFor(x => x.IncidentDate)
            .NotEmpty().WithMessage("Incident date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Incident date cannot be in the future");

        RuleFor(x => x.FiledDate)
            .NotEmpty().WithMessage("Filed date is required")
            .GreaterThanOrEqualTo(x => x.IncidentDate).WithMessage("Filed date must be on or after incident date")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Filed date cannot be in the future");

        RuleFor(x => x.Status)
            .Must(status => ValidStatuses.Contains(status))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

