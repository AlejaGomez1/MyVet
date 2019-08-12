using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Data.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        public User User { get; set; }
       
        //public string FullName (forma vieja)
        //{
        //    get
        //    {
        //        return $"{FirstName} {LastName}";
        //    }
        //}

        public ICollection<Pet> Pets { get; set; } /*(1 propietario tiene muchas mascotas, una coleccion de mascotas)*/

        public ICollection<Agenda> Agendas { get; set; } /*(1 propietario tiene muchas entradas en la agenda)*/

    }
}
