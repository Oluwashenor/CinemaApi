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
using CinemaApi.DTOs;

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
                         where movie.Deleted == false
                         select new MovieDTO
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
            var movieDTO = (from movie in _dbContext.Movies
                         where movie.Id == id
                         where movie.Deleted == false
                        select new MovieDTO{
                Name = movie.Name,
                Description = movie.Description,
                Language = movie.Description,
                Duration = movie.Duration,
                PlayingDate = movie.PlayingDate,
                PlayingTime = movie.PlayingTime,
                TicketPrice = movie.TicketPrice,
                Rating = movie.Rating,
                Genre = movie.Genre,
                TrailerUrl = movie.TrailerUrl
            }).FirstOrDefault();
            if (movieDTO == null)
                return NotFound();
            return Ok(movieDTO);
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public IActionResult Post([FromForm]MovieDTO movieDTO)
        {
            var movie = new Movie{
                Image = movieDTO.Image,
                ImageUrl = movieDTO.ImageUrl,
                Name = movieDTO.Name,
                Description = movieDTO.Description,
                Language = movieDTO.Language,
                Duration = movieDTO.Duration,
                PlayingDate = movieDTO.PlayingDate,
                PlayingTime = movieDTO.PlayingTime,
                TicketPrice = movieDTO.TicketPrice,
                Rating = movieDTO.Rating,
                Genre = movieDTO.Genre,
                TrailerUrl = movieDTO.TrailerUrl
            };
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
        public IActionResult Put(int id, [FromForm] MovieDTO movieDTO)
        {
            var movieInDb = (from m in _dbContext.Movies
                            where m.Deleted == false
                            where m.Id == id
                            select m).FirstOrDefault();
            if (movieInDb == null)
                return NotFound("No Record found against this ID");
            var guid = Guid.NewGuid();
            var movie = new Movie{
                Image = movieDTO.Image,
                Name = movieDTO.Name,
                Description = movieDTO.Description,
               Language = movieDTO.Language,
               Duration = movieDTO.Duration,
               Genre = movieDTO.Genre,
               PlayingDate = movieDTO.PlayingDate,
               PlayingTime = movieDTO.PlayingTime,
               Rating = movieDTO.Rating,
               TrailerUrl = movieDTO.TrailerUrl,
               TicketPrice = movieDTO.TicketPrice
            };
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
                         where movie.Deleted == false
                         select new MovieDTO
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
            movieInDb.Deleted = true;
            _dbContext.SaveChanges();
            return Ok("Record Deleted");
        }
    }
}
