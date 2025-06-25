using Agri_Energy_st10391223.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agri_Energy_st10391223.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Create roles
                string[] roleNames = { "Farmer", "Employee" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create sample employee users
                var employeeData = new List<(string Email, string FirstName, string LastName, string Password)>
                {
                    ("employee@demo.com", "Demo", "Employee", "Employee1!"),
                    ("john.manager@agri-energy.com", "John", "Manager", "Manager1!"),
                    ("sarah.admin@agri-energy.com", "Sarah", "Admin", "Admin1!"),
                    ("michael.support@agri-energy.com", "Michael", "Support", "Support1!"),
                    ("lisa.coordinator@agri-energy.com", "Lisa", "Coordinator", "Coordinator1!")
                };
                
                foreach (var empData in employeeData)
                {
                    if (await userManager.FindByEmailAsync(empData.Email) == null)
                    {
                        var employee = new ApplicationUser
                        {
                            UserName = empData.Email,
                            Email = empData.Email,
                            EmailConfirmed = true,
                            FirstName = empData.FirstName,
                            LastName = empData.LastName
                        };

                        var result = await userManager.CreateAsync(employee, empData.Password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(employee, "Employee");
                        }
                    }
                }

                // Create sample farmer users and their products
                if (!context.Farmers.Any())
                {
                    // Farmer 1
                    var farmer1User = new ApplicationUser
                    {
                        UserName = "farmer1@demo.com",
                        Email = "farmer1@demo.com",
                        EmailConfirmed = true,
                        FirstName = "John",
                        LastName = "Doe"
                    };

                    var result1 = await userManager.CreateAsync(farmer1User, "Farmer1!");
                    if (result1.Succeeded)
                    {
                        await userManager.AddToRoleAsync(farmer1User, "Farmer");

                        var farmer1 = new Farmer
                        {
                            Name = "John",
                            Surname = "Doe",
                            Email = "farmer1@demo.com",
                            PhoneNumber = "0123456789",
                            FarmName = "Green Valley Farm",
                            Region = "Eastern Cape",
                            UserId = farmer1User.Id
                        };

                        context.Farmers.Add(farmer1);
                        await context.SaveChangesAsync();

                        // Products for Farmer 1
                        var products1 = new List<Product>
                        {
                            new Product { 
                                Name = "Organic Tomatoes", 
                                Category = "Vegetables", 
                                ProductionDate = DateTime.Now.AddDays(-10), 
                                FarmerId = farmer1.Id 
                            },
                            new Product { 
                                Name = "Sweet Corn", 
                                Category = "Vegetables", 
                                ProductionDate = DateTime.Now.AddDays(-5), 
                                FarmerId = farmer1.Id 
                            },
                            new Product { 
                                Name = "Free Range Eggs", 
                                Category = "Poultry", 
                                ProductionDate = DateTime.Now.AddDays(-2), 
                                FarmerId = farmer1.Id 
                            }
                        };

                        context.Products.AddRange(products1);
                    }

                    // Farmer 2
                    var farmer2User = new ApplicationUser
                    {
                        UserName = "farmer2@demo.com",
                        Email = "farmer2@demo.com",
                        EmailConfirmed = true,
                        FirstName = "Jane",
                        LastName = "Smith"
                    };

                    var result2 = await userManager.CreateAsync(farmer2User, "Farmer2!");
                    if (result2.Succeeded)
                    {
                        await userManager.AddToRoleAsync(farmer2User, "Farmer");

                        var farmer2 = new Farmer
                        {
                            Name = "Jane",
                            Surname = "Smith",
                            Email = "farmer2@demo.com",
                            PhoneNumber = "0987654321",
                            FarmName = "Sunrise Organic Farm",
                            Region = "Western Cape",
                            UserId = farmer2User.Id
                        };

                        context.Farmers.Add(farmer2);
                        await context.SaveChangesAsync();

                        // Products for Farmer 2
                        var products2 = new List<Product>
                        {
                            new Product { 
                                Name = "Organic Apples", 
                                Category = "Fruits", 
                                ProductionDate = DateTime.Now.AddDays(-15), 
                                FarmerId = farmer2.Id 
                            },
                            new Product { 
                                Name = "Fresh Milk", 
                                Category = "Dairy", 
                                ProductionDate = DateTime.Now.AddDays(-1), 
                                FarmerId = farmer2.Id 
                            },
                            new Product { 
                                Name = "Wheat Flour", 
                                Category = "Grains", 
                                ProductionDate = DateTime.Now.AddDays(-20), 
                                FarmerId = farmer2.Id 
                            },
                            new Product { 
                                Name = "Organic Carrots", 
                                Category = "Vegetables", 
                                ProductionDate = DateTime.Now.AddDays(-7), 
                                FarmerId = farmer2.Id 
                            }
                        };

                        context.Products.AddRange(products2);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
