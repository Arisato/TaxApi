using System;
using System.Collections.Generic;
using DataEF.Models;
using Microsoft.EntityFrameworkCore;

namespace DataEF;

public partial class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Bracket> Brackets { get; set; }

    public virtual DbSet<Ledger> Ledgers { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bracket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brackets__3214EC078BFD2B12");

            entity.HasIndex(e => e.Id, "UQ__Brackets__3214EC066BFF9334").IsUnique();

            entity.HasIndex(e => e.Category, "UQ__Brackets__9EA680CB61D9A141").IsUnique();

            entity.Property(e => e.Category).HasColumnType("decimal(3, 1)");
        });

        modelBuilder.Entity<Ledger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ledgers__3214EC07A8B0888D");

            entity.HasIndex(e => e.Id, "UQ__Ledgers__3214EC06BB2BDD85").IsUnique();

            entity.HasOne(d => d.Bracket).WithMany(p => p.Ledgers)
                .HasForeignKey(d => d.BracketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Brackets_Ledgers");

            entity.HasOne(d => d.Municipality).WithMany(p => p.Ledgers)
                .HasForeignKey(d => d.MunicipalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Municipalities_Ledgers");
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Municipa__3214EC0704B69FA3");

            entity.HasIndex(e => e.Id, "UQ__Municipa__3214EC06F6E9C847").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__Municipa__8D313B0357D3CE64").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
