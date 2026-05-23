using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Net.Kidd.Habitizer.Persistence.Account;

public class Configuration : IEntityTypeConfiguration<AccountDbo>
{
    public void Configure(EntityTypeBuilder<AccountDbo> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(i => i.Id);
        builder.HasOne(i => i.Source)
            .WithMany(i => i.Accounts)
            .HasForeignKey(i => i.SourceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(i => i.Name).IsRequired();
        builder.Property(i => i.LastUpdatedAt).IsRequired(false);

        builder.HasIndex(i => new { i.SourceId, i.Name }).IsUnique();
    }
}
