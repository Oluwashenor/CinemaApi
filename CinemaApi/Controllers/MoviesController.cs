using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaApi.Data;
using CinemaApi.Models;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            return _dbContext.Movies;
        }

        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public Movie Get(int id)
        {
            var movieInDB = _dbContext.Movies.Find(id);
            return movieInDB;
        }

        // POST api/<MoviesController>
        [HttpPost]
        public Movie Post([FromBody] Movie movie)
        {
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return movie;
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public Movie Put(int id, [FromBody] Movie movie)
        {
            var movieInDb = _dbContext.Movies.Find(id);
            movieInDb.Name = movie.Name;
            movieInDb.Language = movie.Language;
            _dbContext.SaveChanges();
            return movieInDb;
            
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var movieInDb = _dbContext.Movies.Find(id);
            _dbContext.Movies.Remove(movieInDb);
            _dbContext.SaveChanges();
        }
    }
}
