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

        // GET: api/Rooms
        // [Authorize]
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        // {
        //     return await _context.Rooms.ToListAsync();
        // }

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

        [Authorize]
        // GET: api/Rooms/5
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

        // PUT: api/Rooms/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
