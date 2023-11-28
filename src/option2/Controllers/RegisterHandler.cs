namespace option2;

using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class RegisterHandler
{
  private readonly UserManager<IdentityUser> userManager;
  private readonly RoleManager<IdentityRole> roleManager;
  private readonly HttpContextAccessor httpContext;

  public RegisterHandler(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager, HttpContextAccessor _httpContext)
  {
    userManager = _userManager;
    roleManager = _roleManager;
    httpContext = _httpContext;
  }

  public async Task<IResult> HandleAsync()
  {
    var roleDoctor = await roleManager.FindByNameAsync("Doctor");
    if (roleDoctor == null)
    {
      await roleManager.CreateAsync(new IdentityRole("Doctor"));
    }
    var roleAdmin = await roleManager.FindByNameAsync("Admin");
    if (roleAdmin == null)
    {
      await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var username = httpContext.HttpContext.Request.Form["email"].ToString() ?? "";
    var password = httpContext.HttpContext.Request.Form["password"].ToString() ?? "";

    var user = new IdentityUser
    {
      UserName = username,
      Email = username
    };

    var result = await userManager.CreateAsync(user, password);

    if (!result.Succeeded)
    {
      return Results.Ok(result.Errors);
    }

    result = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, username));
    result = await userManager.AddClaimAsync(user, new Claim("Department", "IT"));

    if (!result.Succeeded)
    {
      return Results.Unauthorized();
    }

    await userManager.AddToRoleAsync(user, "Admin");

    return Results.Ok(user);
  }
}
