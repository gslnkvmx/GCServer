using System.Text.Json.Serialization;

public class Vehicle
{
  [JsonPropertyName("guid")]
  public required string Guid { get; set; }
  [JsonPropertyName("vehicleType")]
  public required string VehicleType { get; set; }
  [JsonPropertyName("status")]
  public string? Status { get; set; } // "moving" или "waiting"
}

public class VehicleOnEdge : Vehicle
{
  [JsonPropertyName("from")]
  public required string From { get; set; }
  [JsonPropertyName("to")]
  public required string To { get; set; }
}