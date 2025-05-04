using BookStore.Data;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [AllowAnonymous]
    public class StoreController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly RecommendationService _recommendationService;

        public StoreController(BookStoreContext context, RecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }
        public async Task<IActionResult> Index(string searchString, string minPrice, string maxPrice, string language)
        {
            var books = _context.Books.Select(b => b);
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(minPrice))
            {
                var min = int.Parse(minPrice);
                books = books.Where(b => b.Price >= min);
            }
            if (!string.IsNullOrEmpty(maxPrice))
            {
                var max = int.Parse(maxPrice);
                books = books.Where(b => b.Price <= max);
            }
            if (!string.IsNullOrEmpty(language))
            {
                books = books.Where(b => b.Language == language);
            }
            return View(await books.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            var recommendedBooks = _recommendationService.GetRecommendedBooks(book.Id, topN: 5);
            ViewBag.RecommendedBooks = recommendedBooks;

            return View(book);
        }
    }
}
