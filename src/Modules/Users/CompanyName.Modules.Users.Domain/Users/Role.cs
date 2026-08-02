namespace CompanyName.Modules.Users.Domain.Users;

public sealed class Role
{
    public static readonly Role Member = new("Member");
    public static readonly Role Administrator = new("Administrator");

    private Role(string name)
    {
        Name = name;
    }

    private Role()
    {
    }

    public string Name { get; private set; } = null!;
}
