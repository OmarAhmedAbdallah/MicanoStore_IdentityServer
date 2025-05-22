using Microsoft.AspNetCore.Identity;

namespace SharedLibrary.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
} 