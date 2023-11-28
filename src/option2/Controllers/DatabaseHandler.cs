namespace option2;

public class DatabaseHandler
{
  private readonly ApplicationDbContext db;

  public DatabaseHandler(ApplicationDbContext _db)
  {
    db = _db;
  }

  public async Task<IResult> HandleAsync()
  {
    await Task.Delay(10);
    return Results.Ok(new
    {
      users = db.Users.ToList(),
      userClaims = db.UserClaims.ToList(),
      roles = db.Roles.ToList(),
      userRoles = db.UserRoles.ToList()
    });
  }
}
