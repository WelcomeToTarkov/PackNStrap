using System.Text.Json.Serialization;

namespace WTTPackNStrap.Models;

public class PackNStrapConfig
{
    [JsonPropertyName("loseArmbandOnDeath")] 
    public bool loseArmbandOnDeath { get; set; }
    
    [JsonPropertyName("addCasesToSecureContainers")]
    public bool addCasesToSecureContainers { get; set; }
}