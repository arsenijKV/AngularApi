namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    
    [JsonIgnore] // чтобы пароль/хэш не утекал в JSON-ответах
    public string Password { get; set; } = string.Empty;
}
