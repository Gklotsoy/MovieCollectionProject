using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Models;

namespace MovieCollection.Controllers
{
    public class ActorsController : Controller
    {
        dbContext db = new dbContext();

        // GET: ActorsController
        public ActionResult Index()
        {
            var actors = db.Actors.ToList();

            return View(actors);
        }

        // GET: ActorsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ActorsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Actor actor)
        {
            //check if the actor already exists

            var actorExists = db.Actors
                .Where(x=>x.FirstName == actor.FirstName && x.LastName == actor.LastName)
                .Any();


            if (actorExists)
            {
                ViewBag.Error = "Actor already exists";
            }

            try
            {

                db.Actors.Add(actor);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {

                return View();
            }
        }

        // GET: ActorsController/Edit/5
        public ActionResult Edit(int id)
        {
            var actor = db.Actors.Find(id);
            return View(actor);
        }

        // POST: ActorsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var actor = db.Actors.Find(id);
            try
            {
                db.Actors.Update(actor);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {

                return View(actor);
            }
        }

        // GET: ActorsController/Delete/5
        public ActionResult Delete(int id)
        {
            var actor = db.Actors.Find(id);
            return View(actor);
        }

        // POST: ActorsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var actor = db.Actors.Find(id);
            try
            {
                db.Actors.Remove(actor);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(actor);
            }
        }
    }
}
