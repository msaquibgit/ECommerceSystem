using AutoMapper;
using OrderService.Application.DTOs.Cart;
using OrderService.Application.Interfaces;
using OrderService.Contract.ExternalServices;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IProductServiceClient _productServiceClient;

        public CartService(IMapper mapper, ICartRepository cartRepository, IProductServiceClient productServiceClient)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
        }

        public async Task<CartResponseDTO?> AddItemToCartAsync(AddCartItemRequestDTO request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Quantity <= 0) new ArgumentNullException("Quantity must be > 0");
            // Get Cart Owner Id and whether it is a guest user
            var cartUserId = await EnsureCartOwnerIdAsync(request.UserId);
            // Call Product Service API internally to get product details
            var productDetails = await _productServiceClient.GetProductByIdAsync(request.ProductId);
            if (productDetails == null) throw new InvalidOperationException("Product not found");
            if (productDetails.Quantity <= 0) new InvalidOperationException("Currently Out of stock.");
            // Map request to CartItem, using productDetails for price, name, etc.
            var cartItem = new CartItem
            {
                ProductId = productDetails.Id,
                ProductName = productDetails.Name,
                Price = productDetails.Price,
                Discount = productDetails.Price - productDetails.DiscountedPrice,
                Quantity = request.Quantity,
            };
            var updateCartItem = await _cartRepository.AddOrUpdateCartItemAsync(cartUserId, cartItem);
            if (updateCartItem == null) throw new InvalidOperationException("Failed to add or update the cart item.");
            var cart = _cartRepository.GetCartByUserIdAsync(cartUserId);
            if (cart == null) return null;
            // Map Cart entity to CartResponseDTO
            var cartResponse = _mapper.Map<CartResponseDTO>(cart);
            return cartResponse;
        }
        public async Task<CartResponseDTO?> UpdateCartItemAsync(UpdateCartItemRequestDTO request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.UserId == Guid.Empty) throw new ArgumentNullException("UserId is required for updating cart items.");
            if (request.Quantity <= 0) throw new ArgumentNullException("Quantity must be greater than zero.");
            // Check if cart exists
            bool cartExist = await _cartRepository.CartExistsAsync(request.UserId);
            if (!cartExist) throw new KeyNotFoundException("Cart not found for the given user.");
            // Fetch fresh product details from Product Service
            var productDetails = await _productServiceClient.GetProductByIdAsync(request.ProductId);
            if (productDetails == null) throw new KeyNotFoundException("Product not found.");
            // Map updated CartItem using fresh product details and requested quantity
            var cartItem = new CartItem
            {
                Id = request.CartItemId,              // Assuming CartItemId exists in DTO to identify item to update
                ProductId = productDetails.Id,
                ProductName = productDetails.Name,
                Price = productDetails.Price,
                Discount = productDetails.Price - productDetails.DiscountedPrice,
                Quantity = request.Quantity,
            };
            var updateCartItem = _cartRepository.UpdateCartItemAsync(cartItem);
            if (updateCartItem == null) throw new InvalidOperationException("Failed to update the cart item.");
            var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);
            if (cart == null) return null;
            return _mapper.Map<CartResponseDTO>(cart);
        }
        public async Task ClearCartAsync(ClearCartRequestDTO request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.UserId == Guid.Empty) throw new ArgumentNullException("UserId is required for clearing cart");
            bool cartExists = await _cartRepository.CartExistsAsync(request.UserId);
            if (!cartExists) throw new KeyNotFoundException("Cart not found for the given user.");
            await _cartRepository.ClearCartAsync(request.UserId);
        }

        public async Task<CartResponseDTO?> GetCartItemsAsync(Guid? userId)
        {
            // Get Cart Owner Id
            var cartOwnerId = await EnsureCartOwnerIdAsync(userId);
            var cart=_cartRepository.GetCartByUserIdAsync(cartOwnerId);
            if (cart == null)
                return null;

            var cartResponse = _mapper.Map<CartResponseDTO>(cart);
            return cartResponse;
        }

        public async Task<CartResponseDTO?> MergeCartsAsync(Guid targetUserId, Guid sourceUserId)
        {
            await _cartRepository.MergeCartsAsync(targetUserId, sourceUserId);
            var cart = await _cartRepository.GetCartByUserIdAsync(targetUserId);

            if (cart == null)
                return null;

            return _mapper.Map<CartResponseDTO>(cart);
        }

        public async Task<CartResponseDTO?> RemoveCartItemAsync(RemoveCartItemRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required for removing cart items.");

            bool cartExists = await _cartRepository.CartExistsAsync(request.UserId);
            if (!cartExists)
                throw new KeyNotFoundException("Cart not found for the given user.");

            await _cartRepository.RemoveCartItemAsync(request.CartItemId);

            var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);

            if (cart == null)
                return null;

            return _mapper.Map<CartResponseDTO>(cart);

        }

        private async Task<Guid> EnsureCartOwnerIdAsync(Guid? userId)
        {
            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                bool exists = await _cartRepository.CartExistsAsync(userId.Value);
                if (!exists)
                {
                    var newCart = new Cart { UserId = userId.Value, CreatedAt = DateTime.UtcNow };
                    await _cartRepository.AddAsync(newCart);

                }
                return userId.Value; // Not a guest user
            }
            else
            {
                var guestUserId = Guid.NewGuid();
                var newCart = new Cart { UserId = guestUserId, CreatedAt = DateTime.UtcNow };
                await _cartRepository.AddAsync(newCart);
                return guestUserId;// Guest user
            }
        }
    }
}
