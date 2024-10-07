using System.Text.Json.Serialization;  

namespace api_process_mp_pdfs.Models;
  
public class Location  
{  
    [JsonPropertyName("latitude")]  
    public double? Latitude { get; set; }  
  
    [JsonPropertyName("longitude")]  
    public double? Longitude { get; set; }  
}  
