using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaircutManager.Models
{
    public class Reservation
    {

        public int ReservationId { get; set; }

        public DateTime ReservationDate { get; set;}
 
        public string ClientName { get; set;}
       
        public string ClientEmail { get; set;}
        
        public string ClientPhoneNumber { get; set;}

        public int ServiceId { get; set; } // Klucz obcy dla Usługi

        // Relacja: każda rezerwacja dotyczy jednej usługi
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

    }
}
