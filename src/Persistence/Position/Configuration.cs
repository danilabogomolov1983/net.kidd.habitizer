using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Net.Kidd.Habitizer.Persistence.Position;

public class Configuration : IEntityTypeConfiguration<PositionDbo>
{
    public void Configure(EntityTypeBuilder<PositionDbo> builder)
    {
        builder.ToTable("Positions");

        builder.HasKey(i => new { i.AccountId, i.InstrumentId });
        builder.HasOne(i => i.Account)
            .WithMany(i => i.Positions)
            .HasForeignKey(i => i.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(i => i.Instrument)
            .WithMany()
            .HasForeignKey(i => i.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(i => i.ReferencePrice_Currency)
            .HasColumnName("ReferencePrice_Currency");
        builder.Property(i => i.ReferencePrice_Exchange)
            .HasColumnName("ReferencePrice_Exchange");

        builder.Property(i => i.NetSize)
            .HasPrecision(18, 2);

        builder.Property(i => i.NetValue)
            .HasPrecision(18, 2);

        builder.Property(i => i.ReferencePrice_CurrencySpot)
            .HasPrecision(18, 2);

        builder.Property(i => i.ReferencePrice_Price)
            .HasPrecision(18, 2);

        builder.Property(i => i.UnrealisedAverageCost)
            .HasPrecision(18, 2);

        builder.Property(i => i.UnrealisedProfit)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.UnrealisedProfitPercent)
            .HasPrecision(18, 2);
    }
}
