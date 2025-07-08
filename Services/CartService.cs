using BusinessObjects.DTO.CartDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CartCreateResult> CreateAsync(string productId, string buyerId, CreateCartDTO categoryDto)
        {
            var addProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            var buyer = await _unitOfWork.Repository<User>().GetByIdAsync(buyerId);

            if (addProduct == null || !addProduct.IsVerified || addProduct.Status == ProductStatus.UnPaid_Seller)
            {
                return new CartCreateResult { ErrorMessage = "Product not found." };
            }

            if (buyer == null || !buyer.IsVerified || buyer.Role != BusinessObjects.Enums.UserRole.Buyer)
            {
                return new CartCreateResult { ErrorMessage = "Buyer not found." };
            }

            var cart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                CartNo = Guid.NewGuid().ToString(),
                Status = categoryDto.Status,
                Price = addProduct.Price,
                Discount = categoryDto.Discount,
                Quantity = categoryDto.Quantity,
                ProductId = productId,
                BuyerId = buyer.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Repository<Cart>().AddAsync(cart);
            await _unitOfWork.SaveAsync();
            return new CartCreateResult { Cart = cart };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(id);
            if (cart == null)
            {
                return false;
            }
            cart.IsDeleted = true;
            _unitOfWork.Repository<Cart>().Update(cart);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CartDTO>> GetAllAsync()
        {
            var carts = await _unitOfWork.Repository<Cart>()
                 .Query()
                 .Where(c => !c.IsDeleted)
                 .ToListAsync();
            return carts.Select(c => MapToDTO(c)).ToList();
        }

        public async Task<CartDTO?> GetByIdAsync(string id)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(id);
            return cart == null ? null : MapToDTO(cart);
        }

        public async Task<bool> UpdateAsync(string id, UpdateCartDTO categoryDto)
        {
            var existingCart = await _unitOfWork.Repository<Cart>().GetByIdAsync(id);
            if (existingCart == null)
            {
                return false;
            }
            existingCart.Status = categoryDto.Status;
            existingCart.Discount = categoryDto.Discount;
            existingCart.Quantity = categoryDto.Quantity;
            existingCart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Cart>().Update(existingCart);
            await _unitOfWork.SaveAsync();
            return true;

        }

        private CartDTO MapToDTO(Cart cart)
        {
            return new CartDTO
            {
                Id = cart.Id,
                CartNo = cart.CartNo,
                Status = cart.Status,
                Price = cart.Price,
                Discount = cart.Discount,
                Quantity = cart.Quantity,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                IsDeleted = cart.IsDeleted,
                ProductId = cart.ProductId,
                BuyerId = cart.BuyerId
            };
        }
    }
}
