using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApi.Models
{
    public class Reservation{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; }
        public int RoomId {get; set;}
        public virtual Room Room { get; set;}
        public int UserId {get; set;}
        public virtual User User {get; set;}
    }
}