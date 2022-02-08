using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/<MoviesController>
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(String sort, int? pageNumber, int? pageSize)
        {

            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;

            var movies = from movie in _dbContext.Movies
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         };
            switch (sort)
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber - 1 )* currentPageSize).Take(currentPageSize));
            }
        }

        // api/movies/moviedetail/1
        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            //var movie = _dbContext.Movies.SingleOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();
            return Ok(movie);
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movie)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movie.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fileStream);
            }
            movie.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }


        // PUT api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movie)
        {
            var movieInDb = _dbContext.Movies.Find(id);
            if (movieInDb == null)
                return NotFound("No Record found against this ID");
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movie.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fileStream);
                movieInDb.ImageUrl = filePath.Remove(0, 7);
            }

            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Language = movie.Language;
            movieInDb.Duration = movie.Duration;
            movieInDb.Genre = movie.Genre;
            movieInDb.PlayingDate = movie.PlayingDate;
            movieInDb.PlayingTime = movie.PlayingTime;
            movieInDb.Rating = movie.Rating;
            movieInDb.TrailerUrl = movie.TrailerUrl;
            movieInDb.TicketPrice = movie.TicketPrice;
            _dbContext.SaveChanges();
            return Ok("Record Updated Successfully");

        }


        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovie(string movieName)
        {
            var movies = from movie in _dbContext.Movies
                         where movie.Name.StartsWith(movieName)
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                            ImageUrl = movie.ImageUrl
                         };
            return Ok(movies);
        }


        // DELETE api/<MoviesController>/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movieInDb = _dbContext.Movies.Find(id);
            if (movieInDb == null)
                return NotFound("No Record found of the same ID");
            _dbContext.Movies.Remove(movieInDb);
            _dbContext.SaveChanges();
            return Ok("Record Deleted");
        }



    }
}
