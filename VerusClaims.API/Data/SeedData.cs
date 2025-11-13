using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VerusClaims.API.Models;

namespace VerusClaims.API.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<Adjuster> userManager, RoleManager<IdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    public static async Task SeedClaimsAsync(VerusClaimsDbContext context)
    {
        if (await context.Claims.AnyAsync())
        {
            return; // Database already seeded
        }

        var claims = new List<Claim>
        {
            new Claim
            {
                ClaimNumber = "CLM-20241101-1001",
                ClaimantName = "John Doe",
                ClaimantEmail = "john.doe@example.com",
                ClaimantPhone = "555-0101",
                ClaimAmount = 12500.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-45),
                FiledDate = DateTime.UtcNow.AddDays(-40),
                Status = "Pending",
                Description = "Vehicle accident on Highway 101. Rear-end collision with significant damage to both vehicles.",
                Notes = "Awaiting police report and vehicle inspection.",
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                CreatedBy = "System"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241105-1002",
                ClaimantName = "Jane Smith",
                ClaimantEmail = "jane.smith@example.com",
                ClaimantPhone = "555-0102",
                ClaimAmount = 8500.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-35),
                FiledDate = DateTime.UtcNow.AddDays(-30),
                Status = "Under Review",
                Description = "Property damage claim - Water damage from burst pipe in apartment.",
                Notes = "Inspector scheduled for next week. Photos submitted.",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedBy = "adjuster@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241110-1003",
                ClaimantName = "Michael Johnson",
                ClaimantEmail = "michael.j@example.com",
                ClaimantPhone = "555-0103",
                ClaimAmount = 25000.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-60),
                FiledDate = DateTime.UtcNow.AddDays(-55),
                Status = "Approved",
                Description = "Home fire damage claim. Kitchen fire caused by electrical malfunction.",
                Notes = "Approved after investigation. Payment processing.",
                CreatedAt = DateTime.UtcNow.AddDays(-55),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedBy = "admin@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241112-1004",
                ClaimantName = "Sarah Williams",
                ClaimantEmail = "sarah.w@example.com",
                ClaimantPhone = "555-0104",
                ClaimAmount = 3200.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-20),
                FiledDate = DateTime.UtcNow.AddDays(-18),
                Status = "Denied",
                Description = "Theft claim for stolen electronics. Policy does not cover items outside home.",
                Notes = "Denied - Not covered under policy terms. Claimant notified.",
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedBy = "adjuster@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241115-1005",
                ClaimantName = "Robert Brown",
                ClaimantEmail = "robert.brown@example.com",
                ClaimantPhone = "555-0105",
                ClaimAmount = 6800.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-15),
                FiledDate = DateTime.UtcNow.AddDays(-12),
                Status = "Pending",
                Description = "Medical claim - Slip and fall accident at workplace.",
                Notes = "Waiting for medical records and employer statement.",
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                CreatedBy = "System"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241118-1006",
                ClaimantName = "Emily Davis",
                ClaimantEmail = "emily.davis@example.com",
                ClaimantPhone = "555-0106",
                ClaimAmount = 15000.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-25),
                FiledDate = DateTime.UtcNow.AddDays(-22),
                Status = "Under Review",
                Description = "Auto insurance claim - Hit and run incident. Vehicle parked when damaged.",
                Notes = "Police report filed. Security camera footage being reviewed.",
                CreatedAt = DateTime.UtcNow.AddDays(-22),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedBy = "adjuster@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241120-1007",
                ClaimantName = "David Miller",
                ClaimantEmail = "david.miller@example.com",
                ClaimantPhone = "555-0107",
                ClaimAmount = 9500.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-8),
                FiledDate = DateTime.UtcNow.AddDays(-5),
                Status = "Pending",
                Description = "Property damage - Storm damage to roof and windows.",
                Notes = "Weather event confirmed. Assessment pending.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                CreatedBy = "System"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241025-1008",
                ClaimantName = "Lisa Anderson",
                ClaimantEmail = "lisa.a@example.com",
                ClaimantPhone = "555-0108",
                ClaimAmount = 4200.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-70),
                FiledDate = DateTime.UtcNow.AddDays(-65),
                Status = "Closed",
                Description = "Personal property claim - Damaged furniture during move.",
                Notes = "Claim settled and closed. Payment completed.",
                CreatedAt = DateTime.UtcNow.AddDays(-65),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedBy = "admin@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241122-1009",
                ClaimantName = "James Wilson",
                ClaimantEmail = "james.wilson@example.com",
                ClaimantPhone = "555-0109",
                ClaimAmount = 18000.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-12),
                FiledDate = DateTime.UtcNow.AddDays(-10),
                Status = "Under Review",
                Description = "Business interruption claim - Store closure due to water damage.",
                Notes = "Loss of income calculation in progress. Business records requested.",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedBy = "adjuster@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241125-1010",
                ClaimantName = "Patricia Taylor",
                ClaimantEmail = "patricia.t@example.com",
                ClaimantPhone = "555-0110",
                ClaimAmount = 5500.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-3),
                FiledDate = DateTime.UtcNow.AddDays(-1),
                Status = "Pending",
                Description = "Auto claim - Vandalism damage to vehicle. Broken windows and scratched paint.",
                Notes = "Police report number: PR-2024-12345. Photos attached.",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                CreatedBy = "System"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241128-1011",
                ClaimantName = "Christopher Martinez",
                ClaimantEmail = "chris.martinez@example.com",
                ClaimantPhone = "555-0111",
                ClaimAmount = 11200.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-18),
                FiledDate = DateTime.UtcNow.AddDays(-16),
                Status = "Approved",
                Description = "Homeowners claim - Tree fell on roof during storm. Structural damage confirmed.",
                Notes = "Approved. Contractor estimate received. Work scheduled.",
                CreatedAt = DateTime.UtcNow.AddDays(-16),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedBy = "admin@claimsplatform.com"
            },
            new Claim
            {
                ClaimNumber = "CLM-20241130-1012",
                ClaimantName = "Jennifer Garcia",
                ClaimantEmail = "jennifer.g@example.com",
                ClaimantPhone = "555-0112",
                ClaimAmount = 7500.00m,
                IncidentDate = DateTime.UtcNow.AddDays(-28),
                FiledDate = DateTime.UtcNow.AddDays(-25),
                Status = "Denied",
                Description = "Travel insurance claim - Trip cancellation. Policy exclusion applies.",
                Notes = "Denied - Cancellation reason not covered. Documentation reviewed.",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedBy = "adjuster@claimsplatform.com"
            }
        };

        context.Claims.AddRange(claims);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Adjuster" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<Adjuster> userManager)
    {
        // Seed Admin User
        if (await userManager.FindByEmailAsync("admin@claimsplatform.com") == null)
        {
            var admin = new Adjuster
            {
                UserName = "admin@claimsplatform.com",
                Email = "admin@claimsplatform.com",
                FirstName = "Admin",
                LastName = "User",
                Department = "Administration",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Seed Adjuster User
        if (await userManager.FindByEmailAsync("adjuster@claimsplatform.com") == null)
        {
            var adjuster = new Adjuster
            {
                UserName = "adjuster@claimsplatform.com",
                Email = "adjuster@claimsplatform.com",
                FirstName = "John",
                LastName = "Adjuster",
                Department = "Claims",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adjuster, "Adjuster@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adjuster, "Adjuster");
            }
        }
    }
}

