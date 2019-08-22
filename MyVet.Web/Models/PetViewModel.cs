using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data.Entities;

namespace MyVet.Web.Models
{
    public class PetViewModel : Pet /*(PetViewModel hereda de la clase pet, al heredar de la clase pet, ya tiene todos los atributos de mascota)*/
    {
        public int OwnerId { get; set; } /*(Al crear la mascota yo le digo de que propietario es)*/

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Pet Type")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a pet type.")]
        public int PetTypeId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public IEnumerable<SelectListItem> PetTypes { get; set; } /*(lista de mascotas)*/
    }
}
