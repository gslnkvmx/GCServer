using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public interface IVehicle
{
  public string Guid { get; set; }
  public string VehicleType { get; set; }
  public string? Status { get; set; } // "moving" или "waiting"
}

public class Vehicle : IVehicle
{
  [JsonPropertyName("guid")]
  public required string Guid { get; set; }
  [JsonPropertyName("vehicleType")]
  public required string VehicleType { get; set; }
  [JsonPropertyName("status")]
  public string? Status { get; set; } // "moving" или "waiting"

  [SetsRequiredMembers]
  public Vehicle(VehicleOnEdge vehicle, string status)
  {
    Guid = vehicle.Guid;
    VehicleType = vehicle.VehicleType;
    Status = status;
  }

  [SetsRequiredMembers]
  public Vehicle(string guid, string vehicleType, string status)
  {
    Guid = guid;
    VehicleType = vehicleType;
    Status = status;
  }
}

public class VehicleOnEdge : IVehicle
{
  [JsonPropertyName("guid")]
  public required string Guid { get; set; }
  [JsonPropertyName("vehicleType")]
  public required string VehicleType { get; set; }
  [JsonPropertyName("status")]
  public string? Status { get; set; } // "moving" или "waiting"
  [JsonPropertyName("from")]
  public required string From { get; set; }
  [JsonPropertyName("to")]
  public required string To { get; set; }

  [SetsRequiredMembers]
  public VehicleOnEdge(Vehicle vehicle, string from, string to, string status)
  {
    Guid = vehicle.Guid;
    VehicleType = vehicle.VehicleType;
    From = from;
    To = to;
    Status = status;
  }
}