﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CmsCoreV3.Data;

namespace CmsCoreV3.Migrations.HostDb
{
    [DbContext(typeof(HostDbContext))]
    [Migration("20170526081220_updateTheme")]
    partial class updateTheme
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CmsCoreV3.Models.AppTenant", b =>
                {
                    b.Property<string>("AppTenantId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConnectionString");

                    b.Property<string>("Hostname");

                    b.Property<string>("Name");

                    b.Property<long>("ThemeId");

                    b.Property<string>("ThemeName");

                    b.Property<string>("Title");

                    b.HasKey("AppTenantId");

                    b.HasIndex("ThemeId");

                    b.ToTable("AppTenants");
                });

            modelBuilder.Entity("CmsCoreV3.Models.Theme", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ComponentTemplates");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("CustomCSS");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Logo");

                    b.Property<string>("ManyLocation");

                    b.Property<string>("MetaDescription");

                    b.Property<string>("MetaKeywords");

                    b.Property<string>("MetaTitle")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("PageTemplates");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("CmsCoreV3.Models.AppTenant", b =>
                {
                    b.HasOne("CmsCoreV3.Models.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}