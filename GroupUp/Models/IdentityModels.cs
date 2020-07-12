using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GroupUp.Models
{
    // ApplicationUser sınıfınıza daha fazla özellik ekleyerek kullanıcıya profil verileri ekleyebilirsiniz. Daha fazla bilgi için lütfen https://go.microsoft.com/fwlink/?LinkID=317594 adresini ziyaret edin.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public User User { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType özelliğinin CookieAuthenticationOptions.AuthenticationType içinde tanımlanmış olanla eşleşmesi gerektiğini unutmayın
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Özel kullanıcı taleplerini buraya ekle
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public new DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasRequired(u => u.AspNetIdentity)
                .WithRequiredPrincipal(a => a.User);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithMany(u => u.Groups)
                .Map(m
                    =>
                    {
                    m.ToTable("UserGroups");
                    m.MapLeftKey("GroupId");
                    m.MapRightKey("UserId");
                    });
            modelBuilder.Entity<Group>()
                .HasRequired(g => g.Creator);
        }
    }
}