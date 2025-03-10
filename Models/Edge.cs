using System.Diagnostics.CodeAnalysis;

public class Edge
{
  public required string Name { get; set; }
  public int Capacity { get; set; } = 2;
  public required string Type { get; set; } // "carRoad" или "planeRoad"
  public required string Node1 { get; set; }
  public required string Node2 { get; set; }
  public List<Vehicle> Vehicles { get; set; } = new();
  public List<Reservation> Reserved { get; set; } = new();

  public Edge() { }

  [SetsRequiredMembers]
  public Edge(string name, string node1, string node2, string type = "carRoad")
  {
    Name = name;
    Node1 = node1;
    Node2 = node2;
    Type = type;
  }
}

public class Vehicle
{
  public required Guid Guid { get; set; }
  public required string VehicleType { get; set; }
  public required string From { get; set; }
  public required string To { get; set; }
  public required string Status { get; set; } // "moving" или "waiting"
}

public class Reservation
{
  public required Guid Guid { get; set; }
  public required string From { get; set; }
  public required string To { get; set; }
}

