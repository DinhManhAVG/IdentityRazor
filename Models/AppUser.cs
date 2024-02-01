using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityRazor.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(400)]
        public string? HomeAddress { get; set; }
    }
}