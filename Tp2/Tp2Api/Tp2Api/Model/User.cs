using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp2Api.Model;

[Table("users", Schema = "api")]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public string Usernameapp { get; set; } = null!;

    public string Passwordapp { get; set; } = null!;

    public string Usernamepg { get; set; } = null!;

    public string Passwordpg { get; set; } = null!;

    public bool Isadmin { get; set; }
    
    public DateTime Lastactivity { get; set; }
}
