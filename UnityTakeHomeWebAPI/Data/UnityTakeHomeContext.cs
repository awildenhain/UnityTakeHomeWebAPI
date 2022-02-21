using Microsoft.EntityFrameworkCore;

namespace UnityTakeHomeWebAPI.Data
{
    public class UnityTakeHomeContext : DbContext
    {
        public UnityTakeHomeContext(DbContextOptions<UnityTakeHomeContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
