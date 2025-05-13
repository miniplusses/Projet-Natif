using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Tp2Application.Model; 

public class OptionalDevInfo {
    public string parution { get; set; }
    public string developpeur { get; set; }
    public string editeur { get; set; }
    public string franchise { get; set; }
    
    [JsonIgnore]
    public string parutionFormatted {
        get => parution;
        set {
            if (value.Length == 10 && !Regex.IsMatch(value, @"\d{4}-\d{2}-\d{2}")) {
                parution = "";
            } else {
                parution = value;
            }
        }
    }
}