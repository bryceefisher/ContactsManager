using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.DTO;



/// <summary>
/// DTO class that is used as a return type for most of PersonService methods
/// </summary>
public class PersonResponse
{
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public int? CountryId { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }
    
    public string UserId { get; set; }

    /// <summary>
    /// Compares the current object of PersonResponse with the object passed in as param.
    /// </summary>
    /// <param name="obj">PersonResponse object to compare to.</param>
    /// <returns>Boolean indicating whether the objects matched.</returns>
    public override bool Equals(Object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(PersonResponse))
        {
            return false;
        }

        PersonResponse personToCompare = (PersonResponse)obj;

        return PersonId.Equals(personToCompare.PersonId) && PersonName == personToCompare.PersonName && Email == personToCompare.Email &&
                      Nullable.Equals(DateOfBirth, personToCompare.DateOfBirth) && Gender == personToCompare.Gender &&
                      Nullable.Equals(CountryId, personToCompare.CountryId) && Country == personToCompare.Country &&
                      Address == personToCompare.Address && ReceiveNewsLetters == personToCompare.ReceiveNewsLetters &&
                      Nullable.Equals(Age, personToCompare.Age);
    }

    public override string ToString()
    {
        return
            $"{PersonName}, {PersonId}, {Email}, {DateOfBirth}, {CountryId}, {Country}, {Address}\n";
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
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

public static class PersonExtensions
{
    /// <summary>
    /// Converts the current object of Person to PersonResponse type object.
    /// </summary>
    /// <param name="person">Current object Person to convert.</param>
    /// <returns>Returns the converted PersonResponse object.</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            Phone = person.Phone,
            DateOfBirth = person.DateOfBirth,
            CountryId = person.CountryId,
            Country = person.Country?.CountryName,
            Address = person.Address,
            Age = (person.DateOfBirth == null)
                ? null
                : Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25),
            UserId = person.UserId
        };
    }
}