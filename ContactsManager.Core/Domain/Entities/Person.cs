using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsManager.Core.Domain.Entities;

public class Person
{
    /// <summary>
    /// Domain Model for Person
    /// </summary>
    [Key]
    public int PersonId { get; set; }
    [StringLength(40)] public string? PersonName { get; set; }
    [StringLength(40)] public string? Email { get; set; }
    
    [DataType(DataType.PhoneNumber)] public string? Phone { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
   [DataType(nameof(Guid))] public int? CountryId { get; set; }
   
    [StringLength(100)] public string? Address { get; set; }
    
    [ForeignKey("CountryId")]
    public Country? Country { get; set; }
    
    public string UserId { get; set; }
}