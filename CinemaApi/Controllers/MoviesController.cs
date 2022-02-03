﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaApi.Data;
using CinemaApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        public IActionResult Get()
        {
            return Ok(_dbContext.Movies);
        }

        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var movieInDB = _dbContext.Movies.Find(id);
            if (movieInDB == null)
                return NotFound("No record found");
            return Ok(movieInDB);
        }

        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Movie movie)
        {
            var movieInDb = _dbContext.Movies.Find(id);
            if (movieInDb == null)
                return NotFound("No Record found of the same ID");
            movieInDb.Name = movie.Name;
            movieInDb.Language = movie.Language;
            _dbContext.SaveChanges();
            return Ok("Record Updated Successfully");
            
        }

        // DELETE api/<MoviesController>/5
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
