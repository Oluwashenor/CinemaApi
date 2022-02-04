using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name cannot be null or Empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Language Cannot be Empty")]
        public string Language { get; set; }
        [Required]
        public double Rating { get; set; }
        public bool Deleted { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
