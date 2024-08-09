using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RoomWebApi.Common.Data
{
    public partial class dormitorybookingroomContext : DbContext
    {
        public dormitorybookingroomContext()
        {
        }

        public dormitorybookingroomContext(DbContextOptions<dormitorybookingroomContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Building> Buildings { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Roomtype> Roomtypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=dormitorybookingroom;user=root;password=dat123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.3.0-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Building>(entity =>
            {
                entity.ToTable("building");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("room");

                entity.HasIndex(e => e.TypeId, "fk_students_classes");

                entity.HasIndex(e => e.BuildingId, "fk_students_classes2");

                entity.Property(e => e.IsAvailble).HasColumnType("bit(1)");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_students_classes2");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_students_classes");
            });

            modelBuilder.Entity<Roomtype>(entity =>
            {
                entity.ToTable("roomtype");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(150)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
