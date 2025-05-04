using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly Cart _cart;
        private readonly UserManager<DefaultUser> _userManager;
        private readonly IEmailSender _emailSender;
        public OrderController(
            BookStoreContext context,
            Cart cart,
            UserManager<DefaultUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _cart = cart;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cartItems = _cart.GetAllCartItems();
            _cart.CartItems = cartItems;

            if (_cart.CartItems.Count == 0)
            {
                ModelState.AddModelError("", "Grozs ir tukšs, pievienojat grāmatu.");
            }
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);
                order.UserEmail = user.Email;

                CreateOrder(order);
                _cart.ClearCart();
                await SendOrderConfirmationEmail(order);

                return View("CheckoutComplete", order);
            }
            return View(order);
        }
        public IActionResult CheckoutComplete(Order order)
        {
            return View(order);
        }
        public async Task<IActionResult> OrderHistory()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Where(o => o.UserEmail == _userManager.GetUserName(User))
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderPlaced)
                .ToListAsync();
            return View(orders);
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var userEmail = _userManager.GetUserName(User);

            if (!User.IsInRole("Admin"))
            {
                if (order.UserEmail != userEmail)
                {
                    return Forbid(); 
                }
            }
            return View(order);
        }
        private void CreateOrder(Order order)
        {
            order.OrderPlaced = DateTime.Now;
            order.OrderStatus = "Apstrādē";
            var cartItems = _cart.CartItems;
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem()
                {
                    Quantity = item.Quantity,
                    BookId = item.Book.Id,
                    OrderId = order.Id,
                    Price = item.Book.Price * item.Quantity
                };
                order.OrderItems.Add(orderItem);
                order.OrderTotal += orderItem.Price;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
        private async Task SendOrderConfirmationEmail(Order order)
        {
            var employeeEmail = "bookstorefromlv@gmail.com";
            var subject = "Saņemts jauns pasūtījums";
            var orderItemsHtml = string.Join("", order.OrderItems.Select(oi =>
                $"<li class=\"order-item\">{oi.Book.Title} (Daudzums: {oi.Quantity}, Cena:  {oi.Price.ToString("C", new System.Globalization.CultureInfo("fr-FR"))})</li>"));
            var htmlMessage = $@"
        <h2>Saņemts jauns pasūtījums</h2>
        <div class='order-details'>
            <p><strong>Pasūtījuma numurs:</strong> {order.Id}</p>
            <p><strong>Pasūtīts kopā:</strong>  {order.OrderTotal.ToString("C", new System.Globalization.CultureInfo("fr-FR"))}</p>
            <p><strong>Pasūtījums veikts:</strong> {order.OrderPlaced.ToString("g")}</p>
            <p><strong>Lietotāja e-pasts:</strong> {order.UserEmail}</p>
        </div>
        <h3>Pasūtītās grāmatas:</h3>
        <ul>
            {orderItemsHtml}
        </ul>";
            await _emailSender.SendEmailAsync(employeeEmail, subject, htmlMessage);
        }
    }
}