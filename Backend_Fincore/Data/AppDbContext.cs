using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

    }
}
