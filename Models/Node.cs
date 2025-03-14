using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class Node : IMapPoint
{
  [JsonPropertyName("name")]
  public string Name { get; set; }
  [JsonPropertyName("capacity")]
  public int Capacity { get; set; }
  [JsonPropertyName("type")]
  public string Type { get; set; }
  [JsonPropertyName("vehicles")]
  public List<IVehicle> Vehicles { get; set; } = new();
  [SetsRequiredMembers]
  public Node(string name, int capacity = 1)
  {
    Name = name;
    Type = SetNodeType(name);
    if (Type == "carGarage") capacity = 30;
    if (Type == "refuelStation") capacity = 5;
    if (Type == "cateringService") capacity = 4;
    Capacity = capacity;
  }

  private static string SetNodeType(string name)
  {
    return name switch
    {
      _ when name.StartsWith("CR") => "crossroad",
      _ when name.StartsWith("PCR") => "planeCrossroad",
      _ when name.StartsWith("CP") => "planeServicing",
      _ when name.StartsWith("P") => "planeParking",
      _ when name.StartsWith("G") => "gate",
      _ when name.StartsWith("BR") => "baggageRoom",
      _ when name.StartsWith("BW") => "baggageWarehouse",
      _ when name.StartsWith("CS") => "cateringService",
      _ when name.StartsWith("XT") => "exit",
      _ when name.StartsWith("RS") => "refuelStation",
      _ when name.StartsWith("FS") => "followMeStation",
      _ when name.StartsWith("CG") => "carGarage",
      _ when name.StartsWith("RW") => "runway",
      _ when name.StartsWith("RE") => "runwayEntrance",
      _ => "unknown"
    };
  }
}
