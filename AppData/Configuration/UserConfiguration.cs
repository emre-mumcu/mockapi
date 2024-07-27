using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockAPI.AppData.Entities;

namespace MockAPI.AppData.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.Email).IsRequired();
            builder.HasIndex(e => e.Email).IsUnique(true);
            builder.Property(i => i.Email).HasColumnType("text").HasMaxLength(200);

            builder.Property(p => p.Username).IsRequired();
            builder.HasIndex(e => e.Username).IsUnique(true);
            builder.Property(i => i.Username).HasColumnType("text").HasMaxLength(200);

            builder.HasMany(u => u.Roles).WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRole"))// Optional: specify the join table name
            ;            
        }
    }
}