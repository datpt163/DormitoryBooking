using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingWebApi.Common.Data
{
    public partial class dormitorybookingbookingContext : DbContext
    {
        public dormitorybookingbookingContext()
        {
        }

        public dormitorybookingbookingContext(DbContextOptions<dormitorybookingbookingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
              
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");

                entity.HasIndex(e => e.SemesterId, "fk_students_classes3");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_students_classes3");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("semester");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
