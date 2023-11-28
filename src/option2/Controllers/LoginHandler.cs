namespace option2;

using Microsoft.AspNetCore.Identity;

public class LoginHandler
{
  private readonly SignInManager<IdentityUser> signInManager;
  private readonly UserManager<IdentityUser> userManager;
  private readonly HttpContextAccessor httpContext;

  public LoginHandler(SignInManager<IdentityUser> _signInManager, UserManager<IdentityUser> _userManager, HttpContextAccessor _httpContext)
  {
    signInManager = _signInManager;
    userManager = _userManager;
    httpContext = _httpContext;
  }

  public async Task HandleAsync()
  {
    signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

    var username = httpContext.HttpContext.Request.Form["username"].ToString() ?? "";
    var password = httpContext.HttpContext.Request.Form["password"].ToString() ?? "";

    var user = await userManager.FindByEmailAsync(username);

    if (user == null)
    {
      httpContext.HttpContext.Response.StatusCode = 404;
      return;
    }

    var result = await signInManager.PasswordSignInAsync(user, password, false, false);

    if (!result.Succeeded)
    {
      httpContext.HttpContext.Response.StatusCode = 404;
      return;
    }
  }
}
