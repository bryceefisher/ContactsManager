using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.DTO;

public class CountryAddRequest
{
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
    
    /// <summary>
    /// Converts CountryAddRequest object to Country object.
    /// </summary>
    /// <returns>Converted Country object.</returns>
    public Country ToCountry()
    {
        return new Country
        {
            CountryId = CountryId,
            CountryName = CountryName
        };
    }
}