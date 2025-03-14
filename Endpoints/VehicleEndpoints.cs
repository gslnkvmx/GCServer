using Microsoft.AspNetCore.Mvc;
namespace GCServer.Endpoints
{
  public static class VehicleEndpoints
  {
    public static void MapVehicleEndpoints(this WebApplication app)
    {
      app.MapPost("/v1/vehicles/init", (InitRequest request, ControlService service, ConsoleLogger logger) =>
          {
            var (success, error) = service.InitVehicles(request.Vehicles, request.Nodes);

            logger.Log("init", error, success);

            return success
                ? Results.Ok()
                : Results.BadRequest(new { error });
          });

      app.MapGet("/v1/vehicles/move_permission", (string guid, string from, string to, ControlService service, ConsoleLogger logger) =>
      {
        var allowed = service.RequestMovePermission(guid, from, to);
        logger.Log("RequestMovePermission", $"{guid} из {from} в {to}", allowed);
        return Results.Json(new { guid, from, to, allowed });
      });

      app.MapPost("/v1/vehicles/move", (CarRequest request, ControlService service, ConsoleLogger logger) =>
          {
            var (success, error) = service.HandleMoveRequest(
                request.Guid,
                request.From,
                request.To
            );

            logger.Log("Move", error == null ? $"{request.Guid} из {request.From} в {request.To}" : error, success);

            return success
                ? Results.Ok()
                : Results.BadRequest(new { error });
          });

      app.MapPost("/v1/vehicles/arrived", (CarRequest request, ControlService service, ConsoleLogger logger) =>
      {
        var (success, error) = service.HandleArrival(
                request.Guid,
                request.From,
                request.To
            );

        logger.Log("Arrived", error == null ? $"{request.Guid} из {request.From} в {request.To}" : error, success);

        return success
                ? Results.Ok()
                : Results.BadRequest(new { error });
      });
    }
  }

  public record InitRequest(List<string> Vehicles, List<string> Nodes);
  public record CarRequest(string Guid, string VehicleType, string From, string To);
}

