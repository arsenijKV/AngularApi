namespace WebApi.Models;

using System.ComponentModel.DataAnnotations;

public class AuthenticateModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

public class CreateUserModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; }
}

public class UpdateUserModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    
    public string Password { get; set; }
}








