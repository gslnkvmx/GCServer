public class ControlService
{
  public List<Node> Nodes { get; } = new();
  public List<Edge> Edges { get; } = new();
  private readonly object _lock = new();

  public ControlService()
  {
    InitializeFromFile("./configs/map.txt");
  }

  private void InitializeFromFile(string filePath)
  {
    lock (_lock)
    {
      var lines = File.ReadAllLines(filePath);
      foreach (var line in lines)
      {
        var parts = line.Split('-');
        if (parts.Length != 3) continue;

        var node1 = parts[0];
        var edgeName = parts[1];
        var node2 = parts[2];

        AddNodeIfNotExists(node1);
        AddNodeIfNotExists(node2);
        AddEdgeIfNotExists(edgeName, node1, node2);
      }
    }
  }

  private void AddNodeIfNotExists(string nodeName)
  {
    if (!Nodes.Any(n => n.Name == nodeName))
    {
      Nodes.Add(new Node(nodeName));
    }
  }

  private void AddEdgeIfNotExists(string edgeName, string node1, string node2)
  {
    if (!Edges.Any(e => e.Name == edgeName))
    {
      Edges.Add(new Edge(edgeName, node1, node2));
    }
  }

  public bool RequestMovePermission(Guid guid, string from, string to)
  {
    lock (_lock)
    {
      var edge = Edges.FirstOrDefault(e =>
          (e.Node1 == from && e.Node2 == to) ||
          (e.Node2 == from && e.Node1 == to));

      if (edge != null)
      {
        if (edge.Vehicles.Count(v => v.To == to) >= edge.Capacity)
          return false;

        edge.Reserved.Add(new Reservation { Guid = guid, From = from, To = to });
        return true;
      }

      var node = Nodes.FirstOrDefault(n => n.Name == to);
      return node?.Vehicles.Count < node?.Capacity;
    }
  }

  // Другие методы: InitVehicles, HandleArrival, Pathfinding и т.д.
}