using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Models;

namespace MovieCollection.Controllers
{
    public class PlatformsController : Controller
    {

        dbContext db = new dbContext();
        // GET: PlatformsController
        public ActionResult Index()
        {

            var platforms = db.Platforms.ToList();
            return View(platforms);
        }

        // GET: PlatformsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PlatformsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlatformsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Platform platform)
        {
            try
            {
                
                db.Platforms.Add(platform);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlatformsController/Edit/5
        public ActionResult Edit(int id)
        {
            var platform = db.Platforms.Find(id);
            return View(platform);
        }

        // POST: PlatformsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var platform = db.Platforms.Find(id);

            try
            {
                platform.PlatformName = collection["Name"];
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlatformsController/Delete/5
        public ActionResult Delete(int id)
        {
            var platform = db.Platforms.Find(id);
            return View(platform);
        }

        // POST: PlatformsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var platform = db.Platforms.Find(id);
            var movies = db.Movies.Where(m => m.PlatformId == id).ToList();
            try
            {
                if (movies.Count > 0)
                {
                    ViewBag.Error = "This platform is being used by movies";
                    return View(platform);

                }

                db.Platforms.Remove(platform);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var platforms = db.Platforms.ToList();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
