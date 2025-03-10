public static class PlaneEndpoints
{
  public static void MapPlaneEndpoints(this WebApplication app)
  {
    app.MapGet("/v1/vehicles/planes/land_permission", (Guid guid, string runway, ControlService service) =>
    {
      // Заглушка логики проверки разрешения
      var response = new
      {
        allowed = true,
        planeParking = "P1"
      };
      return Results.Json(response);
    });

    app.MapPost("/v1/vehicles/planes/land", (LandRequest request, ControlService service) =>
    {
      // Логика посадки самолета
      var response = new
      {
        success = true,
        planeParking = "P1"
      };
      return Results.Json(response);
    });

    app.MapPost("/v1/vehicles/planes/takeoff", (TakeoffRequest request, ControlService service) =>
    {
      // Проверка нахождения на взлетной полосе
      if (request.Runway != "R1")
        return Results.BadRequest();

      return Results.Ok();
    });
  }
}

public record LandRequest(Guid Guid, string Runway);
public record TakeoffRequest(Guid Guid, string Runway);