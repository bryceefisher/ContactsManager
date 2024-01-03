using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Domain.Entities;

public class Country
{
    /// <summary>
    /// Domain Model for Country
    /// </summary>
    [Key]
    public int CountryId { get; set; }
    public string? CountryName { get; set; }

    public ICollection<Person>? People { get; set; }
}