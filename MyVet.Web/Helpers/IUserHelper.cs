using Microsoft.AspNetCore.Identity;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email); /*(GetUserByEmailAsync es un metodo que le envio el email como parametro y el me devuelve un objeto user)*/ 

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName); /*(Matricula un usuario a un rol)*/

        Task<bool> IsUserInRoleAsync(User user, string roleName); /*(Devuelve un booleano de respuesta)*/

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<bool> DeleteUserAsync(string email);
    }
}
