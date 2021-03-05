using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationApi.Models;
using Microsoft.Extensions.Logging;

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
            List<User> users = _context.getUsers();
            return Ok(users);
        }
        
        [HttpPost]
        public IActionResult Login([FromBody]User body)
        {
            return Ok(body);
        }
    }
}
