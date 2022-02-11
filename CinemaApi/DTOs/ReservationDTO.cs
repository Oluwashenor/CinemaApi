using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Models;

namespace CinemaApi.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string Phone { get; set; }
        public DateTime ReservationTime { get; set; }
        public MovieDTO Movie { get; set; }
        public UserDTO User { get; set; }
    }
}
