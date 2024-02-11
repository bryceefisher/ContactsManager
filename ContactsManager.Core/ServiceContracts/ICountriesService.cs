using ContactsManager.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// Represents buisness logic for manipulating country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the list of countries.
    /// </summary>
    /// <param name="request">CountryAddRequest object to add to list.</param>
    /// <returns>Returns the CountryResponse object after adding it to the list, including newly generated GUID</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? request);

    /// <summary>
    /// Returns all countries from the list
    /// </summary>
    /// <returns>All countries from the list as List<CountryResponse></returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Returns a CountryResponse object based on provided countryId
    /// </summary>
    /// <param name="countryId">The GUID used to search for CountryResponse to return.</param>
    /// <returns>Matching CountryResponse object.</returns>
    Task<CountryResponse>? GetCountryByCountryId(int? countryId);
    
    /// <summary>
    /// Uploads a list of countries from a file.
    /// </summary>
    /// <param name="file">Excel file with a list of people</param>
    /// <returns>Uploads a list of countries from a file.</returns>
    Task<int> CountryUploadFromFile(IFormFile file);
}