using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _dataContext;

        public CombosHelper(DataContext dataContext) /*(Para hacer combo debe haber una conexion con la BD, lo que quiere 
            decir que a esta clase "CombosHelper se le debe inyectar una conexion a la BD, donde esta el comentario es un 
            constructor.Y con esto (DataContext dataContext) le estoy inyectando la conexion a la BD)*/
        {
            _dataContext = dataContext;
        }

        public IEnumerable<SelectListItem> GetComboPetTypes()
        {
            //var list = new List<SelectListItem>();
            //foreach (var petType in _dataContext.PetTypes) *ESTO ES UNA INSTRUCCION INEFICIENTE*
            //{
            //    list.Add(new SelectListItem
            //    {
            //        Text = petType.Name,
            //        Value = $"{petType.Id}"
            //    });
            //}

            var list = _dataContext.PetTypes.Select(pt => new SelectListItem
            {
                Text = pt.Name,
                Value = $"{pt.Id}"
            })
                .OrderBy(pt => pt.Text)
                .ToList();

            list.Insert(0, new SelectListItem /*(Para configurar la primera opcion de la lista)*/
            {
                Text = "[Select a pet type ...]",
                Value = "0",
            });


            return list;
        }
    }
}
