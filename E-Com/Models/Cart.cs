using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Com.Models
{
    public class Cart
    {
       
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
       // public  Product Product { get; set; }
        public int Price { get; set; }// price curently
        public bool Status { get; set; }
        public DateTime? DateDone { get; set; }
        //  public ApplicationUser ApplicationUser { get; set; }
        //   public string ApplicationUserId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public List<Payment> Payments { get; set; }

    }
}