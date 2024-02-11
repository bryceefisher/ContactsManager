using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.Domain.RepositoryContracts;

/// <summary>
/// Represents data access logic for Countries table.
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// Adds a new Country to the Countries table.
    /// </summary>
    /// <param name="country">Country object to add</param>
    /// <returns>Returns the Country object after add</returns>
    Task<Country> AddCountry(Country country);
    
    /// <summary>
    /// Returns all countries from the Countries table as a list.
    /// </summary>
    /// <returns>All Countries from the Countries table</returns>
    Task<List<Country>> GetAllCountries();
    
    
    /// <summary>
    /// Returns a Country from the Countries table by CountryId.
    /// </summary>
    /// <param name="countryId">CountryId to search by</param>
    /// <returns>Returns a Country from the Countries table by CountryId or null</returns>
    Task<Country> GetCountryById(int countryId);
    
    /// <summary>
    /// Returns a Country from the Countries table by CountryName.
    /// </summary>
    /// <param name="countryName">Country name to search by</param>
    /// <returns>Returns a Country from the Countries table by CountryName.</returns>
    Task<Country> GetCountryByName(string countryName);
}