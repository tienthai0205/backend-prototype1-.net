using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApi.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; }
        public string Number { get; set; }
        public string Floor { get; set; }
        public string Description { get; set; }
        
        public List<Reservation> Reservations { get; set; }
    }
    
}