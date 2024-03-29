using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;


namespace ReservationApi.Controllers

{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }
        // GET api/users/seed
        [Route("seed")]
        [HttpGet]
        public async Task<RedirectToActionResult> Seed()
        {
            _context.Users.Add(new User { Id = 1, Email = "tien@saxion.nl", Password = BCrypt.Net.BCrypt.HashPassword("Tien12345") });
            _context.Users.Add(new User { Id = 2, Email = "max@saxion.nl", Password = BCrypt.Net.BCrypt.HashPassword("Max12345") });

            _context.Rooms.Add(new Room { Id = 1, Number = "1A", Floor = "1st" });
            _context.Rooms.Add(new Room { Id = 2, Number = "2A", Floor = "1st" });
            _context.Rooms.Add(new Room { Id = 3, Number = "3A", Floor = "1st" });
            _context.Rooms.Add(new Room { Id = 4, Number = "1B", Floor = "2nd" });
            _context.Rooms.Add(new Room { Id = 5, Number = "2B", Floor = "2nd" });
            _context.Rooms.Add(new Room { Id = 6, Number = "3A", Floor = "2nd" });

            // _context.Reservations.Add(new Reservation{ Id = 1, RoomId = 1, UserId = 1, Date = "01/02/2021"});

            await _context.SaveChangesAsync();
            return RedirectToAction(actionName: "GetUsers", controllerName: "users");
        }
        // GET api/users
        [Authorize]
        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            List<User> users = _context.Users.ToList();
            return Ok(users);
        }
        // POST api/users
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

        // POST api/users/login
        [Route("login")]
        [HttpPost]
        public ActionResult<User> Login([FromBody] User body)
        {
            User user = _context.Users.Where(usr => usr.Email == body.Email).FirstOrDefault();
            if (user == null)
            {
                return NotFound(new { error = "User not found!" });
            }
            if (HttpContext.Session.GetString("email") != null){
                return BadRequest(new { error = "Sorry you're already logged in!" });
            }
            string hashedPwd = user.Password;
            bool verified = BCrypt.Net.BCrypt.Verify(body.Password, hashedPwd);

            if (!verified)
            {
                return BadRequest(new { error = "Username or password incorrect" });
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
                signingCredentials: signingCredentials);
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            HttpContext.Session.SetString("email", user.Email);

            return Ok(new
            {
                accessToken = access_token,
                email = user.Email
            });
        }

        [Route("logout")]
        [Authorize]
        [HttpPost]
        public ActionResult<User> Logout(){
            HttpContext.Session.Remove("email");
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> RemoveAccount(int id)
        {
            if (!IsAuthorized(id))
            {
                return Unauthorized(new { error = "You are not authorized to delete other user!" });
            }
            User user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { error = "User not found!" });
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Your account has been removed!" });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateAccount(int id, [FromBody] User userIn)
        {

            if (!IsAuthorized(id))
            {
                return Unauthorized(new { error = "You are not authorized to update other user accounts!" });
            }
            if (id != userIn.Id)
            {
                return BadRequest(new { error = "The provided ids don't match" });
            }
            User user = _context.Users.Find(id);
            user.Email = userIn.Email;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userIn.Password);
            user.Password = passwordHash;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Your account has been updated!" });
        }


        public bool IsAuthorized(int id)
        {
            var accessToken = HttpContext.GetTokenAsync("access_token");
            var jwt = accessToken.Result;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            if (token.Subject != id.ToString())
            {
                return false;
            }
            return true;
        }
    }
}
