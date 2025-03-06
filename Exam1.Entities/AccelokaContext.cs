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

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

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

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__415B03B87AB9F965");

            entity.HasIndex(e => e.BookedTicketId, "Unique_BookedTicketId").IsUnique();

            entity.Property(e => e.CartId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("cartId");
            entity.Property(e => e.BookedTicketId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookedTicketId");
            entity.Property(e => e.IsCompleted)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("false")
                .HasColumnName("isCompleted");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_userId");
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CFF4CCB9376");

            entity.HasIndex(e => e.UserName, "UQ__Users__66DCF95C8D0A1355").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E61644FEFB766").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("userId");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("user")
                .HasColumnName("role");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("userName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
