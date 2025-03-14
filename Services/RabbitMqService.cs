using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqService : IDisposable
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private const string QueueName = "render.action";
  private const string CarQueue = "render.car";
  private const string PlaneQueue = "render.plane";

  public RabbitMqService(string hostname)
  {
    var factory = new ConnectionFactory();
    factory.Uri = new Uri(hostname);
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.QueueDelete(CarQueue, ifUnused: false, ifEmpty: false);

    _channel.QueueDeclare(
        queue: CarQueue,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    _channel.QueueDelete(PlaneQueue, ifUnused: false, ifEmpty: false);

    _channel.QueueDeclare(
        queue: PlaneQueue,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    _channel.QueueDelete(QueueName, ifUnused: false, ifEmpty: false);

    _channel.QueueDeclare(
        queue: QueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
  }

  public void PublishCarRenderAction(string model, string start, string end)
  {
    var message = $"/car {model} {start} {end}";
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(
        exchange: "",
        routingKey: "render.car",
        basicProperties: null,
        body: body);
  }

  public void PublishPlaneRenderAction(int id)
  {
    var message = $"/plane {id}";
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(
        exchange: "",
        routingKey: "render.plane",
        basicProperties: null,
        body: body);
  }

  public void PublishMoveAction(string guid, string from, string to)
  {
    var message = new
    {
      guid,
      from,
      to
    };

    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

    _channel.BasicPublish(
        exchange: "",
        routingKey: QueueName,
        basicProperties: null,
        body: body);
  }

  public void Dispose()
  {
    _channel?.Close();
    _connection?.Close();
  }
}