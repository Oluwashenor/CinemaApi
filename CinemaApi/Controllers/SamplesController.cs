using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SamplesController : ControllerBase
    {
        // GET: api/<SamplesController>
        [Authorize(Roles = "User")]
        [HttpGet]
        public string Get()
        {
            return "Hello From The User Side";
            //return new string[] { "value1", "value2" };
        }

        // GET api/<SamplesController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Hello From the Admin Side";
        }
    }
}
