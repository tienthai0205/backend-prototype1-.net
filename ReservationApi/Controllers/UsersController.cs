using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationApi.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ReservationApi.Controllers

{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserContext _context;

        public UsersController(ILogger<UsersController> logger, UserContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            List<User> users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register([FromBody] User body)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(body.Password);
            User existingUser = _context.Users.Where(user => user.Email == body.Email).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest(new { error = "User with that email already exists!" });
            }
            User newUser = new User()
            {
                Email = body.Email,
                Password = passwordHash
            };
            _context.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(newUser);
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] User body)
        {
            User user = _context.Users.Where(usr => usr.Email == body.Email).FirstOrDefault();
            if (user == null)
            {
                return BadRequest(new { error = "User not found!" });
            }
            string hashedPwd = user.Password;
            bool verified = BCrypt.Net.BCrypt.Verify(body.Password, hashedPwd);

            if (!verified)
            {

            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            //return jwt here
            return Ok(new {
                accessToken = access_token,
                email = user.Email
            });
        }
    }
}
