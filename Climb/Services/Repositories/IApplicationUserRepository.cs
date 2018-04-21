using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetByEmail(string email);
    }
}