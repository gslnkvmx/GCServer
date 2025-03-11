using System.Text.Json.Serialization;

public class MapPoint
{
  [JsonPropertyName("name")]
  public required string Name { get; set; }
  [JsonPropertyName("capacity")]
  public int Capacity { get; set; }
  [JsonPropertyName("type")]
  public required string Type { get; set; }
  [JsonPropertyName("vehicles")]
  public List<Vehicle> Vehicles { get; set; } = new();

  public string GetChars()
  {
    return Name.Split('-')[0];
  }

  public string GetNum()
  {
    return Name.Split('-')[1];
  }
}