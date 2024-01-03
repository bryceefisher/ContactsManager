using ContactsManager.Core.Domain.Entities.IdentityEntities;
using ContactsManager.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;

namespace ContactsManager.UI.Controllers;

[Route("[Controller]/[Action]")]
[AllowAnonymous] // Allow access to this controller's actions without requiring authentication
public class AccountController : Controller
{
    // Dependency injection of UserManager and SignInManager
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    // Constructor injecting dependencies
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // GET action for registering a new user
    public IActionResult Register()
    {
        return View(); // Return the registration view
    }

    // POST action for processing the registration form
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        // Validate the model state
        if (ModelState.IsValid)
        {
            // Create a new ApplicationUser object
            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(), // Generate a new GUID for the user ID
                PersonName = registerDto.PersonName, // Set the name from the DTO
                UserName = registerDto.Email, // Set the username to the email from the DTO
                PhoneNumber = registerDto.Phone, // Set the phone number from the DTO
                Email = registerDto.Email // Set the email from the DTO
            };

            // Attempt to create the user with the provided password
            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

            // Check if the user creation was successful
            if (result.Succeeded)
            {
                //check if the user is admin
                if (registerDto.UserType == UserTypeOptions.Admin)
                {
                    //check if the admin role exists
                    if (!await _roleManager.RoleExistsAsync(UserTypeOptions.Admin.ToString()))
                    {
                        //create the admin role
                        await _roleManager.CreateAsync(new ApplicationRole()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = UserTypeOptions.Admin.ToString()
                        });
                        //add the admin role to the user
                        await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                    }
                    
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                    // Sign in the newly created user
                    await _signInManager.SignInAsync(user,
                        false); // Second parameter is for 'Remember Me' functionality
                    // Redirect to the Person Index page
                    return RedirectToAction("Index", "Person");
                }

                // Sign in the newly created user
                await _signInManager.SignInAsync(user,
                    false); // Second parameter is for 'Remember Me' functionality
                // Redirect to the Person Index page
                return RedirectToAction("Index", "Person");
            }

            // Add errors to ModelState and return to the registration view
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return View(registerDto);
        }

        // If ModelState is not valid, return to the view with validation errors
        return View(registerDto);
    }


// Action for logging out the user
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync(); // Sign out the current user
        return RedirectToAction("Index", "Person"); // Redirect to the Person Index page
    }

    // GET action for login
    public IActionResult Login()
    {
        return View(); // Return the login view
    }

    // POST action for processing the login form
    //Return Url (case sensitive) for redirecting to the page from where the user was redirected to login page
    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO loginDto, string? ReturnUrl)
    {
        // Validate the model stateUserLogins
        if (!ModelState.IsValid)
            return View(loginDto);

        // Attempt to sign in the user
        var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,
            false, false); // Parameters are for 'Remember Me' and 'lockoutOnFailure'

        // Check if the sign-in was successful
        if (result.Succeeded)
        {
            //Admin check
            ApplicationUser user = await _userManager.FindByEmailAsync(loginDto.Email);
            //If the user is not null
            if (user != null)
            {
                //Check if the user is admin / Returns true if the specified user is in the named role.
                if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                {
                    // Redirect to the Admin Index page
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }

            // Check if the ReturnUrl is not null or empty and is a local URL
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                // Redirect to the ReturnUrl
                return LocalRedirect(ReturnUrl);
            }

            // Redirect to the Person Index page if the ReturnUrl is null or empty or not a local URL
            return RedirectToAction("Index", "Person"); // Redirect to the Person Index page
        }


        // Add error to ModelState and return to the login view
        ModelState.AddModelError("Login", "Invalid Email or Password");

        return View(loginDto);
    }

    [AllowAnonymous]
    //async method for checking if the email is unique
    public async Task<IActionResult> UniqueEmail(string email)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return Json(false); //already exists
        }
        else
        {
            return Json(true); //valid
        }
    }
}