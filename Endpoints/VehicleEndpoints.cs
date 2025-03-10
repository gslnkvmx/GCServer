public static class VehicleEndpoints
{
  public static void MapVehicleEndpoints(this WebApplication app)
  {
    app.MapPost("/v1/vehicles/init", (InitRequest request, ControlService service) =>
    {
      // Логика инициализации
      return Results.Ok();
    });

    app.MapGet("/v1/vehicles/move_permission", (Guid guid, string from, string to, ControlService service) =>
    {
      var allowed = service.RequestMovePermission(guid, from, to);
      return Results.Json(new { guid, from, to, allowed });
    });
  }
}

public record InitRequest(List<Guid> Vehicles, List<string> Nodes);