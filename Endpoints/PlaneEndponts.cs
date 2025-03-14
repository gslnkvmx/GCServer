public static class PlaneEndpoints
{
  public static void MapPlaneEndpoints(this WebApplication app)
  {
    app.MapGet("/v1/vehicles/planes/land_permission",
        (string guid, string runway, ControlService service, ConsoleLogger logger) =>
    {
      var (result, allowed) = service.GetLandPermission(guid, runway);
      logger.Log("GetLandPermission", $"Разрешение на приземление {guid} на {runway}", allowed);
      return Results.Json(result);
    });

    app.MapPost("/v1/vehicles/planes/land", (LandRequest request, ControlService service, ConsoleLogger logger) =>
    {
      var (result, success) = service.ProcessLanding(request.Guid, request.Runway);
      logger.Log("Land", $"Приземление {request.Guid} на {request.Runway}", success);
      return Results.Json(result);
    });

    app.MapPost("/v1/vehicles/planes/takeoff", (TakeoffRequest request, ControlService service, ConsoleLogger logger) =>
    {
      var (success, error) = service.ProcessTakeoff(request.Guid, request.Runway);
      logger.Log("Takeoff", $"Взлет {request.Guid} с {request.Runway}", success);
      return success
                  ? Results.Ok()
                  : Results.BadRequest(error);
    });
  }
}

public record LandRequest(string Guid, string Runway);
public record TakeoffRequest(string Guid, string Runway);