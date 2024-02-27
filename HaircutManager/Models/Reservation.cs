using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaircutManager.Models
{
    public class Reservation
    {

        public int ReservationId { get; set; }

        [Required]
        [StringLength(100)]
        public string ReservationName { get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set;}

        [Required]
        [StringLength(100)]
        public int ClientName { get; set;}

        [DataType(DataType.EmailAddress)]
        public string ClientEmail { get; set;}

        [DataType(DataType.PhoneNumber)]
        public string ClientPhoneNumber { get; set;}



        [Required]
        public int ServiceId { get; set; } // Klucz obcy dla Usługi

        // Relacja: każda rezerwacja dotyczy jednej usługi
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

    }
}
