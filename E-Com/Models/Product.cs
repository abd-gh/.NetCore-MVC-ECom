using System.ComponentModel.DataAnnotations;

namespace E_Com.Models
{
    public class Product
    {
        public int Id { get; set; }
        public Category Ctategory { get; set; }
        public int CtategoryId { get; set; }
        public string Name { get; set; }
        public bool State { get; set; }
        
        public double Price { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
