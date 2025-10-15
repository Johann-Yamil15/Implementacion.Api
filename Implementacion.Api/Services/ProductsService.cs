using Implementacion.Api.Context;
using Implementacion.Api.Contract;
using Implementacion.Api.Flyweight;
using Implementacion.Api.Models.DTO;
using Implementacion.Api.Models;
using Implementacion.Api.Singleton;
using Microsoft.EntityFrameworkCore;

namespace Implementacion.Api.Services
{
    public class ProductsService : IProductsService
    {
        private readonly AppDbContext _db;

        public ProductsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();

            var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
            foreach (var c in products.Select(p => p.Category).Distinct())
                factory.GetOrAdd(c);

            return products.Select(p =>
                new ProductDTO(p.Id, p.Name, p.Price, p.CategoryId, p.Category?.Name ?? string.Empty)
            );
        }

        public async Task<ProductDTO?> GetByIdAsync(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
            factory.GetOrAdd(product.Category);

            return new ProductDTO(product.Id, product.Name, product.Price, product.CategoryId, product.Category?.Name ?? string.Empty);
        }

        public async Task<ProductDTO> CreateAsync(ProductDTO dto)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
            {
                category = new Category { Name = dto.CategoryName };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }

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

            return new ProductDTO(product.Id, product.Name, product.Price, product.CategoryId, sharedCategory.Name);
        }

        public async Task<bool> UpdateAsync(int id, ProductDTO dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Price = dto.Price;

            if (product.CategoryId != dto.CategoryId)
            {
                var cat = await _db.Categories.FindAsync(dto.CategoryId);
                if (cat == null) return false;
                product.CategoryId = cat.Id;

                var factory = SingletonConstruct<CategoryFlyweightFactory>.Instance;
                factory.GetOrAdd(cat);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
