using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
namespace Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDetail> AddProductToCartAsync(string userId, string productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId) ?? await CreateCartAsync(userId);

            var cartDetailRepo = _unitOfWork.Repository<CartDetail>();
            var existingDetail = await cartDetailRepo
                .Query()
                .FirstOrDefaultAsync(cd => cd.CartId == cart.Id && cd.ProductId == productId);

            if (existingDetail != null)
            {
                existingDetail.Quantity += quantity;
                existingDetail.UpdatedAt = DateTime.UtcNow;
                cartDetailRepo.Update(existingDetail);
            }
            else
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null || !product.IsVerified || product.Status == ProductStatus.UnPaid_Seller)
                    throw new Exception("Product not found");

                var newDetail = new CartDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = product.Id,
                    CartId = cart.Id,
                    Quantity = quantity,
                    Price = product.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await cartDetailRepo.AddAsync(newDetail);
                existingDetail = newDetail;
            }

            await _unitOfWork.SaveAsync();
            return existingDetail;

        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var buyer = await _unitOfWork.Repository<User>().Query()
                .FirstOrDefaultAsync(b => b.IsVerified && !b.IsDeleted && b.Status == UserStatus.Active);
            if(buyer == null)
            {
                return null;
            }

            var cart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
            };

            await _unitOfWork.Repository<Cart>().AddAsync(cart);
            await _unitOfWork.SaveAsync();

            return cart;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            if(!user.IsVerified || user.IsDeleted || user.Status != UserStatus.Active)
            {
                return null;
            }

            var cart = await _unitOfWork.Repository<Cart>().Query().Include(c => c.CartDetails).ThenInclude(p => p.Product)
                            .FirstOrDefaultAsync(c => c.BuyerId == userId);

            return cart;
        }

        public async Task<List<CartDetail>> GetCartDetailsAsync(string cartId)
        {
            var cartDetails = await _unitOfWork.Repository<CartDetail>()
                .Query()
                .Where(cd => cd.CartId == cartId)
                .Include(cd => cd.Product)
                .ToListAsync();

            return cartDetails;
        }

        public async Task<bool> RemoveCartDetailAsync(string cartDetailId)
        {
            var detail = await _unitOfWork.Repository<CartDetail>().GetByIdAsync(cartDetailId);
            if (detail == null) return false;

            _unitOfWork.Repository<CartDetail>().Delete(detail);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateCartDetailQuantityAsync(string cartDetailId, int newQuantity)
        {
            var detail = await _unitOfWork.Repository<CartDetail>().GetByIdAsync(cartDetailId);
            if (detail == null) return false;

            detail.Quantity = newQuantity;
            detail.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<CartDetail>().Update(detail);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
