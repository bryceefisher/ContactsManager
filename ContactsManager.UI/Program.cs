using System.Reflection;
using ContactsManager.Core.Domain.Entities.IdentityEntities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.DbContext;
using ContactsManager.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IPersonRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

// Retrieve the connection string from your configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add Entity Framework Core DbContext to the DI container


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("mysql-8.0"), mysqlOptions =>
        mysqlOptions.MigrationsAssembly("ContactsManager.Infrastructure").EnableRetryOnFailure()));

// Enable Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Configure password complexity and security settings
        options.Password.RequiredLength = 8; // Minimum password length
        options.Password.RequireNonAlphanumeric = true; // Requires non-alphanumeric character
        options.Password.RequireUppercase = true; // Requires at least one uppercase letter
        options.Password.RequireLowercase = true; // Requires at least one lowercase letter
        options.Password.RequireDigit = true; // Requires at least one digit
        options.Password.RequiredUniqueChars = 4; // Requires a number of unique characters in the password

        // Require each user to have a unique email
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Specify the database context to store user and role data
    .AddDefaultTokenProviders(); // Add providers for generating tokens for password reset, two-factor authentication, etc.

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    // Set the fallback authorization policy
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser() // Require user to be authenticated for all requests unless explicitly stated otherwise
        .Build(); // Build the policy
});

// Configure cookie settings for handling user authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    // Set the path for the login page
    // This is used when an unauthenticated user tries to access a restricted resource
    options.LoginPath = "/account/login";
});


string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(options => options.FileProviders.Add(
        new PhysicalFileProvider(appDirectory)));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//enable Https also need to run dotnet dev-certs https --trust
app.UseHsts();
app.UseHttpsRedirection();

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");

app.UseStaticFiles();

app.UseRouting();//Identify action method based on route

app.UseAuthentication();//Identify user based on cookie
app.UseAuthorization();//Validate access of the user

app.MapControllers();//Execute the filter pipeline (action + filters)

app.Run();