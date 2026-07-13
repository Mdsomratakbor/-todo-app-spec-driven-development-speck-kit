using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Data.EntityConfigurations;

public class LunchPreferenceConfiguration : IEntityTypeConfiguration<LunchPreference>
{
    public void Configure(EntityTypeBuilder<LunchPreference> builder)
    {
        builder.ToTable("LunchPreferences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.DietaryRestrictions)
            .HasColumnType("text[]")
            .IsRequired(false);

        builder.Property(e => e.LunchStartTime)
            .IsRequired(false);

        builder.Property(e => e.LunchEndTime)
            .IsRequired(false);

        builder.Property(e => e.BreakDurationMinutes)
            .IsRequired(false);

        builder.Property(e => e.NotificationsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.FavoriteMeals)
            .HasColumnType("text[]")
            .IsRequired(false);

        builder.Property(e => e.ExcludedItems)
            .HasColumnType("text[]")
            .IsRequired(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
