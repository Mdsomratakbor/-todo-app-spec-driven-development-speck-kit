using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(e => e.HashedPassword)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Role)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("User");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(e => e.Email)
            .IsUnique();
    }
}
