using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Net.Kidd.Habitizer.Persistence.Source;

public class Configuration : IEntityTypeConfiguration<SourceDbo>
{
    public void Configure(EntityTypeBuilder<SourceDbo> builder)
    {
        builder.ToTable("Sources");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired();

        builder.HasIndex(i => i.Name).IsUnique();
    }
}
