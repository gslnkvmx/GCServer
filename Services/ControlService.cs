public class ControlService
{
  public List<Node> Nodes { get; } = new();
  public List<Edge> Edges { get; } = new();
  public Dictionary<string, List<string[]>> NodesConnections { get; } = new();
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

          if (!NodesConnections.ContainsKey(node1)) NodesConnections.Add(node1, new List<string[]>());
          if (!NodesConnections.ContainsKey(node2)) NodesConnections.Add(node2, new List<string[]>());

          NodesConnections[node1].Add([edgeName, node2]);
          NodesConnections[node2].Add([edgeName, node1]);
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
    }
  }

  public bool RequestMovePermission(string guid, string from, string to)
  {
    lock (_lock)
    {
      var moveToEdge = false;
      IMapPoint? fromPoint = null;
      IMapPoint? toPoint = null;
      if (from.StartsWith("E"))
      {
        fromPoint = Edges.FirstOrDefault(e => e.Name == from);
        toPoint = Nodes.FirstOrDefault(e => e.Name == to);
      }
      else
      {
        fromPoint = Nodes.FirstOrDefault(e => e.Name == from);
        toPoint = Edges.FirstOrDefault(e => e.Name == to);
        moveToEdge = true;
      }

      if (fromPoint == null || toPoint == null) return false;

      var vehicle = fromPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null) return false;

      if (toPoint.Vehicles.FirstOrDefault(v => v.Guid == guid) != null) return true;

      if (toPoint.Vehicles.Count(v => v.Status != "reserved") >= toPoint.Capacity)
        return false;

      if (moveToEdge)
      {
        if (toPoint.Vehicles.Count == 1)
        {
          VehicleOnEdge vehicleOnEdge = (VehicleOnEdge)toPoint.Vehicles[0];
          if (vehicleOnEdge.From == from) return false;
        }

        toPoint.Vehicles.Add(new VehicleOnEdge((Vehicle)vehicle, from, to, "reserved"));
        return true;
      }

      toPoint.Vehicles.Add(new Vehicle((VehicleOnEdge)vehicle, "reserved"));
      return true;
    }
  }

  public (bool Success, string? Error) HandleMoveRequest(string guid, string from, string to)
  {
    lock (_lock)
    {
      IMapPoint? fromPoint = null;
      IMapPoint? toPoint = null;
      if (from.StartsWith("E"))
      {
        fromPoint = Edges.FirstOrDefault(e => e.Name == from);
        toPoint = Nodes.FirstOrDefault(e => e.Name == to);
      }
      else
      {
        fromPoint = Nodes.FirstOrDefault(e => e.Name == from);
        toPoint = Edges.FirstOrDefault(e => e.Name == to);
      }

      if (fromPoint == null) return (false, $"Не существует такой точки: {from}");
      if (toPoint == null) return (false, $"Не существует такой точки: {to}");

      // Проверка резервации
      var vehicle = toPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null || vehicle.Status != "reserved")
        return (false, "Разрешение на движение отсутствует");

      if (fromPoint.Vehicles.FirstOrDefault(v => v.Guid == guid) == null) return (false, "Техника не находится на указанной точке отправления");

      vehicle.Status = "moving";

      vehicle = fromPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);
      // Освобождение предыдцщей позиции
      fromPoint.Vehicles.Remove(vehicle!);

      // Отправка в визуализатор (заглушка)
      //_ = SendToVisualizer(new { guid, from, to });

      return (true, null);
    }
  }

  public (bool Success, string? Error) HandleArrival(string guid, string from, string to)
  {
    lock (_lock)
    {
      IMapPoint? fromPoint = null;
      IMapPoint? toPoint = null;
      if (from.StartsWith("E"))
      {
        fromPoint = Edges.FirstOrDefault(e => e.Name == from);
        toPoint = Nodes.FirstOrDefault(e => e.Name == to);
      }
      else
      {
        fromPoint = Nodes.FirstOrDefault(e => e.Name == from);
        toPoint = Edges.FirstOrDefault(e => e.Name == to);
      }

      if (fromPoint == null) return (false, $"Не существует такой точки: {from}");
      if (toPoint == null) return (false, $"Не существует такой точки: {to}");

      // Проверка резервации
      var vehicle = toPoint.Vehicles.FirstOrDefault(v => v.Guid == guid);

      if (vehicle == null || vehicle.Status != "moving")
        return (false, "Разрешение на движение отсутствует");

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
        (
          guid,
          node.Type switch
          {
            "planeParking" or "runway" => "plane",
            _ => "car"
          },
          "waiting"
        ));

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

  public List<string> FindPath(string from, string to)
  {
    var queue = new Queue<List<string>>();
    var visited = new HashSet<string>();

    queue.Enqueue(new List<string> { from });
    visited.Add(from);

    while (queue.Count > 0)
    {
      var currentPath = queue.Dequeue();
      var currentNode = currentPath.Last();

      if (currentNode == to)
        return currentPath;

      if (NodesConnections.TryGetValue(currentNode, out var connections))
      {
        foreach (var connection in connections)
        {
          string edgeName = connection[0];
          string neighborNode = connection[1];

          if (!visited.Contains(neighborNode))
          {
            visited.Add(neighborNode);
            // Добавляем ребро и следующую вершину в путь
            var newPath = new List<string>(currentPath) { edgeName, neighborNode };
            queue.Enqueue(newPath);
          }
        }
      }
    }

    // Путь не найден
    return new List<string>();
  }
}