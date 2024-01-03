
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Infrastructure.DbContext;
using RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class CountriesRepository : ICountriesRepository
{
    
    private readonly ApplicationDbContext _db;
    
    public CountriesRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<Country> AddCountry(Country country)
    {
        _db.Countries.Add(country);
       await _db.SaveChangesAsync();

       return country;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await _db.Countries.ToListAsync();
    }

    public async Task<Country> GetCountryById(int countryId)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
    }

    public async Task<Country> GetCountryByName(string countryName)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
    }
}