using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Infrastructure.DbContext;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository : IPersonRepository
{
    private readonly ApplicationDbContext _db;

    public PersonsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
        _db.People.Add(person);
        await _db.SaveChangesAsync();

        return person;
    }

    public async Task<List<Person>> GetAllPersons(string userId)
    {
        return await _db.People.Include("Country").Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<Person?> GetPersonById(int personId, string userId)
    {
        return await _db.People.Include("Country")
            .FirstOrDefaultAsync(p => p.PersonId == personId && p.UserId == userId);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate, string userId)
    {
        return await _db.People
            .Include("Country")
            .Where(p => p.UserId == userId)
            .Where(predicate)
            .ToListAsync();
    }


    public async Task<bool> DeletePerson(int personId, string userId)
    {
        _db.People.RemoveRange(_db.People.Where(p => p.PersonId == personId && p.UserId == userId));
        
        int rowsAffected = await _db.SaveChangesAsync();

        return rowsAffected > 0;
    }

    public async Task<Person> UpdatePerson(Person person, string userId)
    {
        Person? personToUpdate = await _db.People.FirstOrDefaultAsync(p => p.PersonId == person.PersonId && p.UserId == userId);

        if (personToUpdate == null) return person;

        personToUpdate.PersonName = person.PersonName;
        personToUpdate.Email = person.Email;
        personToUpdate.Phone = person.Phone;
        personToUpdate.DateOfBirth = person.DateOfBirth;
        personToUpdate.CountryId = person.CountryId;
        personToUpdate.Country = person.Country;
        personToUpdate.Address = person.Address;
  

        int rowsAffected = await _db.SaveChangesAsync();

        return personToUpdate;
    }
}