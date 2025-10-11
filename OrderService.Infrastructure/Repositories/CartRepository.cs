using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderDbContext _context;

        public CartRepository(OrderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Gets the active cart for the specified user (including cart items, read-only)
        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Carts.Include(c => c.CartItems) // Eager load the cart items
                 .AsNoTracking()
                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // Returns all cart items for the user's active cart (read-only)
        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }



        // Adds a new cart entity for the user

        public async Task<Cart?> AddAsync(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;  // Return newly added cart with generated Id
        }

        // Updates the entire cart including its items
        public async Task<Cart?> UpdateAsync(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task DeleteAsyncByUserId(Guid userId)
        {
            // Retrieve cart including items
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);    // Delete all cart items
                _context.Carts.Remove(cart);                       // Delete the cart itself
                await _context.SaveChangesAsync();

            }
        }
        // Removes a specific product from the user's cart by ProductId
        public async Task RemoveProductFromCartAsync(Guid userId, Guid productId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .SingleOrDefaultAsync(x => x.UserId == userId && x.Id == productId);
            if (cart == null) return;
            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
        // Clears all items from the user's cart but keeps the cart entity
        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _context.Carts.Include(ci => ci.CartItems)
                .FirstOrDefaultAsync(ci => ci.UserId == userId);
            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }

        }
        // Checks if a cart exists for the given user
        public async Task<bool> CartExistsAsync(Guid userId)
        {
            return await _context.Carts.AnyAsync(x => x.UserId == userId);
        }
        // Returns all cart items for the user's active cart (read-only)
        public async Task<IList<CartItem>> GetCartItemsAsync(Guid userId)
        {
            var cart = await _context.Carts.Include(ci => ci.CartItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
            return cart?.CartItems.ToList() ?? new List<CartItem>();
        }
        // Adds a single new cart item to the database
        public async Task<CartItem?> AddCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null) throw new ArgumentNullException(nameof(cartItem));
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
        // Updates an existing cart item in the database
        public async Task<CartItem?> UpdateCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null) throw new ArgumentNullException(nameof(cartItem));
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
        // Removes a single cart item by its CartItemId
        public async Task RemoveCartItemAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }
        // Adds or updates a cart item based on a CartItem object
        public async Task<CartItem> AddOrUpdateCartItemAsync(Guid userId, CartItem cartItem)
        {
            if (cartItem == null)
                throw new ArgumentNullException(nameof(cartItem));
            if (cartItem.ProductId == Guid.Empty)
                throw new ArgumentNullException($"ProductId must be set on CartItem", nameof(cartItem));
            if (cartItem.Quantity <= 0)
                throw new ArgumentNullException("Quantity must be greater than zero.", nameof(cartItem));

            // Get cart with items
            var cart = await _context.Carts.Include(ci => ci.CartItems)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (cart == null)
            {
                // Create new cart if not found
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            // Find existing cart item by ProductId

            var exisitingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == cartItem.ProductId);
            if (exisitingItem != null)
            {
                exisitingItem.ProductName = cartItem.ProductName;
                exisitingItem.Price = cartItem.Price;
                exisitingItem.Discount = cartItem.Discount;
                exisitingItem.Quantity += cartItem.Quantity;
                await _context.SaveChangesAsync();

                return exisitingItem;
            }
            else
            {
                // Add new item and associate with cart
                cartItem.CartId = cart.Id;
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();

                return cartItem;
            }
        }
        // Merges the source user's cart into the target user's cart
        public async Task MergeCartsAsync(Guid targetUserId, Guid sourceUserId)
        {
            // TargetUserId: The user ID of the authenticated user who just logged in or registered.
            // SourceUserId: The guest/ anonymous ID used before login(temporary ID linked to the guest cart).

            // If the source and target are the same, no action needed
            if (targetUserId == sourceUserId)
                return;
            // Load Target user cart with items
            var targetCart = await _context.Carts.Include(ci => ci.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == targetUserId);
            // Load source user's cart with items (usually guest or anonymous cart)
            var sourceCart = await _context.Carts.Include(ci => ci.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == sourceUserId);
            if (sourceCart == null)
            {
                return; // No source cart to merge
            }
            if (targetCart == null)
            {
                // If target cart doesn't exist, simply assign source cart to target user
                sourceCart.UserId = targetUserId;
                await _context.SaveChangesAsync();
                return;
            }
            // Merge each item from source cart into target cart
            foreach (var sourceItem in sourceCart.CartItems)
            {
                // Try find matching item in target cart by ProductId
                var targetItem = targetCart.CartItems.FirstOrDefault(ci => ci.ProductId == sourceItem.ProductId);
                if (targetItem != null)
                {
                    // If exists, increment quantity (business logic)
                    targetItem.Quantity += sourceItem.Quantity;
                    _context.CartItems.Update(targetItem);
                }
                else
                {
                    // Otherwise, move the item to target cart by updating CartId
                    sourceItem.CartId = targetCart.Id;
                     _context.CartItems.Update(sourceItem);
                }
                // Save all merged changes
                await _context.SaveChangesAsync();
                // Remove the source cart and its items (cleanup)
                _context.CartItems.RemoveRange(sourceCart.CartItems);
                _context.Carts.Remove(sourceCart);
                await _context.SaveChangesAsync();

            }
        }


    }
}
