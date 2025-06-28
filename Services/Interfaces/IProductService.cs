using BusinessObjects.DTO.ProductDTO;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync(string? categoryId = null,string? sortField = null,bool ascending = true);
        Task<ProductDTO?> GetByIdAsync(string id);
        Task<ProductDTO> CreateAsync(CreateProductDTO productDto);
        Task<ProductCreateResult> SellerCreateAsync(string sellerId, CreateProductDTO productDto);
        Task<bool> UpdateAsync(string id, UpdateProductDTO productDto);
        Task<string> VerifyProduct(string id);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync();
        Task<string> SendPaymentLinkToSellerAsync(string productId, string returnUrl);
        Task<bool> ActivateProductAfterPaymentAsync(string productId);

    }


}
