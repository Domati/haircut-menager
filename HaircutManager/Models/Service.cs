using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HaircutManager.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        
        public string ServiceName { get; set; } // nazwa usługi

        public string Description { get; set; } // opis usługi

        public string Price { get; set; } // cena usługi

        public string AvgTimeOfService { get; set; } // przewidywany czas trwania usługi

        public List<Reservation> Reservations { get; set; } // Relacja: jedna usługa może być przypisana do wielu rezerwacji

    }
}
