using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Com.Models
{
    public class BillingAddress
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public int  Zip { get; set; }
        public string cardType { get; set; }
        public long cardNumber { get; set; }
        public string cardName { get; set; }
        public DateTime expiration { get; set; }
        public int cvv { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
