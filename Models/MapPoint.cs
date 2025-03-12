using System.Text.Json.Serialization;

public interface IMapPoint
{
  public string Name { get; set; }
  public int Capacity { get; set; }
  public string Type { get; set; }
  public List<IVehicle> Vehicles { get; set; }

  public string GetChars()
  {
    return Name.Split('-')[0];
  }

  public int GetFirstNum()
  {
    return Name.Split('-')[1][0] - '0';
  }
}