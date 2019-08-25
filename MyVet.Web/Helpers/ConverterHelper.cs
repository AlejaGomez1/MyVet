using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(
            DataContext dataContext,
            ICombosHelper combosHelper )
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public async Task<Pet> ToPetAsync(PetViewModel model, string path, bool isNew) /*(al metodo ToPet le mandamos el modelo y la ruta y el metodo devuelve
            un objeto pet. Este metodo convierte un objeto de la clase PetViewModel en un objeto de la clase pet)*/
        {
            var pet = new Pet
            {
                Agendas = model.Agendas,
                Born = model.Born,
                Histories = model.Histories,     
                Id = isNew ? 0 : model.Id,
                ImageUrl = path,
                Name = model.Name,
                Owner = await _dataContext.Owners.FindAsync(model.OwnerId),
                PetType = await _dataContext.PetTypes.FindAsync(model.PetTypeId),
                Race = model.Race,
                Remarks = model.Remarks
            };

            return pet;
        }

        public PetViewModel ToPetViewModel(Pet pet) /*(este metodo retorna una nueva instancia del objeto PetViewModel)*/
        {
            return new PetViewModel
            {
                Agendas = pet.Agendas,
                Born = pet.Born,
                Histories = pet.Histories,
                ImageUrl = pet.ImageUrl,
                Name = pet.Name,
                Owner = pet.Owner,
                PetType = pet.PetType,
                Race = pet.Race,
                Remarks = pet.Remarks,
                Id = pet.Id,
                OwnerId = pet.Owner.Id,
                PetTypeId = pet.PetType.Id,
                PetTypes = _combosHelper.GetComboPetTypes()
            };
        }

        public async Task<History> ToHistoryAsync(HistoryViewModel model, bool isNew) /*(Metodo que convierte de HistoryViewModel a History)*/
        {
            return new History
            {
                Date = model.Date.ToUniversalTime(),
                Description = model.Description,
                Id = isNew ? 0 : model.Id, /*(Si el registro es nuevo le mandamos Id cero, si estamos editando le mandamos el id que vamos a editar)*/
                Pet = await _dataContext.Pets.FindAsync(model.PetId),
                Remarks = model.Remarks,
                ServiceType = await _dataContext.ServiceTypes.FindAsync(model.ServiceTypeId)
            };
        }

        public HistoryViewModel ToHistoryViewModel(History history) /*(Metodo que le mandamos un History y lo convierte a HistoryViewModel)*/
        {
            return new HistoryViewModel
            {
                Date = history.Date,
                Description = history.Description,
                Id = history.Id,
                PetId = history.Pet.Id,
                Remarks = history.Remarks,
                ServiceTypeId = history.ServiceType.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes()
            };
        }

    }
}
