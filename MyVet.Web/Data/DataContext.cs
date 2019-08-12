using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Data
{
    public class DataContext : IdentityDbContext<User> /*(Hereda del IdentityDbContext y la clase de usuarios personalizada de mi BD es user)*/
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet <Agenda> Agendas { get; set; } /*(vamos a tener un entity Owner y la base de datos a contener ese modelo )*/

        public DbSet<History> Histories { get; set; }

        public DbSet<Owner> Owners { get; set; }

        public DbSet<Manager> Managers { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<PetType> PetTypes { get; set; }

        public DbSet<ServiceType> ServiceTypes { get; set; }

    }
}
