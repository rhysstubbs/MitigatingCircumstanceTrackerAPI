using Microsoft.EntityFrameworkCore;

namespace MCT.DataAccess.Models
{
    public class EFContext : DbContext
    {
        private DbSet<Request> Requests { get; set; }
        private DbSet<Subject> Subjects { get; set; }
        private DbSet<File> Files { get; set; }
        private DbSet<RequestFile> RequestFiles {get; set;}
        private DbSet<RequestSubject> RequestSubjects { get; set; }

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
                //optionsBuilder.UseSqlServer("Server=desktop-ruplnkq\\sql2017;Database=MCT;Trusted_Connection=True;");
                //optionsBuilder.UseMySql("server=127.0.0.1;database=mct;user=root;password=root");
                //optionsBuilder.UseMySql("Server=127.0.0.1;Database=mct;Uid=rhys;Pwd=Raw54arc3;");
                throw new System.Configuration.ConfigurationErrorsException();
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
                .HasKey(t => new { t.RequestId, t.SubjectId });
        }
    }
}