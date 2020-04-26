using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class GoodsReceiptMap : EntityTypeConfiguration<GoodsReceipt>
    {
        public GoodsReceiptMap()
        {
            // Primary Key
            this.HasKey(t => t.ReceiptId);

            // Properties
            this.Property(t => t.ReceiptId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ReceiptNo)
                .HasMaxLength(50);

            this.Property(t => t.Reason)
                .HasMaxLength(4000);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.Purchaser)
                .HasMaxLength(50);

            this.Property(t => t.Submitter)
                .HasMaxLength(50);

            this.Property(t => t.StorageChecker)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GoodsReceipt");
            this.Property(t => t.ReceiptId).HasColumnName("ReceiptId");
            this.Property(t => t.ReceiptNo).HasColumnName("ReceiptNo");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Purchaser).HasColumnName("Purchaser");
            this.Property(t => t.Submitter).HasColumnName("Submitter");
            this.Property(t => t.SubmitDate).HasColumnName("SubmitDate");
            this.Property(t => t.StorageChecker).HasColumnName("StorageChecker");
            this.Property(t => t.InStorageDate).HasColumnName("InStorageDate");
        }
    }
}
