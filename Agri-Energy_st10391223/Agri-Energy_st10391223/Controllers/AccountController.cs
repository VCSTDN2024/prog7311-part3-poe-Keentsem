using Agri_Energy_st10391223.Data;
using Agri_Energy_st10391223.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Agri_Energy_st10391223.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Always initialize the model to avoid NullReferenceException
            var model = new LoginViewModel { IsEmployeeLogin = false };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        if (user != null)
                        {
                            if (await _userManager.IsInRoleAsync(user, "Farmer"))
                            {
                                return RedirectToAction("Dashboard", "Farmer");
                            }
                            else if (await _userManager.IsInRoleAsync(user, "Employee"))
                            {
                                return RedirectToAction("Dashboard", "Employee");
                            }
                        }
                    }
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    try
                    {
                        // By default, register as a Farmer
                        await _userManager.AddToRoleAsync(user, "Farmer");
                        
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        
                        // Create a default Farmer profile to prevent crashes
                        var farmer = new Farmer
                        {
                            UserId = user.Id,
                            Name = model.FirstName,
                            Surname = model.LastName,
                            Email = model.Email,
                            // Set some default values for required fields
                            PhoneNumber = "Not Set",
                            FarmName = "Not Set",
                            Region = "Not Set"
                        };
                        
                        _context.Farmers.Add(farmer);
                        await _context.SaveChangesAsync();
                        
                        return RedirectToAction("Dashboard", "Farmer");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        Console.WriteLine($"Error during registration: {ex.Message}");
                        ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                        return View(model);
                    }
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult EmployeeLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewModel();
            model.IsEmployeeLogin = true;
            return View("Login", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    
                    // Check if user exists and is an employee
                    if (user != null && await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        // Redirect to employee dashboard
                        return RedirectToAction("Dashboard", "Employee");
                    }
                    else
                    {
                        // Sign out if user doesn't exist or is not an employee
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "You are not authorized as an Employee.");
                        model.IsEmployeeLogin = true;
                        return View("Login", model);
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            
            model.IsEmployeeLogin = true;
            return View("Login", model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
