using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            this.HasKey(t => t.EmployeeId);

            // Properties
            this.Property(t => t.EmployeeNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EmployeeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Sex)
                .HasMaxLength(10);

            this.Property(t => t.CardId)
                .HasMaxLength(50);

            this.Property(t => t.Education)
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .HasMaxLength(250);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.UrgentContactName)
                .HasMaxLength(50);

            this.Property(t => t.UrgentContactPhone)
                .HasMaxLength(50);

            this.Property(t => t.QuitRemark)
                .HasMaxLength(1000);

            this.Property(t => t.Remark)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("Employee");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.EmployeeNo).HasColumnName("EmployeeNo");
            this.Property(t => t.EmployeeName).HasColumnName("EmployeeName");
            this.Property(t => t.EstateId).HasColumnName("EstateId");
            this.Property(t => t.Sex).HasColumnName("Sex");
            this.Property(t => t.CardId).HasColumnName("CardId");
            this.Property(t => t.PostId).HasColumnName("PostId");
            this.Property(t => t.Education).HasColumnName("Education");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.UrgentContactName).HasColumnName("UrgentContactName");
            this.Property(t => t.UrgentContactPhone).HasColumnName("UrgentContactPhone");
            this.Property(t => t.QuitDate).HasColumnName("QuitDate");
            this.Property(t => t.QuitRemark).HasColumnName("QuitRemark");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
