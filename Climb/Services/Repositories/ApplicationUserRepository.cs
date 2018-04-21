using System.Threading.Tasks;
using Climb.Data;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ApplicationUser> GetByEmail(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}