using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class WorkPlanTimeMap : EntityTypeConfiguration<WorkPlanTime>
    {
        public WorkPlanTimeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PlanId, t.ConfigId });

            // Properties
            this.Property(t => t.PlanId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ConfigId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("WorkPlanTime");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.ConfigId).HasColumnName("ConfigId");
            this.Property(t => t.IsWork).HasColumnName("IsWork");
        }
    }
}
