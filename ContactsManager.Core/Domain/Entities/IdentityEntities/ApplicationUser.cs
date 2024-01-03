using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Core.Domain.Entities.IdentityEntities;

public class ApplicationUser : IdentityUser<string>
{
    public string PersonName { get; set; } 
}