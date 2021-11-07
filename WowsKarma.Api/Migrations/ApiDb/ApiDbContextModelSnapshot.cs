﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WowsKarma.Api.Data;
using WowsKarma.Common.Models;

#nullable disable

namespace WowsKarma.Api.Migrations.ApiDb
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-rc.2.21480.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "mod_action_type", new[] { "deletion", "update" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "notification_type", new[] { "unknown", "other", "post_added", "post_edited", "post_deleted", "post_mod_edited", "post_mod_deleted" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.NotificationBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("AcknowledgedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EmittedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<NotificationType>("Type")
                        .HasColumnType("notification_type");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Notifications");

                    b.HasDiscriminator<NotificationType>("Type").IsComplete(false).HasValue(NotificationType.Unknown);
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.PlatformBan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BannedUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<long>("ModId")
                        .HasColumnType("bigint");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Reverted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ModId");

                    b.HasIndex("UserId");

                    b.ToTable("PlatformBans");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Player", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("CourtesyRating")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int>("GameKarma")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastBattleTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("OptOutChanged")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("OptedOut")
                        .HasColumnType("boolean");

                    b.Property<int>("PerformanceRating")
                        .HasColumnType("integer");

                    b.Property<bool>("PostsBanned")
                        .HasColumnType("boolean");

                    b.Property<int>("SiteKarma")
                        .HasColumnType("integer");

                    b.Property<int>("TeamplayRating")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<DateTime>("WgAccountCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("WgHidden")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int>("Flairs")
                        .HasColumnType("integer");

                    b.Property<bool>("ModLocked")
                        .HasColumnType("boolean");

                    b.Property<bool>("NegativeKarmaAble")
                        .HasColumnType("boolean");

                    b.Property<long>("PlayerId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.PostModAction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<ModActionType>("ActionType")
                        .HasColumnType("mod_action_type");

                    b.Property<long>("ModId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModId");

                    b.HasIndex("PostId");

                    b.ToTable("PostModActions");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostAddedNotification", b =>
                {
                    b.HasBaseType("WowsKarma.Api.Data.Models.Notifications.NotificationBase");

                    b.Property<Guid>("PostId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.HasDiscriminator().HasValue(NotificationType.PostAdded);
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostDeletedNotification", b =>
                {
                    b.HasBaseType("WowsKarma.Api.Data.Models.Notifications.NotificationBase");

                    b.Property<Guid>("PostId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.HasDiscriminator().HasValue(NotificationType.PostDeleted);
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostEditedNotification", b =>
                {
                    b.HasBaseType("WowsKarma.Api.Data.Models.Notifications.NotificationBase");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.HasDiscriminator().HasValue(NotificationType.PostEdited);
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostModDeletedNotification", b =>
                {
                    b.HasBaseType("WowsKarma.Api.Data.Models.Notifications.NotificationBase");

                    b.Property<Guid>("ModActionId")
                        .HasColumnType("uuid");

                    b.HasIndex("ModActionId");

                    b.HasDiscriminator().HasValue(NotificationType.PostModDeleted);
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.NotificationBase", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Player", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.PlatformBan", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Player", "Mod")
                        .WithMany()
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WowsKarma.Api.Data.Models.Player", "User")
                        .WithMany("PlatformBans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Mod");

                    b.Navigation("User");
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

            modelBuilder.Entity("WowsKarma.Api.Data.Models.PostModAction", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Player", "Mod")
                        .WithMany()
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WowsKarma.Api.Data.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mod");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostAddedNotification", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostDeletedNotification", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostEditedNotification", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Notifications.PostModDeletedNotification", b =>
                {
                    b.HasOne("WowsKarma.Api.Data.Models.PostModAction", "ModAction")
                        .WithMany()
                        .HasForeignKey("ModActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ModAction");
                });

            modelBuilder.Entity("WowsKarma.Api.Data.Models.Player", b =>
                {
                    b.Navigation("PlatformBans");

                    b.Navigation("PostsReceived");

                    b.Navigation("PostsSent");
                });
#pragma warning restore 612, 618
        }
    }
}
