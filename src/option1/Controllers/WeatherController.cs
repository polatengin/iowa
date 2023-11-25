using Microsoft.AspNetCore.Mvc;

namespace option1.Controllers
{
  public class WeatherController
  {
    [Route("/weather")]
    public async Task<IResult> PostAsync()
    {
      await Task.Delay(10);

      return Results.Ok(Enumerable.Range(1, 5).Select(index =>
      new
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        Temperature = Random.Shared.Next(-20, 55),
        Summary = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[Random.Shared.Next(9)]
      })
      .ToArray());
    }
  }
}
