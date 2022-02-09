using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.Models;
using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _dbContext.Users.Where(u=>u.Email == user.Email).SingleOrDefault(); 
            if(userWithSameEmail != null)
            {
                return BadRequest("User with same email already Exist"); 
            }
            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "User"
            };
            _dbContext.Users.Add(userObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            var userInDb = _dbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if(userInDb == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
             //   return NotFound();
            }
            if(!SecurePasswordHasherHelper.Verify(user.Password, userInDb.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(ClaimTypes.Email,user.Email),
               new Claim(ClaimTypes.Role, userInDb.Role)
             };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id = userInDb.Id
            });
        }
    
        
         [HttpPost]
         public IActionResult ChangePassword([FromForm]Password password)
        {
            var user = (_dbContext.Users.Where(u=> u.Email == password.Email)).FirstOrDefault();
            if (user == null)
                return NotFound();
            if(!SecurePasswordHasherHelper.Verify(password.OldPassword, user.Password))
            {
                return BadRequest("Old Password not Correct");
            }
            if(password.NewPassword != password.ConfirmPassword)
            {
                return BadRequest("New Password and Confirm Password does not Match");
            }
            user.Password = SecurePasswordHasherHelper.Hash(password.ConfirmPassword);
            _dbContext.SaveChanges();
            return Ok("Password Updated Successfully");
            
        }
    
    }
}
