using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using VerusClaims.API.Data;
using VerusClaims.API.Models;
using VerusClaims.API.Services;
using VerusClaims.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/verusclaims-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Use Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Configure problem details for validation errors
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Detail = "See the errors field for details",
                Instance = context.HttpContext.Request.Path
            };
            return new BadRequestObjectResult(problemDetails);
        };
    });

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version")
    );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Configure Swagger with versioning
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Claims Management API",
        Version = "v1",
        Description = "API for managing insurance claims",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Claims Platform Support",
            Email = "support@claimsplatform.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateClaimRequestValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add DbContext - Support both SQL Server and InMemory for testing
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);
if (useInMemory)
{
    builder.Services.AddDbContext<VerusClaimsDbContext>(options =>
        options.UseInMemoryDatabase("ClaimsManagementDB"));
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("ClaimsIdentityDB"));
}
else
{
    builder.Services.AddDbContext<VerusClaimsDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Add Identity
builder.Services.AddIdentity<Adjuster, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Auth0 Authentication
var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0Audience = builder.Configuration["Auth0:Audience"];

if (!string.IsNullOrEmpty(auth0Domain))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain}";
        options.Audience = auth0Audience ?? $"https://{auth0Domain}/api/v2/";
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}
else
{
    // Fallback to JWT if Auth0 is not configured
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ClaimsManagement";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ClaimsManagement";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });
}

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Auth0 roles/permissions - adjust these to match your Auth0 role names
    // Auth0 roles are typically in the 'https://schemas.quickstarts.dev/roles' claim
    // or in the standard 'roles' claim depending on your Auth0 configuration
    
    // Admin: Full system access
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Contains("Admin") || permissions.Contains("admin:all");
        }));
    
    // Supervisor: Can approve claims and manage adjusters
    options.AddPolicy("RequireSupervisorRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Contains("Supervisor") || roles.Contains("Admin") ||
                   permissions.Contains("approve:claims") || permissions.Contains("admin:all");
        }));
    
    // Adjuster: Can create and update claims (most common role)
    options.AddPolicy("RequireAdjusterRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Contains("Adjuster") || roles.Contains("Supervisor") || roles.Contains("Admin") ||
                   permissions.Any(p => new[] { "read:claims", "write:claims", "approve:claims", "admin:all" }.Contains(p));
        }));
    
    // CSR: Can create claims and update basic info
    options.AddPolicy("RequireCSRRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Any(r => new[] { "CSR", "Adjuster", "Supervisor", "Admin" }.Contains(r)) ||
                   permissions.Any(p => new[] { "read:claims", "write:claims" }.Contains(p));
        }));
    
    // Viewer: Read-only access
    options.AddPolicy("RequireViewerRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Any(r => new[] { "Viewer", "Auditor", "CSR", "Adjuster", "Supervisor", "Admin" }.Contains(r)) ||
                   permissions.Contains("read:claims");
        }));
    
    // Auditor: Read-only with audit access
    options.AddPolicy("RequireAuditorRole", policy => 
        policy.RequireAssertion(context =>
        {
            var roles = context.User.FindAll("https://schemas.quickstarts.dev/roles")
                .Select(c => c.Value)
                .Concat(context.User.FindAll("roles").Select(c => c.Value))
                .ToList();
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            
            return roles.Contains("Auditor") || roles.Contains("Admin") ||
                   permissions.Any(p => new[] { "read:claims", "view:audit", "admin:all" }.Contains(p));
        }));
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<VerusClaimsDbContext>("database");

// Add Services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed Identity data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Adjuster>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var identityContext = services.GetRequiredService<ApplicationDbContext>();
        var claimsContext = services.GetRequiredService<VerusClaimsDbContext>();
        
        if (!useInMemory)
        {
            identityContext.Database.Migrate();
            claimsContext.Database.Migrate();
        }
        
        await SeedData.SeedAsync(identityContext, userManager, roleManager);
        
        // Seed Claims data for SQL Server database
        if (!useInMemory)
        {
            await SeedData.SeedClaimsAsync(claimsContext);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

// Configure the HTTP request pipeline
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Claims API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root
    });
}
else
{
    // In production, enforce HTTPS
    app.UseHsts();
}

app.UseHttpsRedirection();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapControllers();

// Seed in-memory database if using it
if (useInMemory)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<VerusClaimsDbContext>();
        context.Database.EnsureCreated();
        
        // Add sample data for testing
        if (!context.Claims.Any())
        {
            context.Claims.AddRange(
                new Claim
                {
                    ClaimNumber = "CLM-20240101-1001",
                    ClaimantName = "John Doe",
                    ClaimantEmail = "john.doe@example.com",
                    ClaimantPhone = "555-0101",
                    ClaimAmount = 5000.00m,
                    IncidentDate = DateTime.UtcNow.AddDays(-30),
                    FiledDate = DateTime.UtcNow.AddDays(-25),
                    Status = "Pending",
                    Description = "Vehicle accident claim"
                },
                new Claim
                {
                    ClaimNumber = "CLM-20240101-1002",
                    ClaimantName = "Jane Smith",
                    ClaimantEmail = "jane.smith@example.com",
                    ClaimantPhone = "555-0102",
                    ClaimAmount = 3500.00m,
                    IncidentDate = DateTime.UtcNow.AddDays(-20),
                    FiledDate = DateTime.UtcNow.AddDays(-15),
                    Status = "Under Review",
                    Description = "Property damage claim"
                },
                new Claim
                {
                    ClaimNumber = "CLM-20240101-1003",
                    ClaimantName = "Bob Johnson",
                    ClaimantEmail = "bob.johnson@example.com",
                    ClaimantPhone = "555-0103",
                    ClaimAmount = 12000.00m,
                    IncidentDate = DateTime.UtcNow.AddDays(-45),
                    FiledDate = DateTime.UtcNow.AddDays(-40),
                    Status = "Approved",
                    Description = "Medical expenses claim"
                }
            );
            context.SaveChanges();
        }
    }
}

await app.RunAsync();

