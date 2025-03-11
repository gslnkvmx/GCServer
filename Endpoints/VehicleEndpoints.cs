using Microsoft.AspNetCore.Mvc;

public static class VehicleEndpoints
{
  public static void MapVehicleEndpoints(this WebApplication app)
  {
    app.MapPost("/v1/vehicles/init", (InitRequest request, ControlService service) =>
        {
          var (success, error) = service.InitVehicles(request.Vehicles, request.Nodes);

          return success
              ? Results.Ok()
              : Results.BadRequest(new { error });
        });

    app.MapGet("/v1/vehicles/move_permission", ([FromBody] CarRequest request, ControlService service) =>
    {
      var allowed = service.RequestMovePermission(
              request.Guid,
              request.From,
              request.To
          );
      return Results.Json(new
      {
        request.Guid,
        request.From,
        request.To,
        allowed
      });
    });

    app.MapPost("/v1/vehicles/move", (CarRequest request, ControlService service) =>
        {
          var (success, error) = service.HandleMoveRequest(
              request.Guid,
              request.From,
              request.To
          );

          return success
              ? Results.Ok()
              : Results.BadRequest(new { error });
        });

    app.MapPost("/v1/vehicles/arrived", (CarRequest request, ControlService service) =>
    {
      var (success, error) = service.HandleArrival(
              request.Guid,
              request.From,
              request.To
          );

      return success
              ? Results.Ok()
              : Results.BadRequest(new { error });
    });
  }
}

public record InitRequest(List<string> Vehicles, List<string> Nodes);
public record CarRequest(string Guid, string VehicleType, string From, string To);