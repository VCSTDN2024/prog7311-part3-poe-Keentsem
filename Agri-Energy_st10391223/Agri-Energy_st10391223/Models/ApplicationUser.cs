using Microsoft.AspNetCore.Identity;

namespace Agri_Energy_st10391223.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
