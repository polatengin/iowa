using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using option2;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("option_2"));

builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddApiEndpoints();

builder.Services.AddResponseCompression(options =>
{
  options.EnableForHttps = true;
});

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme, options =>
{
  options.BearerTokenExpiration = TimeSpan.FromDays(365);
});

builder.Services.Configure<IdentityOptions>(options =>
{
  options.Password.RequireDigit = true;
  options.Password.RequireLowercase = true;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequiredLength = 6;
  options.Password.RequiredUniqueChars = 1;

  options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
  options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
  options.AddPolicy("RequireDoctor", policy => policy.RequireRole("Doctor"));
  options.AddPolicy("RequireUser", policy => policy.RequireAuthenticatedUser());
  options.AddPolicy("RequireITAdmin", policy => policy.RequireRole("Admin").RequireClaim("Department", "IT"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
  {
    In = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });

  options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddScoped<HttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<DatabaseHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapPost("/register", async (RegisterHandler handler) => await handler.HandleAsync());

app.MapPost("/login", async (LoginHandler handler) => await handler.HandleAsync());

app.MapGet("/db", async (DatabaseHandler handler) => await handler.HandleAsync());

app.MapGet("/weather", async (HttpContext httpContext) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Temperature = Random.Shared.Next(-20, 55),
            Summary = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[Random.Shared.Next(9)]
        })
        .ToArray();
    await Task.Delay(10);
    return Results.Ok(forecast);
})
.WithName("GetWeatherForecast")
.WithDescription("Gets the weather forecast")
.WithOpenApi()
.RequireAuthorization("RequireITAdmin");

app.Run();

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }
}
