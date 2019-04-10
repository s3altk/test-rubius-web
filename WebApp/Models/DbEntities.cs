using Microsoft.EntityFrameworkCore;

namespace TestRubius.Models
{
    public partial class DbEntities : DbContext
    {
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Record> Record { get; set; }

        public DbEntities(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Record)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Record_Project");
            });
        }
    }
}
