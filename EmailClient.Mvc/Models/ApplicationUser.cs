using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EmailClient.Mvc.Models;

/// <summary>
/// User model for Microsoft Identity authorization and authentication
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string RecoveryEmail { get; set; } = string.Empty;
}
