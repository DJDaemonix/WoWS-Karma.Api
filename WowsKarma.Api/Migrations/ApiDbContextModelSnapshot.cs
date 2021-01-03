﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WowsKarma.Api.Data;

namespace WowsKarma.Api.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Player", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("GameKarma")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastBattleTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SiteKarma")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("WgAccountCreatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<bool>("NegativeKarmaAble")
                        .HasColumnType("bit");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<int>("PostFlairs")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Post", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Player", "Author")
                        .WithMany("PostsSent")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("WowsKarma.Api.Data.Models.Player", "Player")
                        .WithMany("PostsReceived")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Author");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Player", b =>
                {
                    b.Navigation("PostsReceived");

                    b.Navigation("PostsSent");
                });
#pragma warning restore 612, 618
        }
    }
}
