using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Entities;

public partial class AccelokaContext : DbContext
{
    public AccelokaContext()
    {
    }

    public AccelokaContext(DbContextOptions<AccelokaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookedTicket> BookedTickets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=Acceloka;Integrated Security=True;Encrypt=False");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookedTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookedTi__3213E83F0781BEB0");

            entity.ToTable("BookedTicket");

            entity.HasIndex(e => new { e.BookedTicketId, e.TicketCode }, "UQ_BookedTicket_BookedTicketId_TicketCode").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BookedTicketId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookedTicketId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ticketCode");

            entity.HasOne(d => d.TicketCodeNavigation).WithMany(p => p.BookedTickets)
                .HasForeignKey(d => d.TicketCode)
                .HasConstraintName("FK__BookedTic__ticke__46E78A0C");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__23CAF1D89350D2E3");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categoryId");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("categoryName");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketCode).HasName("PK__Tickets__FCC3B0008C226C19");

            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ticketCode");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categoryId");
            entity.Property(e => e.EventDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("eventDate");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quota).HasColumnName("quota");
            entity.Property(e => e.TicketName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ticketName");

            entity.HasOne(d => d.Category).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Tickets__categor__3A81B327");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
