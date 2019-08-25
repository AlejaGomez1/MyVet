using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data.Entities;

namespace MyVet.Web.Models
{
    public class HistoryViewModel : History /*(hereda de historia)*/
    {
        public int PetId { get; set; } /*(El modelo History tiene relacion con Pet, pero esta relacion es con el objeto completo, pero cuando se manda el modelo entre vista y controlador, no se puede mandar el objeto completo, entonces solo se manda el Id y luego con el Id recupero la mascota)*/

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Service Type")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a service type.")]
        public int ServiceTypeId { get; set; } /*(El servicio seleccionado en el combo box)*/

        public IEnumerable<SelectListItem> ServiceTypes { get; set; } /*(SelectListItem es el origen del DropDownList)*/
    }
}
