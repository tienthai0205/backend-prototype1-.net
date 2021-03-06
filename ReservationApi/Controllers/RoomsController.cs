using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationApi.Models;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly UserContext _context;

        public RoomsController(UserContext context)
        {
            _context = context;
        }

        // GET: api/rooms/filter?date=01/02/2021
        [Route("filter")]
        [HttpGet]
        public async Task<ActionResult<List<Room>>> GetRoomsByDate([FromQuery(Name = "date")] string date)
        {
            List<Reservation> reservations = _context.Reservations.Where(re => re.Date == date).ToList();
            if (reservations.Count == 0){
                return await _context.Rooms.ToListAsync();
            }
            List<int> bookedRooms = new List<int>();
            foreach( Reservation re in reservations){
                bookedRooms.Add(re.RoomId);
            }
            List<Room> allRooms = await _context.Rooms.ToListAsync();
            List<Room> availableRooms = allRooms.Where(x => !bookedRooms.Any(y => x.Id == y)).ToList();
            return Ok(availableRooms);
        }

        // GET: api/rooms
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        [Authorize]
        // GET: api/rooms/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

    }
}
