using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using LrsysIntegration.Models;

namespace LrsysIntegration.DataLogic
{
    public class LrsysContext : DbContext
    {
        public LrsysContext() : base("APIString")
        {
            // Don't let EF try to create/alter the existing DB by default
            Database.SetInitializer<LrsysContext>(null);

            // Optional: adjust EF behavior for serialization and performance
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Employees> Employees { get; set; }

        public DbSet<ApplicationErrorLog> ApplicationErrorLog { get; set; }


        //public DbSet<CustomerModel> Customers { get; set; }
        // Add other DbSet<T> properties that map to your database tables

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Prevent EF from pluralizing table names if your tables are singular
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Example explicit mappings (uncomment and adjust if needed)
            // modelBuilder.Entity<LoginModel>().ToTable("Login");
            // modelBuilder.Entity<CustomerModel>().ToTable("Customer");

            base.OnModelCreating(modelBuilder);
        }
    }
}