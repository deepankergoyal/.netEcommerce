using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System.Linq;
using System.Security.Claims;

namespace ShoopingWebApplication.Controllers
{
    [Authorize]
    public class AddToCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddToCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the current logged-in user's ID

            if (string.IsNullOrEmpty(currentUserId))
            {
                // If user is not logged in, handle the scenario (you might redirect to a login page)
                return RedirectToAction("Login", "Account");
            }

            var productToAdd = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            if (productToAdd == null || quantity <= 0)
            {
                // If product doesn't exist or quantity is invalid, handle accordingly (e.g., show an error message)
                return RedirectToAction("Index", "Product");
            }

            var userCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == currentUserId);

            if (userCart == null)
            {
                // If the user doesn't have a cart, create a new one
                userCart = new Cart { UserId = currentUserId };
                _context.Carts.Add(userCart);
                _context.SaveChanges(); // Save changes to generate the CartId
            }

            // Find the cart item within the user's cart based on the product ID
            var existingCartItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingCartItem != null)
            {
                // If the product is already in the cart, update the quantity
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Otherwise, add a new cart item
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = userCart.CartId // Assign the current user's cart ID to the cart item
                };

                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Product"); // Redirect back to the product list or any other page
        }
    }
}
