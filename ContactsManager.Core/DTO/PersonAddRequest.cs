using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using ContactsManager.Core.Domain.Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// Acts as a DTO for inserting a new Person
/// </summary>
public class PersonAddRequest
{
    public int PersonId { get; set; }
    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Must supply a valid email address")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; }

    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
    
    public int? CountryId { get; set; }
    public string? Address { get; set; }
    
    public string? UserId { get; set; }

    /// <summary>
    /// Converts the current object of PersonAddRequest to Person type object.
    /// </summary>
    /// <returns>Converted Person object.</returns>
    public Person ToPerson()
    {
        return new Person()
        {
            PersonId = PersonId,
            PersonName = PersonName,
            Email = Email,
            Phone = Phone,
            DateOfBirth = DateOfBirth,
            CountryId = CountryId,
            Address = Address,
            UserId = UserId
        };
    }
}