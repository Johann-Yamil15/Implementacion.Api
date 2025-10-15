using Implementacion.Api.Models.DTO;

namespace Implementacion.Api.Contract
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(int id);
        Task<ProductDTO> CreateAsync(ProductDTO dto);
        Task<bool> UpdateAsync(int id, ProductDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
