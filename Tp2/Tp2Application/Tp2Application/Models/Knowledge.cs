namespace Tp2Application.Model; 

public class Knowledge {
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public OptionalInfo data { get; set; } = new OptionalInfo();
}