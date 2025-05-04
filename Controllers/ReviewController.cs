using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly BookStoreContext _context;

        public ReviewController(BookStoreContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId, string comment, int rating)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var review = new Review
                {
                    UserEmail = userEmail, 
                    Comment = comment,
                    Rating = rating,
                    BookId = bookId
                };
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Store", new { id = bookId });
            }
            return RedirectToAction("Details", "Store", new { id = bookId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Store", new { id = review.BookId });
        }
    }
}