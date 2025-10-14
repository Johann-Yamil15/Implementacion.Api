namespace Implementacion.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }


        // Navegación
        public ICollection<Product> Products { get; set; }
    }
}
