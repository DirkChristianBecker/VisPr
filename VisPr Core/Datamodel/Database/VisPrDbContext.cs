using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;
using VisPrCore.Datamodel.Database.ApplicationModeller;
using Microsoft.Extensions.Configuration;
using VisPrCore.Datamodel;

namespace VisPrCore.Datamodel.Database
{
    public class VisPrDbContext : IdentityDbContext
    {
        [NotMapped] private string ConnectionString { get; set; }
        [NotMapped] private IConfiguration Configuration { get; set; }

        public DbSet<BusinessObject> BusinessObjects { get; set; } = default!;
        public DbSet<ApplicationElement> ApplicationElements { get; set; } = default!;
        public DbSet<ElementSelector> ElementSelectors { get; set; } = default!;

        public VisPrDbContext(IConfiguration configuration, DbContextOptions<VisPrDbContext> options)
            : base(options)
        {
            Configuration = configuration;
            ConnectionString = Configuration.ReadConnectionString();            
        }

        public VisPrDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.ReadConnectionString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default users and roles
            var admin_role_id = Guid.NewGuid().ToString();
            var dev_role_id = Guid.NewGuid().ToString();
            var user_role_id = Guid.NewGuid().ToString();

            var admin_id = Guid.NewGuid().ToString();
            var dev_id = Guid.NewGuid().ToString(); 
            var user_id = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            { 
                Id = admin_role_id,
                Name = Names.AdminName, 
                NormalizedName = Names.AdminName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = dev_role_id,
                Name = Names.DevName,
                NormalizedName = Names.DevName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = user_role_id,
                Name = Names.UsersName,
                NormalizedName = Names.UsersName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

            PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
            var admin = new IdentityUser
            {
                Id = admin_id,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "admin@admin.com"
            };
            admin.PasswordHash = ph.HashPassword(admin, "admin");

            var userName = "Developer";
            var dev = new IdentityUser
            {
                Id = dev_id,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "dev@dev.com"
            };
            dev.PasswordHash = ph.HashPassword(dev, "developer");

            userName = "User";
            var user = new IdentityUser
            {
                Id = user_id,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "user@user.com"
            };
            user.PasswordHash = ph.HashPassword(dev, "user");

            modelBuilder.Entity<IdentityUser>().HasData(admin);
            modelBuilder.Entity<IdentityUser>().HasData(dev);
            modelBuilder.Entity<IdentityUser>().HasData(user);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = admin_role_id,
                UserId = admin.Id
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = dev_role_id,
                UserId = dev.Id
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = user_role_id,
                UserId = user.Id
            });

            // Indices
            modelBuilder.Entity<BusinessObject>()
                .HasIndex(u => u.Name)
                .IsUnique();

            // Trees
            // https://habr.com/en/post/516596/
            modelBuilder.Entity<ApplicationElement>(
                entity =>
                {
                    entity.HasKey(x => x.Id);
                    entity.Property(x => x.Name);
                    entity.HasOne(x => x.Parent)
                        .WithMany(x => x.Children)
                        .HasForeignKey(x => x.ParentId)
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
