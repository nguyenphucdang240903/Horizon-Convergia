using BusinessObjects.DTO.ProductDTO;

namespace Services.Interfaces
{
    public interface IProductService
    {
        // GET
        Task<IEnumerable<ProductDTO>> GetAllAsync(
            string? categoryId = null,
            string? location = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);
        Task<ProductDTO?> GetByIdAsync(string id);
        Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync(string sellerId, 
            string? categoryId = null,
            string? location = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);
        Task<IEnumerable<ProductDTO>> GetUnpaidProductsAsync(string sellerId, 
            string? categoryId = null,
            string? location = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);
        Task<IEnumerable<ProductDTO>> GetFavoriteProductsAsync(string userId, int pageNumber =1, int pageSize = 5);


        // POST
        Task<ProductCreateResult> CreateAsync(CreateProductDTO productDto, string adminId);
        Task<ProductCreateResult> SellerCreateAsync(string sellerId, CreateProductDTO productDto);
        Task<string> SendPaymentLinkToSellerAsync(string productId);
        Task<bool> AddToFavoritesAsync(string userId, string productId);
        // PUT
        Task<bool> UpdateAsync(string id, UpdateProductDTO productDto);


        // DELETE
        Task<bool> DeleteAsync(string id);
        Task<bool> RemoveFromFavoritesAsync(string userId, string productId);
    }

}