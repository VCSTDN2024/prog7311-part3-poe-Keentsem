using Agri_Energy_st10391223.Data;
using Agri_Energy_st10391223.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Energy_st10391223.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FarmerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Farmer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var products = await _context.Products
                .Where(p => p.FarmerId == farmer.Id)
                .OrderByDescending(p => p.ProductionDate)
                .ToListAsync();

            ViewData["FarmerName"] = $"{farmer.Name} {farmer.Surname}";
            ViewData["FarmName"] = farmer.FarmName;

            return View(products);
        }

        // GET: Farmer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Farmer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                // Check if the user already has a farmer profile
                var existingFarmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (existingFarmer != null)
                {
                    return RedirectToAction(nameof(Dashboard));
                }

                farmer.UserId = user.Id;
                
                _context.Add(farmer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            return View(farmer);
        }

        // GET: Farmer/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(farmer);
        }

        // POST: Farmer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var existingFarmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (existingFarmer == null)
                {
                    return NotFound();
                }

                // Update farmer details
                existingFarmer.Name = farmer.Name;
                existingFarmer.Surname = farmer.Surname;
                existingFarmer.Email = farmer.Email;
                existingFarmer.PhoneNumber = farmer.PhoneNumber;
                existingFarmer.FarmName = farmer.FarmName;
                existingFarmer.Region = farmer.Region;

                try
                {
                    _context.Update(existingFarmer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(existingFarmer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(farmer);
        }

        // GET: Farmer/AddProduct
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: Farmer/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (farmer == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                product.FarmerId = farmer.Id;
                product.ProductionDate = DateTime.SpecifyKind(product.ProductionDate, DateTimeKind.Utc);

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            return View(product);
        }

        // GET: Farmer/EditProduct/5
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Farmer/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == user.Id);

                if (farmer == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.Name = product.Name;
                existingProduct.Category = product.Category;
                existingProduct.ProductionDate = DateTime.SpecifyKind(product.ProductionDate, DateTimeKind.Utc);

                try
                {
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(product);
        }

        // GET: Farmer/DeleteProduct/5
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Farmer/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (farmer == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.Id == id);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
