using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApi.Models
{
    public class Reservation{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; }

        public int RoomId {get; set;}

        [ForeignKey("RoomId")]
        public Room Room { get; set;}

        public int UserId {get; set;}
        [ForeignKey("UserId")]
        public User User {get; set;}

        public string Date {get; set;}
    }
}