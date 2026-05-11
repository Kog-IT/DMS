namespace DMS.EntityFrameworkCore.Seed.Host;

public class InitialHostDbBuilder
{
    private readonly DMSDbContext _context;

    public InitialHostDbBuilder(DMSDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        new DefaultEditionCreator(_context).Create();
        new DefaultLanguagesCreator(_context).Create();
        new HostRoleAndUserCreator(_context).Create();
        new DefaultSettingsCreator(_context).Create();
        new DefaultGovernorateCreator(_context).Create();
        new DefaultCityCreator(_context).Create();

        _context.SaveChanges();
    }
}
