using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Models;

namespace MovieCollection.Controllers
{
    public class MoviesActorsController : Controller
    {
        dbContext db = new dbContext();

        // GET: MoviesActorsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MoviesActorsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MoviesActorsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MoviesActorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var movieId = int.Parse(collection["MovieId"]);
            var actorId = int.Parse(collection["ActorId"]);

            try
            {

                //check if the movie-actor relationship already exists

                //create a new MoviesActor object
                var movieActor = new MoviesActor
                {
                    MovieId = movieId,
                    ActorId = actorId,
                };

                //add the new MoviesActor object to the database
                db.MoviesActors.Add(movieActor);
                db.SaveChanges();


                //return to the movie details page
                return RedirectToAction("Edit", "Movies", new { id = movieId });
            }
            catch
            {
                return View();
            }
        }

        // GET: MoviesActorsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MoviesActorsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MoviesActorsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MoviesActorsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
