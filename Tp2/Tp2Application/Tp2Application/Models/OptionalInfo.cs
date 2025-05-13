using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Tp2Application.Model; 

public class OptionalInfo {
    public List<string> categorie { get; set; }
    public float prix { get; set; }
    public OptionalDevInfo developpement { get; set; } = new();
    public string evaluation { get; set; }
    public string platforme { get; set; }
    
    public string manette { get; set; }
    
    public bool complet { get; set; }
    
    [JsonIgnore]
    public string CompletTranslate {
        get => complet ? "Oui" : "Non";
    }

    [JsonIgnore]
    public string CompletEntry {
        get;
        set;
    }
    
    [JsonIgnore]
    public string CategorieFormatted {
        get {
            if (categorie == null) {
                return "";
            }
            return string.Join(", ", categorie);
        }
        set => categorie = value.Split(", ").ToList();
    }
}