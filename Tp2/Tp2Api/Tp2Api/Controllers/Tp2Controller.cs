using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tp2Api.Model;
using static Tp2Api.Model.Security;

namespace Tp2Api.Controllers;

[ApiController]
[Route("[controller]")]
public class Tp2Controller : ControllerBase {
    private Tp2Context dbContext = new();
    private readonly string secretKey = "A0Eb65Fc3CDeAbe3C2a047d6FE52h15B";

    [HttpPost]
    public bool Post(UserInfo info) {  
        if (info.username == "" || info.password == "") {
            return false;
        }
        
        var passwordpg = GenerateRandomString(256);

        var passwordAppHashed = BCrypt.Net.BCrypt.HashPassword(info.password);
        var passwordPgCrypted = EncryptString(passwordpg, secretKey);

        var achive = from b in dbContext.Users
            select dbContext.InsertUsers(info.username, passwordAppHashed, passwordPgCrypted, passwordpg);
        
        return achive.FirstOrDefault();
    }
    
    [HttpPut]
    public string Put(UserInfo info) {
        
        if (info.username == "" || info.password == "") {
            return null;
        }
        
        var user = dbContext.Users.SingleOrDefault(u => u.Usernameapp == info.username);
        
        if (user == null || BCrypt.Net.BCrypt.Verify(info.password, user.Passwordapp) == false) {
            return null;
        }
        var passwordPg = DecryptString(user.Passwordpg, secretKey);
        
        user.Lastactivity = DateTime.Now;
        dbContext.SaveChanges();
        
        return $"Host=localhost;Database=tp2;Username={user.Usernamepg};Password={passwordPg}";
    }
    
    [HttpPut("{id:guid}")]
    public IActionResult Put(Guid id, [FromBody] UserInfo idAdmin) {
        var admin = dbContext.Users.SingleOrDefault(u => u.Usernamepg == idAdmin.username);
        
        if (admin is not { Isadmin: true }) {
            return BadRequest();
        }
        
        
        var user = dbContext.Users.SingleOrDefault(u => u.Id == id);
        
        if (user == null) {
            return BadRequest();
        }
        
        user.Isadmin = true;
        dbContext.SaveChanges();
        
        return Ok();
    }
    
    [HttpDelete]
    public IActionResult Delete(UserInfo userInfo) {
        var admin = dbContext.Users.SingleOrDefault(u => u.Usernamepg == userInfo.username);
        
        if (admin is not { Isadmin: true }) {
            return BadRequest();
        }
        
        var listInactif = dbContext.Users.Where(u => u.Lastactivity < DateTime.Today - TimeSpan.FromDays(7) && u.Isadmin == false).ToList();
        
        foreach (var user in listInactif) {
            dbContext.Users.Remove(user);
        }
        
        dbContext.SaveChanges();
        
        return Ok();
    }
    
    [HttpPost, Route("regulier")]
    public List<User> GetRegular([FromBody] UserInfo user) {
        var admin = dbContext.Users.SingleOrDefault(u => u.Usernamepg == user.username);
        
        if (admin is not { Isadmin: true }) {
            return null;
        }
        
        return dbContext.Users.Where(u => u.Isadmin == false).ToList();
    }
    
    [HttpPost, Route("inactif")]
    public List<User> GetInactive([FromBody] UserInfo user) {
        var admin = dbContext.Users.SingleOrDefault(u => u.Usernamepg == user.username);
        
        if (admin is not { Isadmin: true }) {
            return null;
        }
        
        return dbContext.Users.Where(u => u.Lastactivity < DateTime.Today - TimeSpan.FromDays(7) && u.Isadmin == false).ToList();
    }
    
    [HttpPost, Route("isAdmin")]
    public bool isAdmin([FromBody] UserInfo user) {
        var admin = dbContext.Users.SingleOrDefault(u => u.Usernamepg == user.username);
        
        if (admin is not { Isadmin: true }) {
            return false;
        }
        return true;
    }
    
}