using Microsoft.AspNetCore.Identity;

namespace E_Com.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Payment> Payments { get; set; }

    }
}
