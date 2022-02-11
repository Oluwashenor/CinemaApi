using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Models;

namespace CinemaApi.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description  { get; set; }
        public string Language { get; set; }
        public string Duration { get; set; }
        public DateTime PlayingDate  { get; set; }
        public DateTime PlayingTime { get; set; }
        public double TicketPrice { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string TrailerUrl { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
