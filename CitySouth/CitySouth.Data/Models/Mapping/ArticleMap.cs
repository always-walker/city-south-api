using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CitySouth.Data.Models.Mapping
{
    public class ArticleMap : EntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            // Primary Key
            this.HasKey(t => t.ArticleId);

            // Properties
            this.Property(t => t.Title)
                .HasMaxLength(250);

            this.Property(t => t.GoUrl)
                .HasMaxLength(250);

            this.Property(t => t.Image)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Article");
            this.Property(t => t.ArticleId).HasColumnName("ArticleId");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.GoUrl).HasColumnName("GoUrl");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.Click).HasColumnName("Click");
            this.Property(t => t.IsShow).HasColumnName("IsShow");
            this.Property(t => t.AddTime).HasColumnName("AddTime");
            this.Property(t => t.Sort).HasColumnName("Sort");
        }
    }
}
