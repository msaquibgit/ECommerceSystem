using OrderService.Domain.Entities;

namespace OrderService.Domain.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart?> AddAsync(Cart cart);
        Task<Cart?> UpdateAsync(Cart cart);
        Task DeleteAsyncByUserId(Guid userId);
        Task RemoveProductFromCartAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);
        Task<bool> CartExistsAsync(Guid userId);
        Task<IList<CartItem>> GetCartItemsAsync(Guid userId);
        Task<CartItem?> AddCartItemAsync(CartItem cartItem);
        Task<CartItem?> UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(Guid cartItemId);
        Task<CartItem> AddOrUpdateCartItemAsync(Guid userId, CartItem cartItem);
        Task MergeCartsAsync(Guid targetUserId, Guid sourceUserId);
        



        //// Gets the active cart for the specified user.
        //Task<Cart?> GetByUserIdAsync(Guid userId);

        //// Adds a new cart for the specified user (should only be needed once, typically).
        //Task<Cart?> AddAsync(Cart cart);

        //// Updates the entire cart (including items).
        //Task<Cart?> UpdateAsync(Cart cart);

        //// Completely deletes the user's cart (rarely used; usually use ClearCart).
        //Task DeleteAsyncByUserId(Guid userId);

        //// Removes a specific product from the user's cart.
        //Task RemoveProductFromCartAsync(Guid userId, Guid productId);

        //// Clears all items from the user's cart (but keeps the cart itself).
        //Task ClearCartAsync(Guid userId);

        ////To quickly check if a user has a cart without retrieving it fully.
        //Task<bool> CartExistsAsync(Guid userId);

        //// Gets all items for the user's active cart.
        //Task<IList<CartItem>> GetCartItemsAsync(Guid userId);

        //// Adds a single cart item to the cart.
        //Task<CartItem?> AddCartItemAsync(CartItem cartItem);

        //// Updates a single cart item in the cart.
        //Task<CartItem?> UpdateCartItemAsync(CartItem cartItem);

        //// Removes a single cart item by its CartItemId.
        //Task RemoveCartItemAsync(Guid cartItemId);

        //// Adds or updates a cart item: if the item exists, increments or sets the quantity; otherwise, adds as new.
        //Task<CartItem> AddOrUpdateCartItemAsync(Guid userId, CartItem cartItem);

        //// Merging anonymous guest cart with user cart on login
        //Task MergeCartsAsync(Guid targetUserId, Guid sourceUserId);

    }
}
