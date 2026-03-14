using System.Text;
using GardenAssistant.Data;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

if (jwtKey.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<PlantSeeder>();
builder.Services.AddScoped<AssociationSeeder>();
builder.Services.AddScoped<GardenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlantService>();
builder.Services.AddScoped<PlantAssociationService>();
builder.Services.AddScoped<PlantingService>();
builder.Services.AddScoped<PlantingEntryService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedOrigins", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? [];
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v1";
    config.Title = "Garden Assistant API";
    config.Version = "v1";
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("NSwag") && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await scope.ServiceProvider.GetRequiredService<PlantSeeder>().SeedAsync();
    await scope.ServiceProvider.GetRequiredService<AssociationSeeder>().SeedAsync();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.UseCors("RestrictedOrigins");

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace GardenAssistant
{
    public abstract partial class Program;
}
