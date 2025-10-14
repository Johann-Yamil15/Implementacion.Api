using Microsoft.EntityFrameworkCore;

namespace Implementacion.Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }


        // FK a Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
