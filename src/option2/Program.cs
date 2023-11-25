using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("option_2"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }
}
