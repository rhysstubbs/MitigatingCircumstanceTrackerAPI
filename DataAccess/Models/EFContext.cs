using Microsoft.EntityFrameworkCore;

namespace MCT.DataAccess.Models
{
    public class EFContext : DbContext
    {
        private DbSet<Request> Requests { get; set; }
        private DbSet<Subject> Subjects { get; set; }

        public EFContext()
        {
        }

        public EFContext(DbContextOptions<EFContext> options)
         : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=desktop-ruplnkq\\sql2017;Database=MCT;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>();

            modelBuilder.Entity<Subject>();

            modelBuilder.Entity<File>();

            modelBuilder.Entity<RequestFile>()
                .HasKey(t => new { t.RequestId, t.FileId });

            modelBuilder.Entity<RequestSubject>()
                .HasKey(t => new { t.RequestId, t. SubjectId });
        }
    }
}