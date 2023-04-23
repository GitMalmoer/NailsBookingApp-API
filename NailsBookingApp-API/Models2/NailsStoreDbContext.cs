using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NailsBookingApp_API.Models2;

public partial class NailsStoreDbContext : DbContext
{
    public NailsStoreDbContext()
    {
    }

    public NailsStoreDbContext(DbContextOptions<NailsStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Log> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NailsStoreDB;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Log");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Logged).HasColumnType("datetime");
            entity.Property(e => e.Logger).HasMaxLength(250);
            entity.Property(e => e.MachineName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
