public static class MapEndpoints
{
  public static void MapMapEndpoints(this WebApplication app)
  {
    app.MapGet("/v1/map/path", (string from, string to, ControlService service) =>
    {
      // Заглушка для алгоритма поиска пути
      var path = new[] { from, "E15", to };
      return Results.Json(new { path });
    });

    app.MapGet("/v1/map", (ControlService service) =>
    {
      var response = new
      {
        mapVersion = "1",
        nodes = service.Nodes,
        edges = service.Edges
      };
      return Results.Json(response);
    });

    app.MapGet("/v1/map/edges/{edgeName}", (string edgeName, ControlService service) =>
    {
      var edge = service.Edges.FirstOrDefault(e => e.Name == edgeName);
      return edge != null
              ? Results.Json(edge)
              : Results.NotFound();
    });

    app.MapGet("/v1/map/nodes/{nodeName}", (string nodeName, ControlService service) =>
    {
      var node = service.Nodes.FirstOrDefault(n => n.Name == nodeName);
      return node != null
              ? Results.Json(node)
              : Results.NotFound();
    });
  }
}