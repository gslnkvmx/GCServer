using System.Diagnostics.CodeAnalysis;

public class Node
{
  public required string Name { get; set; }
  public int Capacity { get; set; } = 1;
  public required string Type { get; set; }
  public List<Vehicle> Vehicles { get; set; } = new();
  public List<Reservation> Reserved { get; set; } = new();

  [SetsRequiredMembers]
  public Node(string name)
  {
    Name = name;
    Type = SetNodeType(name);
  }

  private static string SetNodeType(string name)
  {
    return name switch
    {
      _ when name.StartsWith("G") => "gate",
      _ when name.StartsWith("CR") => "crossroad",
      _ when name.StartsWith("P") => "planeParking",
      _ => "crossroad"
    };
  }
}
