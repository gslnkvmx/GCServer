public class ControlService
{
  public List<Node> Nodes { get; } = new();
  public List<Edge> Edges { get; } = new();
  public List<MapPoint> MapPoints { get; } = new();
  private readonly object _lock = new();

  public ControlService()
  {
    InitializeMapFromFile("./configs/map.txt");
    PrintMapToConsole();
  }

  private void InitializeMapFromFile(string filePath)
  {
    lock (_lock)
    {
      var lines = File.ReadAllLines(filePath);
      foreach (var line in lines)
      {
        var parts = line.Split('>');
        if (parts.Length == 3)
        {
          var node1 = parts[0];
          var edgeName = parts[1];
          var node2 = parts[2];

          AddNodeIfNotExists(node1);
          AddNodeIfNotExists(node2);
          AddEdgeIfNotExists(edgeName, node1, node2);
        }
      }
    }
  }

  private void AddNodeIfNotExists(string nodeName)
  {
    if (!Nodes.Any(n => n.Name == nodeName))
    {
      var node = new Node(nodeName);
      Nodes.Add(node);
      MapPoints.Add(node);
    }
  }

  private void AddEdgeIfNotExists(string edgeName, string node1, string node2)
  {
    if (!Edges.Any(e => e.Name == edgeName))
    {
      var edge = new Edge(edgeName, node1, node2);
      if (edgeName.StartsWith("ES")) edge = new Edge(edgeName, node1, node2, "planeRoad", 1);
      else if (edgeName.StartsWith("EP")) edge = new Edge(edgeName, node1, node2, "planeRoad", 2);

      Edges.Add(edge);
      MapPoints.Add(edge);
    }
  }

  public bool RequestMovePermission(string guid, string from, string to)
  {
    lock (_lock)
    {
      var fromPoint = MapPoints.FirstOrDefault(e => e.Name == from);
      var toPoint = MapPoints.FirstOrDefault(e => e.Name == to);

      if (fromPoint == null || toPoint == null) return false;

      var vehicle = fromPoint.Vehicles.First(v => v.Guid == guid);

      if (vehicle == null) return false;

      if (toPoint.Vehicles.Count() >= toPoint.Capacity)
        return false;

      vehicle.Status = "reserved";
      return true;
    }
  }

  public (bool Success, string? Error) HandleMoveRequest(string guid, string from, string to)
  {
    lock (_lock)
    {
      var fromPoint = MapPoints.FirstOrDefault(e => e.Name == from);
      if (fromPoint == null) return (false, $"Не существует такой точки: {from}");
      var toPoint = MapPoints.FirstOrDefault(e => e.Name == to);
      if (toPoint == null) return (false, $"Не существует такой точки: {to}");

      // Проверка резервации
      var vehicle = toPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null || vehicle.Status != "reserved")
        return (false, "Разрешение на движение отсутствует");

      vehicle = fromPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null) return (false, "Техника не находится на указанной точке отправления");

      vehicle.Status = "moving";

      // Освобождение предыдцщей позиции
      fromPoint.Vehicles.Remove(vehicle);

      // Отправка в визуализатор (заглушка)
      //_ = SendToVisualizer(new { guid, from, to });

      return (true, null);
    }
  }

  public (bool Success, string? Error) HandleArrival(string guid, string from, string to)
  {
    lock (_lock)
    {
      var fromPoint = MapPoints.FirstOrDefault(e => e.Name == from);
      if (fromPoint == null) return (false, $"Не существует такой точки: {from}");
      var toPoint = MapPoints.FirstOrDefault(e => e.Name == to);
      if (toPoint == null) return (false, $"Не существует такой точки: {to}");

      // Проверка резервации
      var vehicle = toPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null || vehicle.Status != "moving")
        return (false, "Разрешение на движение отсутствует");

      vehicle = fromPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null) return (false, "Техника не находится на указанной точке отправления");

      vehicle.Status = "waiting";

      return (true, null);
    }
  }

  private async Task SendToVisualizer(object data)
  {
    using var client = new HttpClient();
    await client.PostAsJsonAsync("http://visualizer/renderMove", data);
  }

  public (bool Success, string? Error) InitVehicles(List<string> vehicles, List<string> nodes)
  {
    lock (_lock)
    {
      if (vehicles.Count != nodes.Count)
        return (false, "Количество транспортных средств и узлов не совпадает");

      if (vehicles.Distinct().Count() != vehicles.Count)
        return (false, "Обнаружены повторяющиеся GUID");

      for (int i = 0; i < vehicles.Count; i++)
      {
        var guid = vehicles[i];
        var nodeName = nodes[i];

        var node = Nodes.FirstOrDefault(n => n.Name == nodeName);
        if (node == null)
          return (false, $"Узел {nodeName} не найден");

        if (node.Vehicles.Count >= node.Capacity)
          return (false, $"Узел {nodeName} переполнен");

        if (Nodes.Any(n => n.Vehicles.Any(v => v.Guid == guid)) ||
           Edges.Any(e => e.Vehicles.Any(v => v.Guid == guid)))
          return (false, $"Транспортное средство {guid} уже существует");

        node.Vehicles.Add(new Vehicle
        {
          Guid = guid,
          VehicleType = node.Type switch
          {
            "planeParking" or "runway" => "plane",
            _ => "car"
          },
          Status = "waiting"
        });

        Console.WriteLine($"Инициализировано транспортное средство {guid} на узле {nodeName}");
      }
      return (true, null);
    }
  }

  public void PrintMapToConsole()
  {
    Console.WriteLine("\n=== Визуализация графа ===");

    // Вывод всех узлов
    Console.WriteLine("\nУзлы:");
    foreach (var node in Nodes)
    {
      Console.WriteLine($"• {node.Name} [{node.Type}]");
    }

    // Вывод всех ребер
    Console.WriteLine("\nРебра:");
    foreach (var edge in Edges)
    {
      Console.WriteLine($"═ {edge.Name} ({edge.Type})");
      Console.WriteLine($"   {edge.Node1} ←→ {edge.Node2}");
    }

    Console.WriteLine("\nСоединения:");
    foreach (var node in Nodes)
    {
      var connections = Edges
          .Where(e => e.Node1 == node.Name || e.Node2 == node.Name)
          .Select(e => e.Node1 == node.Name ? e.Node2 : e.Node1);

      Console.WriteLine($"{node.Name} → {string.Join(", ", connections)}");
    }

    Console.WriteLine("=======================\n");
  }

  // Другие методы: HandleArrival, Pathfinding и т.д.
}