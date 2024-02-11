using AutoFixture;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.DbContext;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace ContactsManager.ServiceTests;

public class PersonServiceTest
{
    // private fields
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;
    
    private readonly Mock<IPersonRepository> _mockPersonRepository;
    
    private readonly IPersonRepository _personRepository;
    
    private readonly ITestOutputHelper _testHelper;
    private readonly IFixture _fixture;


    // constructor
    public PersonServiceTest(ITestOutputHelper testHelper)
    {
        _fixture = new Fixture();
        _mockPersonRepository = new Mock<IPersonRepository>();
        _personRepository = _mockPersonRepository.Object;
        
        
        var countriesInitialData = new List<Country>() { };
        var personInitialData = new List<Person>() { };


        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.People, personInitialData);

        _countriesService = new CountriesService(null);
        _personService = new PersonService(_personRepository);
    }

    #region HelperMethods

    private async Task<List<CountryResponse>> AddCountries()
    {
        CountryAddRequest country1 = new CountryAddRequest()
        {
            CountryId = 0,
            CountryName = "USA"
        };
        CountryAddRequest country2 = new CountryAddRequest()
        {
            CountryId = 1,
            CountryName = "Japan"
        };
        CountryAddRequest country3 = new CountryAddRequest()
        {
            CountryId = 2,
            CountryName = "Norway"
        };

        List<CountryResponse> response = new List<CountryResponse>()
        {
            await _countriesService.AddCountry(country1),
            await _countriesService.AddCountry(country2),
            await _countriesService.AddCountry(country3)
        };

        return response;
    }

    /// <summary>
    /// Adds 3 countries and three people to respective lists
    /// </summary>
    /// <returns>List of type person response</returns>
    private async Task<List<PersonResponse>> AddPeople()
    {
        List<CountryResponse> response = await AddCountries();


        PersonAddRequest person1 = new PersonAddRequest()
        {
            PersonId = 0,
            PersonName = "John Doe",
            Email = "johndoe@example.com",
            DateOfBirth = new DateTime(1990, 5, 15),
            CountryId = response[0].CountryID,
            Address = "123 Main Street, City",
        };


        PersonAddRequest person2 = new PersonAddRequest()
        {
            PersonId = 1,
            PersonName = "Jane Smith",
            Email = "janesmith@example.com",
            DateOfBirth = new DateTime(1985, 8, 25),
            CountryId = response[1].CountryID,
            Address = "456 Elm Street, Town",
        };

        PersonAddRequest person3 = new PersonAddRequest()
        {
            PersonId = 2,
            PersonName = "Bob Johnson",
            Email = "bobjohnson@example.com",
            DateOfBirth = new DateTime(1975, 3, 10),
            CountryId = response[2].CountryID,
            Address = "789 Oak Street, Village",
          
        };

        PersonResponse response1 = await _personService.AddPerson(person1);
        PersonResponse response2 = await _personService.AddPerson(person2);
        PersonResponse response3 = await _personService.AddPerson(person3);

        List<PersonResponse> responses = new List<PersonResponse>()
        {
            response1,
            response2,
            response3
        };

        return responses;
    }

    #endregion

    #region AddPerson

    //When supplied null value as PersonAddReq it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest? personAddRequest = null;

        //Act
        // await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPerson(personAddRequest));
        Func<Task> action = async () => { await _personService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    //Same as above but PersonName is null
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        //Arrange
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        //Act
        Func<Task> action = async () => { await _personService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    //Supply proper person details pass and return PersonResponse
    [Fact]
    public async Task AddPerson_ValidPerson_Success()
    {
        //Arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@email.com")
            .Create();

        Person person = personAddRequest.ToPerson();
        PersonResponse personToCompare = person.ToPersonResponse();
        personToCompare.PersonId = 1;
        _mockPersonRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        //Act
        PersonResponse personResponse =
            await _personService.AddPerson(personAddRequest);
        personResponse.PersonId = 1;
        

        // Assert.True(personResponse.PersonId != 0);
        personResponse.PersonId.Should().NotBe(0);
        personToCompare.Should().BeEquivalentTo(personResponse);
    }

    #endregion

    #region GetPerson

    //Check if there is no people in the list
    [Fact]
    public async Task GetAllPeople_EmptyList()
    {
        List<PersonResponse> people = await _personService.GetAllPeople();

        people.Should().BeEmpty();
    }

    //check that if the list is valid it properly returns the list
    [Fact]
    public async Task GetAllPeople_ValidList()
    {
        CountryAddRequest country1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country2 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country3 = _fixture.Create<CountryAddRequest>();

        CountryResponse countryResponse1 = await _countriesService.AddCountry(country1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(country2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(country3);

        PersonAddRequest person1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@email.com")
            .Create();


        PersonAddRequest person2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email1@email.com")
            .Create();

        PersonAddRequest person3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email2@email.com")
            .Create();

        PersonResponse response1 = await _personService.AddPerson(person1);
        PersonResponse response2 = await _personService.AddPerson(person2);
        PersonResponse response3 = await _personService.AddPerson(person3);


        List<PersonResponse> responses = new List<PersonResponse>()
        {
            response1,
            response2,
            response3
        };

        var personList = await _personService.GetAllPeople();

        responses.Should().BeEquivalentTo(personList);
    }

    //check if returns a valid person matching the supplied personid
    [Fact]
    public async Task GetPersonByPersonId_ValidId()
    {
        CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse response = await _countriesService.AddCountry(countryRequest);

        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "example@email.com")
            .Create();

        PersonResponse personId = await _personService.AddPerson(personAddRequest);

        PersonResponse? toCompare = await _personService.GetPersonByPersonId(personId.PersonId);

        // Assert.Equal(personId, toCompare);
        personId.Should().BeEquivalentTo(toCompare);
    }

    [Fact]
    public async Task GetPersonByPersonId_NullId()
    {
        int? nullId = null;

        // Assert.Null(await _personService.GetPersonByPersonId(nullId)!);
        var nullPerson = await _personService.GetPersonByPersonId(nullId )!;

        nullPerson.Should().BeNull();
    }

    #endregion

    #region GetFilteredPerson

    //If the search text is empty and search by is "personName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptyString()
    {
        CountryAddRequest country1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country2 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country3 = _fixture.Create<CountryAddRequest>();

        CountryResponse countryResponse1 = await _countriesService.AddCountry(country1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(country2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(country3);

        PersonAddRequest person1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@email.com")
            .Create();


        PersonAddRequest person2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email1@email.com")
            .Create();

        PersonAddRequest person3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email2@email.com")
            .Create();

        PersonResponse response1 = await _personService.AddPerson(person1);
        PersonResponse response2 = await _personService.AddPerson(person2);
        PersonResponse response3 = await _personService.AddPerson(person3);


        List<PersonResponse> responses = new List<PersonResponse>();

        responses.Add(response1);
        responses.Add(response2);
        responses.Add(response3);


        List<PersonResponse> filteredPersons =
            await _personService.GetFilteredPersons(nameof(person1.PersonName), null);

        filteredPersons.Should().BeEquivalentTo(responses);
    }

    //Add people to the list, search based on person name with a search string
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        CountryAddRequest country1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country2 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country3 = _fixture.Create<CountryAddRequest>();

        CountryResponse countryResponse1 = await _countriesService.AddCountry(country1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(country2);
        CountryResponse countryResponse3 = await _countriesService.AddCountry(country3);

        PersonAddRequest person1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@email.com")
            .With(temp => temp.PersonName, "Bob")
            .Create();


        PersonAddRequest person2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email1@email.com")
            .Create();

        PersonAddRequest person3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "email2@email.com")
            .Create();

        PersonResponse response1 = await _personService.AddPerson(person1);
        PersonResponse response2 = await _personService.AddPerson(person2);
        PersonResponse response3 = await _personService.AddPerson(person3);


        List<PersonResponse> responses = new List<PersonResponse>();

        responses.Add(response1);
        responses.Add(response2);
        responses.Add(response3);


        List<PersonResponse> filteredPersons =
            await _personService.GetFilteredPersons(nameof(person1.PersonName), "bo");


        filteredPersons.Should()
            .OnlyContain(temp => temp.PersonName.Contains("bo", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region SortedPeople

    //When sorting based on personName in Desc, return people in desc order
    [Fact]
    public async Task GetSortedPeople()
    {
        List<PersonResponse> responses = await AddPeople();


        List<PersonResponse> allPeople = await _personService.GetAllPeople();
        List<PersonResponse> sortedPersons =
            _personService.GetSortedPeople(allPeople, nameof(Person.PersonName), SortOrderEnum.DESC);


        // responses = responses.OrderByDescending(p => p.PersonName).ToList();
        //
        // //Assert
        // Assert.Equal(responses, sortedPersons);

        sortedPersons.Should().BeInDescendingOrder(temp => temp.PersonName);
    }

    #endregion

    #region UpdatePerson

    //When we supply nill as PersonUpdateRequest, it should throw Argument null exception
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        //Arrange
        PersonUpdateRequest? personUpdateRequest = null;


        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(personUpdateRequest);
        };

       await  action.Should().ThrowAsync<ArgumentException>();

    }

    //When person id is invalid throw Argument exception
    [Fact]
    public async Task UpdatePerson_InvalidPerson()
    {
        //Arrange
        PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();


        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(personUpdateRequest);
        };
        
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When PersonName is null, throw argumentexception
    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
        //Arrange
        List<PersonResponse> people = await AddPeople();
        PersonUpdateRequest? personUpdateRequest = people[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = null;

        //Assert
        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(personUpdateRequest);
        };

       await action.Should().ThrowAsync<ArgumentException>();
    }

    //First add new person then try to update that person's name and email
    [Fact]
    public async Task UpdatePerson_ValidName()
    {
        //Arrange
        List<PersonResponse> people = await AddPeople();
        PersonUpdateRequest? personUpdateRequest = people[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = "William";
        personUpdateRequest.Email = "william@example.com";


        //Act
        PersonResponse updatedPerson = await _personService.UpdatePerson(personUpdateRequest);

        PersonResponse? getPerson = await _personService.GetPersonByPersonId(updatedPerson.PersonId);


        //Assert
        getPerson.Should().BeEquivalentTo(updatedPerson);
    }

    #endregion

    #region DeletePerson

    // If supply valid id should return true

    [Fact]
    public async Task DeletePerson_validPersonId()
    {
        //Arrange
        List<PersonResponse> addedPeople = await AddPeople();
        PersonResponse toDelete = addedPeople[0];

        bool isDeleted = await _personService.DeletePerson(toDelete.PersonId);
        //Act
        isDeleted.Should().BeTrue();
    }

    // If supply invalid id, should return false

    [Fact]
    public async Task DeletePerson_InvalidPersonId()
    {
        //Arrange
        List<PersonResponse> addedPeople = await AddPeople();
        PersonResponse toDelete = addedPeople[0];
        toDelete.PersonId = 1;

        bool isDeleted = false;
        isDeleted = await _personService.DeletePerson(4);
        //Act
        isDeleted.Should().BeFalse();
    }

    #endregion
}