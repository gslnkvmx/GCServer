using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class Node : MapPoint
{
  [SetsRequiredMembers]
  public Node(string name, int capacity = 1)
  {
    Name = name;
    Type = SetNodeType(name);
    if (Type == "carGarage") capacity = 30;
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
      _ when name.StartsWith("EX") => "exit",
      _ when name.StartsWith("RS") => "refuelStation",
      _ when name.StartsWith("FS") => "followMeStation",
      _ when name.StartsWith("CG") => "carGarage",
      _ when name.StartsWith("RW") => "runway",
      _ when name.StartsWith("RE") => "runwayEntrance",
      _ => "unknown"
    };
  }
}
