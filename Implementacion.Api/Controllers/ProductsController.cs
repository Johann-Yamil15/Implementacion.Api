using Implementacion.Api.Context;
using Implementacion.Api.Flyweight;
using Implementacion.Api.Models.DTO;
using Implementacion.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Implementacion.Api.Singleton;

namespace Implementacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();

            // Usar Flyweight: registrar categorías en la fábrica para reutilización
            var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
            foreach (var c in products.Select(p => p.Category).Distinct())
            {
                factory.GetOrAdd(c);
            }

            var dtos = products.Select(p => new ProductDTO(p.Id, p.Name, p.Price, p.CategoryId, p.Category?.Name ?? string.Empty));
            return Ok(dtos);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Buscar categoria existente
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
            {
                // Si la categoria no existe, crearla
                category = new Category { Name = dto.CategoryName };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }

            // Usar flyweight para obtener la instancia compartida
            var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
            var sharedCategory = factory.GetOrAdd(category);

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = sharedCategory.Id
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            var resultDto = new ProductDTO(product.Id, product.Name, product.Price, product.CategoryId, sharedCategory.Name);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, resultDto);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            // Registrar categoria en flyweight para futuras llamadas
            var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
            factory.GetOrAdd(product.Category);

            var dto = new ProductDTO(product.Id, product.Name, product.Price, product.CategoryId, product.Category?.Name ?? string.Empty);
            return Ok(dto);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;

            if (product.CategoryId != dto.CategoryId)
            {
                var cat = await _db.Categories.FindAsync(dto.CategoryId);
                if (cat == null) return BadRequest("Categoria no existe");
                product.CategoryId = cat.Id;

                var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
                factory.GetOrAdd(cat);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}