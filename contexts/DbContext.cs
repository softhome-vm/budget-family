using Microsoft.EntityFrameworkCore;
using budget.models;

namespace budget.contexts
{
    public class OurDbContext : DbContext
    {
        private readonly string _connectDb = "server=127.0.0.1;" +
            "port=5432;" +
            "userid=postgres;" +
            "password=12345;" +
            "database=Budget-family;" +
            "SSL Mode=Disable;" +
            "Include Error Detail=true;" +
            "Timezone=Europe/Moscow";

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql(_connectDb);
        }

        public DbSet<Accouts> Accouts { get; set; }
        public DbSet<Budgets> Budgets { get; set; }
        public DbSet<Family> Family { get; set; }
        public DbSet<Kategories> Kategories { get; set; }
        public DbSet<Transitions> Transitions { get; set; }
    }
}
