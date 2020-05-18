using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SP.Core.Model;

namespace SP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // внешняя ссылка на таблицу dbo.AspNetUsers
            modelBuilder.Entity("SP.Core.Model.Person", b =>
            {
                b.HasOne("SP.Data.ApplicationUser", null)
                    .WithMany()
                    .HasForeignKey("AspNetUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });
        }

        public DbSet<Person> Persons { get; set; }
    }
}
