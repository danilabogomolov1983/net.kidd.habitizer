using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wst.Tools.PosiBridge.Tradix.Snapshot.Position;

public class Configuration : IEntityTypeConfiguration<PositionDbo>
{
    public void Configure(EntityTypeBuilder<PositionDbo> builder)
    {
        builder.ToTable("WELCOME_TRX_ShortPositionsOverview", "dbo");
        builder.HasNoKey();

        builder.Property(i => i.Isin)
            .HasColumnName("ISIN")
            .HasMaxLength(12)
            .IsUnicode(false);
        
        builder.Property(i => i.NetValue)
            .HasColumnName("Betrag")
            .HasPrecision(15, 2);
        
        builder.Property(i => i.NetSize)
            .HasColumnName("Nominale")
            .HasPrecision(15, 0);
        
        builder.Property(i => i.Exchange)
            .HasColumnName("Boerse")
            .HasMaxLength(50)
            .IsUnicode(false);
        
        builder.Property(i => i.UnrealisedProfit)
            .HasColumnName("UnrealPL")
            .HasPrecision(15, 2);
        
        builder.Property(i => i.Account)
            .HasColumnName("Depot")
            .HasMaxLength(50)
            .IsUnicode(false);
        
        builder.Property(i => i.Price)
            .HasColumnName("SchnittKurs")
            .HasPrecision(15, 7);
    }
}
