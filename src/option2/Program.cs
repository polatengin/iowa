using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }
}
