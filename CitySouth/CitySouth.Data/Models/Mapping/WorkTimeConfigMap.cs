using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class WorkTimeConfigMap : EntityTypeConfiguration<WorkTimeConfig>
    {
        public WorkTimeConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.ConfigId);

            // Properties
            // Table & Column Mappings
            this.ToTable("WorkTimeConfig");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.TimeStart).HasColumnName("TimeStart");
            this.Property(t => t.TimeEnd).HasColumnName("TimeEnd");
            this.Property(t => t.IsAble).HasColumnName("IsAble");
        }
    }
}
