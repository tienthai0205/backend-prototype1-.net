using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationApi.Models;

namespace ReservationApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly UserContext _context;

        public ReservationsController(UserContext context)
        {
            _context = context;
        }

        // GET: api/reservations
        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetReservations()
        {
            List<Reservation> myReservations = _context.Reservations.Where(re => re.UserId == CurrentUserId()).ToList();
            return myReservations;
        }
        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult> PostReservation([FromBody]Reservation reservation)
        {
            Room room = _context.Rooms.Find(reservation.RoomId);
            User user = _context.Users.Find(reservation.UserId);
            reservation.Room = room;
            reservation.User = user;
            await _context.Reservations.AddAsync(reservation);
            AddReservationToRoom(room.Id, reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        // PUT: api/reservations/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new {error = "Reservation does not exist!"});
                }
                else
                {
                    throw;
                }
            }

            return Ok(new {message = "Your reservation has been updated!"});
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { error = "Reservation does not exist!"});
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok( new {message = "Your reservation has been removed!"});
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        private int CurrentUserId(){
            var accessToken = HttpContext.GetTokenAsync("access_token");
            var jwt = accessToken.Result;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return Int32.Parse(token.Subject);
        }

        private void AddReservationToRoom(int roomId, Reservation reservation){
            _context.Rooms.Find(roomId).Reservations.Add(reservation);
            _context.SaveChangesAsync();
        }
    }
}
