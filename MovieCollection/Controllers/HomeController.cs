using Microsoft.AspNetCore.Mvc;
using MovieCollection.Models;
using System.Diagnostics;

namespace MovieCollection.Controllers
{
    public class HomeController : Controller
    {
        dbContext db = new dbContext();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var randomMovies = db.Movies.Where(x => x.ViewDate == null)
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();

            return View(randomMovies);
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
