using Agri_Energy_st10391223.Data;
using Agri_Energy_st10391223.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Agri_Energy_st10391223.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var farmers = await _context.Farmers
                    .OrderBy(f => f.Name)
                    .ToListAsync();

                // Even if no farmers are found, return an empty list instead of null
                return View(farmers ?? new List<Farmer>());
            }
            catch (Exception)
            {
                // If there's an error, return an empty list to avoid null reference exceptions
                return View(new List<Farmer>());
            }
        }
        
        // GET: Employee
        // Note: Using simple version that just displays employee dashboard
        public IActionResult Employee()
        {
            // Redirect to Dashboard view which is already working
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Employee/FarmerDetails/5
        public async Task<IActionResult> FarmerDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(p => p.FarmerId == farmer.Id)
                .OrderByDescending(p => p.ProductionDate)
                .ToListAsync();

            ViewData["Farmer"] = farmer;
            return View(products);
        }

        // GET: Employee/AddFarmer
        public IActionResult AddFarmer()
        {
            return View();
        }

        // POST: Employee/AddFarmer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer([Bind("Name,Surname,Email,PhoneNumber,FarmName,Region")] Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                // Generate a random password for the farmer user
                var password = GenerateRandomPassword();

                // Create the user with the given email
                var user = new ApplicationUser
                {
                    UserName = farmer.Email,
                    Email = farmer.Email,
                    EmailConfirmed = true,
                    FirstName = farmer.Name,
                    LastName = farmer.Surname
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Add the user to the Farmer role
                    await _userManager.AddToRoleAsync(user, "Farmer");

                    // Set the UserId for the farmer
                    farmer.UserId = user.Id;

                    // Save the farmer
                    _context.Add(farmer);
                    await _context.SaveChangesAsync();

                    // Store the password temporarily to display to the employee
                    TempData["FarmerPassword"] = password;
                    TempData["FarmerEmail"] = farmer.Email;

                    return RedirectToAction(nameof(FarmerCreated));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(farmer);
        }

        // GET: Employee/FarmerCreated
        public IActionResult FarmerCreated()
        {
            // Display the created farmer's credentials
            return View();
        }

        // GET: Employee/AllProducts
        public async Task<IActionResult> AllProducts(string category = null, string farmerName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Start with all products
            var productsQuery = _context.Products.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            if (startDate.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductionDate <= endDate.Value);
            }

            // Include farmer details and apply farmer name filter if provided
            var products = await productsQuery
                .Include(p => p.Farmer)
                .OrderByDescending(p => p.ProductionDate)
                .ToListAsync();

            if (!string.IsNullOrEmpty(farmerName))
            {
                products = products.Where(p => 
                    p.Farmer.Name.Contains(farmerName, StringComparison.OrdinalIgnoreCase) || 
                    p.Farmer.Surname.Contains(farmerName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Get available categories for filter dropdown
            ViewData["Categories"] = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            // Get all farmers for filter dropdown
            ViewData["Farmers"] = await _context.Farmers
                .OrderBy(f => f.Name)
                .ToListAsync();

            // Set filter values for form
            ViewData["SelectedCategory"] = category;
            ViewData["SelectedFarmerName"] = farmerName;
            ViewData["SelectedStartDate"] = startDate;
            ViewData["SelectedEndDate"] = endDate;

            return View(products);
        }

        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.Id == id);
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password that meets the requirements
            const string upperCase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijkmnopqrstuvwxyz";
            const string numeric = "0123456789";
            const string special = "!@#$%^&*()_-+=[{]};:>|./?";

            // Ensure at least one of each required character type
            var password = new StringBuilder();
            var random = new Random();

            // Add one uppercase letter
            password.Append(upperCase[random.Next(upperCase.Length)]);

            // Add one lowercase letter
            password.Append(lowerCase[random.Next(lowerCase.Length)]);

            // Add one number
            password.Append(numeric[random.Next(numeric.Length)]);

            // Add one special character
            password.Append(special[random.Next(special.Length)]);

            // Fill the rest of the password with random characters
            const string allChars = upperCase + lowerCase + numeric + special;
            for (int i = 0; i < 4; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password characters
            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }
}
