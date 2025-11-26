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
            entity.HasKey(e => e.Id).HasName("PK__Brackets__3214EC072D3868F9");

            entity.HasIndex(e => e.Id, "UQ__Brackets__3214EC06A41D6B88").IsUnique();

            entity.HasIndex(e => e.Category, "UQ__Brackets__4BB73C3294543B29").IsUnique();

            entity.Property(e => e.Category).HasColumnType("decimal(3, 1)");
        });

        modelBuilder.Entity<Ledger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ledgers__3214EC0767890A07");

            entity.HasIndex(e => e.Id, "UQ__Ledgers__3214EC069CE6108D").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Municipa__3214EC079BF33FDA");

            entity.HasIndex(e => e.Id, "UQ__Municipa__3214EC060EDFBC96").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__Municipa__737584F66F6049D6").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
