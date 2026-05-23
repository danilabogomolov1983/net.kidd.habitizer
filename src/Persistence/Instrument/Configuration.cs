using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wst.Tools.PosiBridge.Persistence.Instrument;

public class Configuration : IEntityTypeConfiguration<InstrumentDbo>
{
    public void Configure(EntityTypeBuilder<InstrumentDbo> builder)
    {
        builder.ToTable("Instruments");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Isin).IsRequired();
        
        builder.HasIndex(i => i.Isin).IsUnique();
    }
}
