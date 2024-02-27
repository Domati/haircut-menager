using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HaircutManager.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string NameOfService { get; set; } // nazwa usługi

        [StringLength(500)]
        public string Description { get; set; } // opis usługi

        [DataType(DataType.Currency)]
        public string Price { get; set; } // cena usługi

        [DataType(DataType.Time)]
        public DateTime AvgTimeOfService { get; set; } // przewidywany czas trwania usługi

        public List<Reservation> Reservations { get; set; } // Relacja: jedna usługa może być przypisana do wielu rezerwacji

    }
}
