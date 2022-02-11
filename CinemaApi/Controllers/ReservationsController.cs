using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.DTOs;
using CinemaApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {

        private CinemaDbContext _dbContext;
        
        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpPost]
        public IActionResult Post(ReservationDTO reservationDto)
        {
            var reservation = new Reservation
            {
               Qty = reservationDto.Qty,
               Price = reservationDto.Price,
               Phone = reservationDto.Phone,
               ReservationTime = reservationDto.ReservationTime,
               MovieId = reservationDto.Movie.Id,
               UserId = reservationDto.User.Id,
            };
            reservation.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservation);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult GetReservations()
        {
            var reservations = from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where reservation.Deleted == false
                               select new ReservationDTO
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   Phone = reservation.Phone,
                                   Movie = new MovieDTO{
                                       Id = movie.Id,
                                       Name = movie.Name
                                   },
                                   User = new UserDTO{
                                       Id = customer.Id,
                                       Name = customer.Name
                                   }
                               };   
            return Ok(reservations);
        }


        // [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservations(int id)
        {
            var r = (from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where reservation.Id == id
                               where reservation.Deleted == false
                               select new ReservationDTO
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   Qty = reservation.Qty,
                                   Price = reservation.Price,
                                   Phone = reservation.Phone,
                                   User = new UserDTO{
                                     Name =  customer.Name,
                                     Id = customer.Id, 
                                    Email = customer.Email,
                                    },
                                    Movie = new MovieDTO{
                                        Name = movie.Name,
                                        PlayingDate = movie.PlayingDate,
                                        PlayingTime = movie.PlayingTime
                                    },
                                   
                                  }).FirstOrDefault();
            return Ok(r);
        }

        // DELETE api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
                return NotFound("No Record found of the same ID");
            reservation.Deleted = true;
            _dbContext.SaveChanges();
            return Ok("Record Deleted");
        }
    }
}
