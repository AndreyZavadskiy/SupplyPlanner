using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SP.Core.Master;
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

        // таблицы с данными
        public DbSet<Person> Persons { get; set; }
        public DbSet<Region> Regions { get; set; }
        // справочники
        public DbSet<CashboxLocation> CashboxLocations { get; set; }
        public DbSet<ClientRestroom> ClientRestrooms { get; set; }
        public DbSet<ManagementSystem> ManagementSystems { get; set; }
        public DbSet<OperatorRoomFormat> OperatorRoomFormats { get; set; }
        public DbSet<ServiceLevel> ServiceLevels { get; set; }
        public DbSet<StationLocation> StationLocations { get; set; }
        public DbSet<StationStatus> StationStatuses { get; set; }
        public DbSet<TradingHallOperatingMode> TradingHallOperatingModes { get; set; }
        public DbSet<TradingHallSize> TradingHallSizes { get; set; }

    }
}
