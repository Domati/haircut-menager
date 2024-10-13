using HaircutManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HaircutManager.Data
{
    public class AppDbContext : IdentityDbContext <ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<OldPassword> OldPasswords { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Service>().HasData(

                    new Service {ServiceId= 1, ServiceName = "Strzyżenie męskie", Description = "Krótkie strzyżenie męskie", Price = 50.00m, AvgTimeOfService = 30 },
                    new Service {ServiceId = 2 ,ServiceName = "Strzyżenie damskie", Description = "Strzyżenie damskie wraz z modelowaniem", Price = 70.00m, AvgTimeOfService = 45 },      
                    new Service {ServiceId =  3,ServiceName = "Farbowanie", Description = "Farbowanie włosów", Price = 150, AvgTimeOfService = 75 },
                    new Service {ServiceId =  4,ServiceName = "Trwała", Description = "Zabieg chemicznego podkręcenia włosów", Price = 130, AvgTimeOfService = 120 },
                    new Service {ServiceId =  5,ServiceName = "Pasemka", Description = "Pasmowe farbowanie włosów", Price = 200, AvgTimeOfService = 150 }
                );
        }

    
                }

    }

