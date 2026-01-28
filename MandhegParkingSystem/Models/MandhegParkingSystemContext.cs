using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Models;

public partial class MandhegParkingSystemContext : DbContext
{
    public MandhegParkingSystemContext()
    {
    }

    public MandhegParkingSystemContext(DbContextOptions<MandhegParkingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<HourlyRate> HourlyRates { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<ParkingDatum> ParkingData { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MandhegParkingSystem");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3213E83F3BD336D4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<HourlyRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HourlyRa__3213E83FCCF35C78");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Membership).WithMany(p => p.HourlyRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HourlyRat__membe__5535A963");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.HourlyRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HourlyRat__vehic__5629CD9C");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Member__3213E83F2DA8C2BE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Membership).WithMany(p => p.Members)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Member__membersh__5AEE82B9");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3213E83F02BEA4D9");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ParkingDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ParkingD__3213E83F91727C65");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Employee).WithMany(p => p.ParkingData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingDa__emplo__656C112C");

            entity.HasOne(d => d.HourlyRates).WithMany(p => p.ParkingData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingDa__hourl__66603565");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.ParkingData).HasConstraintName("FK__ParkingDa__vehic__6477ECF3");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicle__3213E83F347452AD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Member).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicle__member___60A75C0F");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicle__vehicle__5FB337D6");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3213E83FC87D619A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
