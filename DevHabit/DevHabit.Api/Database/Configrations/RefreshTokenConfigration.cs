using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configrations;

public class RefreshTokenConfigration: IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserId).HasMaxLength(300);
        builder.Property(u => u.Token).HasMaxLength(1000);

        builder.HasIndex(u => u.Token).IsUnique();
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
