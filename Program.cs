using GCServer.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ControlService>();
builder.Services.AddSingleton(new RabbitMqService("amqp://xnyyznus:OSOOLzaQHT5Ys6NPEMAU5DxTChNu2MUe@hawk.rmq.cloudamqp.com:5672/xnyyznus"));
builder.Services.AddSingleton<ConsoleLogger>();

var app = builder.Build();

app.MapVehicleEndpoints();
app.MapPlaneEndpoints();
app.MapMapEndpoints();

PrintEndpoints(app);

app.Run();

void PrintEndpoints(WebApplication app)
{
  Console.WriteLine("Доступные эндпоинты:");
  var endpoints = app as IEndpointRouteBuilder;
  foreach (var endpoint in endpoints.DataSources
      .SelectMany(ds => ds.Endpoints)
      .OfType<RouteEndpoint>())
  {
    var method = endpoint.Metadata
        .OfType<HttpMethodMetadata>()
        .FirstOrDefault()?
        .HttpMethods
        .FirstOrDefault() ?? "ANY";

    var route = endpoint.RoutePattern.RawText;
    Console.WriteLine($"{method} {route}");
  }
}