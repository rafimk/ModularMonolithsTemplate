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
        dbContext.Users.Add(user);
    }
}
