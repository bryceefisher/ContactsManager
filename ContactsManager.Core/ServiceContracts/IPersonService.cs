using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Person entity
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Adds a Person object to the list of People
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Returns the Person object after adding it to the list, including newly generated GUID</returns>
    Task<PersonResponse> AddPerson(PersonAddRequest? request);

    /// <summary>
    /// Returns all people from the list
    /// </summary>
    /// <returns>All people from the list as List of PeopleResponse</returns>
    Task<List<PersonResponse>> GetAllPeople(string userId);

    /// <summary>
    /// Returns a PersonResponse objects matching supplied PersonId.
    /// </summary>
    /// <param name="personId"></param>
    /// <returns>PersonResponse object matching supplied param Id.</returns>
    Task<PersonResponse?> GetPersonByPersonId(int? personId, string userId);
    
    /// <summary>
    /// Returns all person objects that match with the given search field and search string
    /// </summary>
    /// <param name="searchBy">Field to search</param>
    /// <param name="searchString">String to search for with searchBy field</param>
    /// <returns>Returns all PersonResponse objects that meet the search criteria</returns>
    //Get filtered Persons
    Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString, string userId);

    /// <summary>dot
    /// Returns sorted list of people.
    /// </summary>
    /// <param name="allPeople">List of people to sort.</param>
    /// <param name="sortBy">Name of the property (key) in which to sort by.</param>
    /// <param name="sortOrder">ASC or DESC</param>
    /// <returns>Returns the person response object</returns>
    List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string? sortBy, SortOrderEnum sortOrder);

    /// <summary>
    /// Updates the specified person details based on given person ID
    /// </summary>
    /// <param name="personUpdateRequest"></param>
    /// <returns></returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest, string userId);

    /// <summary>
    /// Deletes a person based on the given person id
    /// </summary>
    /// <param name="personId"></param>
    /// <param name="userId"></param>
    /// <returns>Returns true, if the deletion is a success</returns>
    Task<bool> DeletePerson(int? personId, string userId);

    /// <summary>
    /// Returns People as a CSV file
    /// </summary>
    /// <returns>People as a CSV</returns>
    Task<MemoryStream> GetPeopleCSV(string userId);

    /// <summary>
    /// Returns people as Excel fil
    /// </summary>
    /// <returns>Returns people as Excel fil</returns>
    Task<MemoryStream> GetPeopleExcel(string userId);


}