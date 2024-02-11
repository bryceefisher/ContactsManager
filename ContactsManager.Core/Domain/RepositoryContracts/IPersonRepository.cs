using System.Linq.Expressions;
using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.Domain.RepositoryContracts;

/// <summary>
/// Represents data access logic for Person table.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Adds a new Person to the Person table.
    /// </summary>
    /// <param name="person">Person object to add</param>
    /// <returns>Returns the Person object after add</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// Returns all Persons from the Person table as a list.
    /// </summary>
    /// <returns>All Persons from the Person table</returns>
    Task<List<Person>> GetAllPersons(string userId);

    /// <summary>
    /// Returns a Person from the Person table by PersonId.
    /// </summary>
    /// <param name="personId">Person id to search by</param>
    /// <param name="userId"></param>
    /// <returns>Returns a Person from the Person table by PersonId.</returns>
    Task<Person?> GetPersonById(int personId, string userId);

    /// <summary>
    /// Returns all Persons from the Person table based on given expression
    /// </summary>
    /// <param name="predicate">Linq Expression to check</param>
    /// <param name="userId"></param>
    /// <returns>All persons matching condition</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate, string userId);

    /// <summary>
    /// Deletes a Person from the Person table by PersonId
    /// </summary>
    /// <param name="personId">PersonId of person object to delete</param>
    /// <param name="userId"></param>
    /// <returns>bool value representing is the deletion was successful</returns>
    Task<bool> DeletePerson(int personId, string userId);


    /// <summary>
    /// Updates person details in the Person table. Returns updated person object.
    /// </summary>
    /// <param name="person">Person object to update</param>
    /// <param name="userId"></param>
    /// <returns>Returns updated person object.</returns>
    Task<Person> UpdatePerson(Person person, string userId);

}