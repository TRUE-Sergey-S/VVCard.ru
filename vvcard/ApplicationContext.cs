using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using vvcard.Models;

namespace vvcard
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardField> CardFields { get; set; }
        public virtual DbSet<VisitСounter> VisitСounters { get; set; }
        public virtual DbSet<ClickCardField> ClickCardFields { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
           //Database.EnsureDeleted();
           //Database.EnsureCreated();
        }
    }
}
