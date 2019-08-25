using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OwnersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public OwnersController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper) /*(el controlador owner inyecta el DataContext, 'context es la representacion de toda la bd')*/
        {
            _dataContext = context; /*(La conexion llega inyectada al controlador, con solo inyectarlo puedo acceder a los datos)*/
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Owners /*(A la vista le estoy mandando solo la sentencia sql)*/
                .Include(o => o.User)/* (Aqui estoy haciendo el inner join de owner con user, una expresiòn lamba que le digo que me incluya 
            de los owner, me incluya el owners inner join user.Cuando llegue al index ejecuta la consulta */
                .Include(o => o.Pets));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners /*(Select * from owners where id=id enviado como parametro)*/
                .Include(o => o.User) /*(para los detalles tambièn debo enviar la relacion sino el campo user y pets van a llegar nulos)*/
                .Include(o => o.Pets)
                .ThenInclude(p => p.PetType)
                .Include(o => o.Pets)
                .ThenInclude(p => p.Histories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Address = model.Address, /*(el addres y todos los campos de lo que devuelve el modelo)*/
                    Document = model.Document,
                    Email = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username

                };

                var response = await _userHelper.AddUserAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);/*(En esta parte obtenemos el usuario y le agregamos rol)*/
                    await _userHelper.AddUserToRoleAsync(userInDB, "Customer");

                    var owner = new Owner
                    {
                        Agendas = new List<Agenda>(), /*(un usuario nuevo no tiene agenda, pero se manda una lista vacia, lo mismo para la mascota )*/
                        Pets = new List<Pet>(),
                        User = userInDB /*(Hasta este momento el usuario esta en memoria)*/
                    };

                    _dataContext.Owners.Add(owner); /*(Aqui estoy haciendo el insert)*/
                    try
                    {
                        await _dataContext.SaveChangesAsync();/*(Aqui hago commit de la transaccion)*/
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }

                }

                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }
            return View(model);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Update(owner);
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Pets)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            if (owner.Pets.Count > 0)
            {
                //TODO: Message
               return RedirectToAction(nameof(Index));
            }

            await _userHelper.DeleteUserAsync(owner.User.Email);

            _dataContext.Owners.Remove(owner);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool OwnerExists(int id)
        {
            return _dataContext.Owners.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id.Value); /*(Select * from owners where id=id enviado como parametro)*/
            if (owner == null)
            {
                return NotFound();
            }

            var model = new PetViewModel /*(un objeto petviewmodel, model es el que se le va a mandar a la vista, con algunos datos precargados )*/
            {
                Born = DateTime.Today,
                OwnerId = owner.Id,
                PetTypes = _combosHelper.GetComboPetTypes() /*(PetTypes es el combo donde voy a pintar la lista de tipos de mascota)*/
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var pet = await _converterHelper.ToPetAsync(model, path, true);
                _dataContext.Pets.Add(pet);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }
            model.PetTypes = _combosHelper.GetComboPetTypes();/*(si hay un error le volvemos a precargar al modelo la lista de mascotas)*/
            return View(model);
        }
        public async Task<IActionResult> EditPet(int? id) /*(en este contexto id no es el owner, es la mascota)*/
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _dataContext.Pets
                .Include(p => p.Owner)
                .Include(p => p.PetType)
                .FirstOrDefaultAsync(p => p.Id == id ); /*(Select * from owners where id=id enviado como parametro)*/
            if (pet == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToPetViewModel(pet));
        }

        [HttpPost]
        public async Task<IActionResult> EditPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var pet = await _converterHelper.ToPetAsync(model, path, false);
                _dataContext.Pets.Update(pet);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }
            model.PetTypes = _combosHelper.GetComboPetTypes();
            return View(model);
        }
        public async Task<IActionResult> DetailsPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _dataContext.Pets
                .Include(p => p.Owner)
                .ThenInclude(o => o.User)
                .Include(p => p.Histories)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        public async Task<IActionResult> AddHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _dataContext.Pets.FindAsync(id.Value);
            if (pet == null)
            {
                return NotFound();
            }

            var model = new HistoryViewModel
            {
                Date = DateTime.Now, /*(En el front end muestro la fecha actual)*/
                PetId = pet.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, true);
                _dataContext.Histories.Add(history);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }

            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
        }

        public async Task<IActionResult> EditHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _dataContext.Histories
                .Include(h => h.Pet)
                .Include(h => h.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (history == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToHistoryViewModel(history));
        }

        [HttpPost]
        public async Task<IActionResult> EditHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, false);
                _dataContext.Histories.Update(history);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }

            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
        }
        public async Task<IActionResult> DeleteHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _dataContext.Histories
                .Include(h => h.Pet) 
                .FirstOrDefaultAsync(h => h.Id == id.Value);
            if (history == null)
            {
                return NotFound();
            }

            _dataContext.Histories.Remove(history);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsPet)}/{history.Pet.Id}");
        }

        public async Task<IActionResult> DeletePet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _dataContext.Pets
                .Include(p => p.Owner)
                .Include(p => p.Histories)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (pet == null)
            {
                return NotFound();
            }

            if(pet.Histories.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The pet can't be deleted because it has relared records.");
                return RedirectToAction($"{nameof(Details)}/{pet.Owner.Id}");
            }
                         
            _dataContext.Pets.Remove(pet);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{pet.Owner.Id}");
        }

    }
}
