using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class PostMap : EntityTypeConfiguration<Post>
    {
        public PostMap()
        {
            // Primary Key
            this.HasKey(t => t.PostId);

            // Properties
            this.Property(t => t.PostType)
                .HasMaxLength(50);

            this.Property(t => t.PostName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Post");
            this.Property(t => t.PostId).HasColumnName("PostId");
            this.Property(t => t.ParentPostId).HasColumnName("ParentPostId");
            this.Property(t => t.PostType).HasColumnName("PostType");
            this.Property(t => t.PostName).HasColumnName("PostName");
        }
    }
}
