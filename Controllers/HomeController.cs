using System.Diagnostics;
using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookStoreContext _context;
        private readonly RecommendationService _recommendationService;

        public HomeController(BookStoreContext context, RecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index()
        {
            var allBooks = await _context.Books.ToListAsync();

            if (!allBooks.Any())
            {
                return View(new List<Book>());
            }

            var randomBook = allBooks.OrderBy(b => Guid.NewGuid()).FirstOrDefault();

            var recommendedBooks = _recommendationService.GetRecommendedBooks(randomBook.Id, topN: 6);

            if (!recommendedBooks.Any())
            {
                recommendedBooks = allBooks.Take(6).ToList();
            }

            return View(recommendedBooks);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}