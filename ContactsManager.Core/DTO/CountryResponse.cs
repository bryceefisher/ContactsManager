using ContactsManager.Core.Domain.Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that is used as a return type for most of CountriesService methods
/// </summary>
public class CountryResponse
{
    public int CountryID { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(Object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(CountryResponse))
        {
            return false;
        }

        CountryResponse countryToCompare = (CountryResponse)obj;

        if (this.CountryName != countryToCompare.CountryName || this.CountryID != countryToCompare.CountryID)
        {
            return false;
        }

        return true;
    }
    
    public override string ToString()
    {

        return $"{CountryName}, {CountryID} \n";

    }
}

public static class CountryExtensions
{
    /// <summary>
    /// Converts the current Country object into a CountryResponse object.
    /// </summary>
    /// <param name="country">The current Country object to be converted.</param>
    /// <returns>Converted CountryResponse object</returns>
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryID = country.CountryId,
            CountryName = country.CountryName
        };
    }
}