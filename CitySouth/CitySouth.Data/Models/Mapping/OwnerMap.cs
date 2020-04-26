using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class OwnerMap : EntityTypeConfiguration<Owner>
    {
        public OwnerMap()
        {
            // Primary Key
            this.HasKey(t => t.OwnerId);

            // Properties
            this.Property(t => t.CheckInType)
                .HasMaxLength(50);

            this.Property(t => t.OwnerName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CheckInName)
                .HasMaxLength(50);

            this.Property(t => t.CardId)
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Occupation)
                .HasMaxLength(50);

            this.Property(t => t.UseInfo)
                .HasMaxLength(250);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Owner");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.HouseId).HasColumnName("HouseId");
            this.Property(t => t.CheckInType).HasColumnName("CheckInType");
            this.Property(t => t.OwnerName).HasColumnName("OwnerName");
            this.Property(t => t.CheckInName).HasColumnName("CheckInName");
            this.Property(t => t.CardId).HasColumnName("CardId");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Occupation).HasColumnName("Occupation");
            this.Property(t => t.PropertyStartDate).HasColumnName("PropertyStartDate");
            this.Property(t => t.PropertyExpireDate).HasColumnName("PropertyExpireDate");
            this.Property(t => t.HandDate).HasColumnName("HandDate");
            this.Property(t => t.RenovationDate).HasColumnName("RenovationDate");
            this.Property(t => t.CheckInDate).HasColumnName("CheckInDate");
            this.Property(t => t.UseInfo).HasColumnName("UseInfo");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
