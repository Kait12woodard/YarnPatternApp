using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YarnPatternApp.Models;

public partial class YarnPatternContext : DbContext
{
    public YarnPatternContext()
    {
    }

    public YarnPatternContext(DbContextOptions<YarnPatternContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CraftType> CraftTypes { get; set; }

    public virtual DbSet<Designer> Designers { get; set; }

    public virtual DbSet<Difficulty> Difficulties { get; set; }

    public virtual DbSet<Pattern> Patterns { get; set; }

    public virtual DbSet<PatternTag> PatternTags { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<ToolSize> ToolSizes { get; set; }

    public virtual DbSet<YarnBrand> YarnBrands { get; set; }

    public virtual DbSet<YarnWeight> YarnWeights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__CraftTyp__3214EC27760DC751");

            entity.ToTable("CraftType");

            entity.HasIndex(e => e.Craft, "UQ__CraftTyp__C45F81C4773FEC54").IsUnique();

            entity.Property(e => e.Craft).HasMaxLength(150);
        });

        modelBuilder.Entity<Designer>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Designer__3214EC27EE722765");

            entity.HasIndex(e => e.Name, "UQ__Designer__737584F65A7D47A7").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Difficul__3214EC27CF1052C0");

            entity.ToTable("Difficulty");

            entity.Property(e => e.Ranking).HasMaxLength(150);
        });

        modelBuilder.Entity<Pattern>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Patterns__3214EC277E960596");

            entity.HasIndex(e => e.FilePath, "UQ__Patterns__48D910BD65A0E90C").IsUnique();

            entity.Property(e => e.DateAdded)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FilePath).HasMaxLength(250);
            entity.Property(e => e.LastViewed).HasPrecision(0);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PatSource).HasMaxLength(250);

            entity.HasOne(d => d.CraftType).WithMany(p => p.Patterns)
                .HasForeignKey(d => d.CraftTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Pattern_CraftType");

            entity.HasOne(d => d.Designer).WithMany(p => p.Patterns)
                .HasForeignKey(d => d.DesignerID)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Fk_Pattern_Designer");

            entity.HasOne(d => d.Difficulty).WithMany(p => p.Patterns)
                .HasForeignKey(d => d.DifficultyID)
                .HasConstraintName("Fk_Pattern_Difficulty");
        });

        modelBuilder.Entity<PatternTag>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__PatternT__3214EC27185DC175");

            entity.ToTable("PatternTag");

            entity.HasIndex(e => e.Tag, "UQ__PatternT__C4516413DF88A9BA").IsUnique();

            entity.Property(e => e.Tag).HasMaxLength(150);

            entity.HasMany(d => d.Patterns).WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "TagLookup",
                    r => r.HasOne<Pattern>().WithMany()
                        .HasForeignKey("PatternID")
                        .HasConstraintName("FK_TagLookup_Pattern"),
                    l => l.HasOne<PatternTag>().WithMany()
                        .HasForeignKey("TagID")
                        .HasConstraintName("FK_TagLookup_Tag"),
                    j =>
                    {
                        j.HasKey("TagID", "PatternID");
                        j.ToTable("TagLookup");
                    });
        });

        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ProjectT__3214EC27E95B3277");

            entity.ToTable("ProjectType");

            entity.HasIndex(e => e.Name, "UQ__ProjectT__737584F6206E26EB").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasMany(d => d.Patterns).WithMany(p => p.ProjectTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectTypeLookup",
                    r => r.HasOne<Pattern>().WithMany()
                        .HasForeignKey("PatternID")
                        .HasConstraintName("FK_ProjectTypeLookup_Pattern"),
                    l => l.HasOne<ProjectType>().WithMany()
                        .HasForeignKey("ProjectTypeID")
                        .HasConstraintName("FK_ProjectTypeLookup_ProjectType"),
                    j =>
                    {
                        j.HasKey("ProjectTypeID", "PatternID");
                        j.ToTable("ProjectTypeLookup");
                    });
        });

        modelBuilder.Entity<ToolSize>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ToolSize__3214EC27945446C4");

            entity.ToTable("ToolSize");

            entity.HasIndex(e => e.Size, "UQ__ToolSize__A3250D068B37CB67").IsUnique();

            entity.Property(e => e.Size).HasColumnType("decimal(4, 2)");

            entity.HasMany(d => d.Patterns).WithMany(p => p.ToolSizes)
                .UsingEntity<Dictionary<string, object>>(
                    "ToolSizeLookup",
                    r => r.HasOne<Pattern>().WithMany()
                        .HasForeignKey("PatternID")
                        .HasConstraintName("Fk_ToolSizeLookup_Pattern"),
                    l => l.HasOne<ToolSize>().WithMany()
                        .HasForeignKey("ToolSizeID")
                        .HasConstraintName("Fk_ToolSizeLookup_ToolSize"),
                    j =>
                    {
                        j.HasKey("ToolSizeID", "PatternID").HasName("Pk_ToolSizeLookup");
                        j.ToTable("ToolSizeLookup");
                    });
        });

        modelBuilder.Entity<YarnBrand>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__YarnBran__3214EC272146D2EF");

            entity.ToTable("YarnBrand");

            entity.HasIndex(e => e.Name, "UQ__YarnBran__737584F6CC135C2F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasMany(d => d.Patterns).WithMany(p => p.YarnBrands)
                .UsingEntity<Dictionary<string, object>>(
                    "YarnBrandLookup",
                    r => r.HasOne<Pattern>().WithMany()
                        .HasForeignKey("PatternID")
                        .HasConstraintName("FK_YarnBrandLookup_Pattern"),
                    l => l.HasOne<YarnBrand>().WithMany()
                        .HasForeignKey("YarnBrandID")
                        .HasConstraintName("FK_YarnBrandLookup_YarnBrand"),
                    j =>
                    {
                        j.HasKey("YarnBrandID", "PatternID");
                        j.ToTable("YarnBrandLookup");
                    });
        });

        modelBuilder.Entity<YarnWeight>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__YarnWeig__3214EC27290CCE97");

            entity.ToTable("YarnWeight");

            entity.HasIndex(e => e.Weight, "UQ__YarnWeig__CAD8CB4EAD2481BF").IsUnique();

            entity.HasMany(d => d.Patterns).WithMany(p => p.YarnWeights)
                .UsingEntity<Dictionary<string, object>>(
                    "YarnWeightLookup",
                    r => r.HasOne<Pattern>().WithMany()
                        .HasForeignKey("PatternID")
                        .HasConstraintName("FK_YarnWeightLookup_Pattern"),
                    l => l.HasOne<YarnWeight>().WithMany()
                        .HasForeignKey("YarnWeightID")
                        .HasConstraintName("FK_YarnWeightLookup_YarnWeight"),
                    j =>
                    {
                        j.HasKey("YarnWeightID", "PatternID");
                        j.ToTable("YarnWeightLookup");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
