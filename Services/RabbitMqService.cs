using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqService : IDisposable
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private const string QueueName = "render.action";
  private const string MoveQueue = "render.move";
  private const string InitQueue = "render.init";

  public RabbitMqService(string hostname)
  {
    var factory = new ConnectionFactory();
    factory.Uri = new Uri(hostname);
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.QueueDelete(MoveQueue, ifUnused: false, ifEmpty: false);

    _channel.QueueDeclare(
        queue: MoveQueue,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    _channel.QueueDelete(InitQueue, ifUnused: false, ifEmpty: false);

    _channel.QueueDeclare(
        queue: InitQueue,
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

  public void PublishMoveRenderAction(string guid, string start, string end)
  {
    var message = $"/move {guid} {start} {end}";
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(
        exchange: "",
        routingKey: "render.move",
        basicProperties: null,
        body: body);
  }

  public void PublishInitRenderAction(string guid, string node)
  {
    var message = $"/init {guid} {node}";
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(
        exchange: "",
        routingKey: "render.init",
        basicProperties: null,
        body: body);
  }

  public void PublishClearRenderAction(string type)
  {
    var message = $"/clear {type}";
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(
        exchange: "",
        routingKey: "render.init",
        basicProperties: null,
        body: body);
  }

  public void Dispose()
  {
    _channel?.Close();
    _connection?.Close();
  }
}