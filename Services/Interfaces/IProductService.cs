using BusinessObjects.DTO.ProductDTO;

namespace Services.Interfaces
{
    public interface IProductService
    {
        // GET
        Task<IEnumerable<ProductDTO>> GetAllAsync(
            string? categoryId = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);
        Task<ProductDTO?> GetByIdAsync(string id);
        Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync(string sellerId, string? categoryId = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);
        Task<IEnumerable<ProductDTO>> GetUnpaidProductsAsync(string sellerId, string? categoryId = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5);

        // POST
        Task<ProductDTO> CreateAsync(CreateProductDTO productDto);
        Task<ProductCreateResult> SellerCreateAsync(string sellerId, CreateProductDTO productDto);
        Task<string> SendPaymentLinkToSellerAsync(string productId, string returnUrl);

        // PUT
        Task<bool> UpdateAsync(string id, UpdateProductDTO productDto);
        Task<string> VerifyProduct(string id);
        Task<bool> ActivateProductAfterPaymentAsync(string productId);

        // DELETE
        Task<bool> DeleteAsync(string id);
    }

}
