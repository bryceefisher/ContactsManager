using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Domain.Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents the DTO class that contains the person details to update.
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "PersonId cannot be blank")]
    public int PersonId { get; set; }

    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Person Name can't be blank")]
    [EmailAddress(ErrorMessage = "Must supply a valid email address")]
    public string? Email { get; set; }

    public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? CountryId { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
    
    /// <summary>
    /// Converts the current object of PersonUpdateRequest to Person type object.
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