using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class Edge : MapPoint
{
  [JsonPropertyName("node1")]
  public required string Node1 { get; set; }
  [JsonPropertyName("node2")]
  public required string Node2 { get; set; }
  public new List<VehicleOnEdge> Vehicles { get; set; } = new List<VehicleOnEdge>();
  public Edge() { }

  [SetsRequiredMembers]
  public Edge(string name, string node1, string node2, string type = "carRoad", int capacity = 2)
  {
    Name = name;
    Node1 = node1;
    Node2 = node2;
    Type = type;
    Capacity = capacity;
  }
}

