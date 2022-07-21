using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Com.Models
{
    public class Payment
    {
        public int Id { get; set; }
       public int Total { get; set; }
        
        public Cart Cart { get; set; }
        public int cartId { get; set; }
        //[ForeignKey("cartId")]  
        public int billingId { get; set; }
        [ForeignKey("billingId")]

        public BillingAddress BillingAddress { get; set; }
          
         public string UserId { get; set; }
         [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


    }
}
