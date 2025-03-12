using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class Edge : IMapPoint
{
  [JsonPropertyName("name")]
  public string Name { get; set; }
  [JsonPropertyName("capacity")]
  public int Capacity { get; set; }
  [JsonPropertyName("type")]
  public string Type { get; set; }
  [JsonPropertyName("node1")]
  public required string Node1 { get; set; }
  [JsonPropertyName("node2")]
  public required string Node2 { get; set; }
  public List<IVehicle> Vehicles { get; set; } = new List<IVehicle>();

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

