using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Models;

namespace MovieCollection.Controllers
{
    public class CategoriesController : Controller
    {
        dbContext db = new dbContext();

        // GET: CategoriesController
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();

            return View(categories);
        }

        // GET: CategoriesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoriesController/Edit/5
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);

            return View(category);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var category = db.Categories.Find(id);

            try
            {

                category.CatName = collection["Name"];

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoriesController/Delete/5
        public ActionResult Delete(int id)
        {

            var category = db.Categories.Find(id);
            return View(category);
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var category = db.Categories.Find(id);
            try
            {
                //check if category is used in movies
                var movies = db.Movies.Where(m => m.CategId == id).ToList();

                if (movies.Count > 0)
                {
                    ViewBag.Error = "Category is used in movies";
                    return View(category);
                }

                db.Categories.Remove(category);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
