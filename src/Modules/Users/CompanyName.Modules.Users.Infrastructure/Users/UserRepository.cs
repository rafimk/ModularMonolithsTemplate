using CompanyName.Modules.Users.Domain.Users;
using CompanyName.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public void Insert(User user)
    {
        // Role.Member/Role.Administrator are static singletons seeded via migration HasData, never
        // queried through this DbContext, so EF treats them as new entities to insert unless attached.
        foreach (Role role in user.Roles)
        {
            if (dbContext.Entry(role).State == EntityState.Detached)
            {
                dbContext.Set<Role>().Attach(role);
            }
        }

        dbContext.Users.Add(user);
    }
}
