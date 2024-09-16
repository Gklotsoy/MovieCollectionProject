using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCollection.Models;
using Newtonsoft.Json.Linq;

namespace MovieCollection.Controllers
{
    public class MoviesController : Controller
    {
        dbContext db = new dbContext();
        // GET: MoviesController
        public ActionResult Index()
        {
            var movies = db.Movies.ToList();

            movies.ForEach(m =>
            {   
                m.Categ = db.Categories.Find(m.CategId);
                m.Platform = db.Platforms.Find(m.PlatformId);
            });

            return View(movies);
        }

        // GET: MoviesController/Details/5
        public ActionResult Details(int id)
        {
            var movie = db.Movies.Find(id);

            movie.Categ = db.Categories.Find(movie.CategId);
            movie.Platform = db.Platforms.Find(movie.PlatformId);

            var actors = db.MoviesActors
                .Where(x => x.MovieId == id)
                .Select(y => $"{y.Actor.FirstName} {y.Actor.LastName}").ToList();

            ViewBag.Actors = actors;

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://www.omdbapi.com/?apikey=198c5a7&t=" + movie.Title),
              
            };

            using (var response = client.Send(request))
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync();

                JObject movieData = JObject.Parse(body.Result);

                if (movieData["Error"] != null)
                {
                    ViewBag.Overview = "No overview available";
                    return View(movie);
                }

                string plot = movieData["Plot"].ToString();

                ViewBag.Overview = plot;
            }


            return View(movie);
        }

        // GET: MoviesController/Create
        public ActionResult Create()
        {
            
            ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName");
            ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName");

            return View();
        }

        // POST: MoviesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Title,Rating,ReleaseDate,ViewDate,CategId,PlatformId")] Movie movie, IFormFile ImageFile)
        {

            //check if the movie already exists
            var movieExists = db.Movies.Any(x => x.Title == movie.Title);

            if (movieExists)
            {
                ViewBag.Error = "This movie already exists";
                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName");
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName");
                return View();
            }

            try
            {
                if (ImageFile != null)
                {
                    // Generate a unique file name
                    //var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);


                    var extension = Path.GetExtension(ImageFile.FileName);
                    var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                    // Define the path to save the file
                    var path = Path.Combine("wwwroot/images", uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    // Set the image path in the movie object
                    movie.Image = uniqueFileName;
                }


                db.Movies.Add(movie);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            catch
            {
                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName");
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName");
                return View();
            }
        }

        // GET: MoviesController/Edit/5
        public ActionResult Edit(int id)
        {
            var movie = db.Movies.Find(id);

            ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
            ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);

            var actors = db.Actors.ToList();
            var actorsData = actors.Select(x => new 
            {
                x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
            }).ToList();

            ViewBag.Actors = new SelectList(actorsData, "Id", "FullName");


            return View(movie);
        }

        // POST: MoviesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection formData)
        {   

            var movie = db.Movies.Find(id);
            var ImageFile = formData.Files["ImageFile"];
            try
            {

                if (ImageFile != null) {

                    // Generate a unique file name
                    
                    var extension = Path.GetExtension(ImageFile.FileName);
                    var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                    // Define the path to save the file
                    var path = Path.Combine("wwwroot/images", uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    // Delete the old image

                    if (movie.Image != null)
                    {
                        // Delete the old image
                        var oldImagePath = Path.Combine("wwwroot/images", movie.Image);
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Set the image path in the movie object
                    movie.Image = uniqueFileName;
                }

                //TryUpdateModelAsync(movie);
                db.Movies.Update(movie);

                db.SaveChanges();

                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddActors(int movieId, int actorId)
        {
            var movie = db.Movies.Find(movieId);
            var actor = db.Actors.Find(actorId);

            try
            {
                if (movie == null || actor == null)
                {
                    return RedirectToAction("Edit", "Movies", new { id = movieId });
                }

                var movieActorExists = db.MoviesActors.Any(ma => ma.MovieId == movieId && ma.ActorId == actorId);

                if (movieActorExists)
                {
                    ViewBag.Error = "This actor is already associated with this movie";
                    return RedirectToAction("Edit", "Movies", new { id = movieId });
                }

                var movieActor = new MoviesActor
                {
                    MovieId = movieId,
                    ActorId = actorId,
                };

                db.MoviesActors.Add(movieActor);
                db.SaveChanges();

                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);
            }
            catch
            {
                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);


            }


            return RedirectToAction("Edit", new { id = movieId });
        }

        // GET: MoviesController/Delete/5
        public ActionResult Delete(int id)
        {
            var movie = db.Movies.Find(id);

            movie.Categ = db.Categories.Find(movie.CategId);
            movie.Platform = db.Platforms.Find(movie.PlatformId);

            return View(movie);
        }

        // POST: MoviesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var movie = db.Movies.Find(id);

            try
            {

                if (movie.Image != null)
                {
                    // Delete the old image
                    var oldImagePath = Path.Combine("wwwroot/images", movie.Image);
                    System.IO.File.Delete(oldImagePath);
                }

                db.Movies.Remove(movie);
                db.SaveChanges();

                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Categories = new SelectList(db.Categories, "Id", "CatName", movie.CategId);
                ViewBag.Platforms = new SelectList(db.Platforms, "Id", "PlatformName", movie.PlatformId);

                return View();
            }
        }
    }
}
