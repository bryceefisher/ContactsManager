using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.DbContext;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
namespace ContactsManager.ServiceTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    

    //constructor
    public CountriesServiceTest()
    {
        var countriesInitialData = new List<Country>() {};
        
      
      DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
          new DbContextOptionsBuilder<ApplicationDbContext>().Options);
      
      ApplicationDbContext dbContext =  dbContextMock.Object;
      dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
      
      _countriesService = new CountriesService(null);
      
    }


    #region AddCountry

    //when null is passed as a parameter, the method should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        //arrange
        CountryAddRequest request = null;

        //Assert
       await Assert.ThrowsAsync<ArgumentNullException>(async() => await _countriesService.AddCountry(request));
    }

    //when country name is null, the method should throw ArgumentException

    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        //arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(request));
    }

    //when country name is duplicated, the method should throw ArgumentException
    
    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        //arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        });
    }

    //when country name is valid, the method should insert the country into the list and return the country object with
    //generated GUID
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        //arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        //Act 
        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countriesFromGetAllCountries = await  _countriesService.GetAllCountries();

        //Assert

        // Assert.True(response.CountryID != 0);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion

    #region GetAllCountries

    [Fact]
    //list of countries should be empty by default
    public async Task GetAllCountries_EmptyList()
    {
        //Acts
        List<CountryResponse> actualCountries = await _countriesService.GetAllCountries();

        //Assert

        Assert.Empty(actualCountries);
    }

    //should return all countries in the list
    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        //Arrange
        List<CountryAddRequest> countryRequests = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { CountryName = "USA" },
            new CountryAddRequest() { CountryName = "Japan" },
            new CountryAddRequest() { CountryName = "Canada" }
        };

        //Act
        List<CountryResponse> countries = new List<CountryResponse>();

        int id = 0;
        foreach (CountryAddRequest request in countryRequests)
        {
            request.CountryId = id;
            countries.Add(await _countriesService.AddCountry(request));
            id++;
        }

        List<CountryResponse> countryResponse = await _countriesService.GetAllCountries();
        
        //read each element
         foreach (var expectedCountry in countryResponse)
         {
             Assert.Contains(expectedCountry, countryResponse);
         }
    }

    #endregion

    #region GetCountryById

    //check if returns null if supplied id is null
    [Fact]
    public async Task GetCountryById_NullId()
    {
        CountryResponse? nullResponse = await _countriesService.GetCountryByCountryId(null);
        
        Assert.Null(nullResponse);
    }

    //should return countryResponse object if id is valid
    [Fact]
    public async Task GetCountryById_ValidId()
    {
        
        //Arrange
        List<CountryAddRequest> countryRequests = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { CountryName = "USA" },
            new CountryAddRequest() { CountryName = "Japan" },
            new CountryAddRequest() { CountryName = "Canada" }
        };

        //Act
        List<CountryResponse> countries = new List<CountryResponse>();

        int id = 0;
        foreach (CountryAddRequest request in countryRequests)
        {
            request.CountryId = id;
            countries.Add(await _countriesService.AddCountry(request));
            id++;
        }

        CountryResponse expectedCountry = countries.FirstOrDefault();

        CountryResponse? validCountry = await _countriesService.GetCountryByCountryId(countries.FirstOrDefault().CountryID);

        Assert.Equal(validCountry, expectedCountry);
        

    }

    #endregion
}