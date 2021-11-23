﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using WowsKarma.Api.Data.Models;
using WowsKarma.Api.Data.Models.Notifications;
using WowsKarma.Api.Utilities;
using WowsKarma.Common.Models;


namespace WowsKarma.Api.Data;


public class ApiDbContext : DbContext
{
	public DbSet<PlatformBan> PlatformBans { get; set; }
	public DbSet<Player> Players { get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<PostModAction> PostModActions { get; set; }



	#region Notifications
	public DbSet<NotificationBase> Notifications { get; set; }

	public DbSet<PlatformBanNotification> PlatformBanNotifications { get; set; }
	public DbSet<PostAddedNotification> PostAddedNotifications { get; set; }
	public DbSet<PostEditedNotification> PostEditedNotifications { get; set; }
	public DbSet<PostDeletedNotification> PostDeletedNotifications { get; set; }
	public DbSet<PostModDeletedNotification> PostModDeletedNotifications { get; set; }
	#endregion


	static ApiDbContext()
	{
		NpgsqlConnection.GlobalTypeMapper.MapEnum<ModActionType>();
		NpgsqlConnection.GlobalTypeMapper.MapEnum<NotificationType>();
	}

	public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		foreach (Type type in modelBuilder.Model.GetEntityTypes().Where(t => t.ClrType.ImplementsInterface(typeof(ITimestamped))).Select(t => t.ClrType))
		{
			modelBuilder.Entity(type)
				.Property<DateTime>(nameof(ITimestamped.CreatedAt))
					.ValueGeneratedOnAdd()
					.HasDefaultValueSql("NOW()");
		}


		#region Notifications

		modelBuilder.HasPostgresEnum<NotificationType>();

		modelBuilder.Entity<NotificationBase>()
			.HasDiscriminator(n => n.Type)
				.HasValue<NotificationBase>(NotificationType.Unknown)
				.HasValue<PostAddedNotification>(NotificationType.PostAdded)
				.HasValue<PostEditedNotification>(NotificationType.PostEdited)
				.HasValue<PostDeletedNotification>(NotificationType.PostDeleted)
				.HasValue<PostModDeletedNotification>(NotificationType.PostModDeleted)
				.HasValue<PlatformBanNotification>(NotificationType.PlatformBan)
				.IsComplete(false);

		#endregion    // Notifications

		#region Players

		modelBuilder.Entity<Player>()
			.HasMany(p => p.PostsReceived)
			.WithOne(p => p.Player)
			.HasForeignKey(p => p.PlayerId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Player>()
			.HasMany(p => p.PostsSent)
			.WithOne(p => p.Author)
			.HasForeignKey(p => p.AuthorId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Player>()
			.HasMany(p => p.PlatformBans)
			.WithOne(p => p.User)
			.HasForeignKey(p => p.UserId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.Restrict);

		#endregion

		#region	PostModActions

		modelBuilder.HasPostgresEnum<ModActionType>();

		modelBuilder.Entity<PostModAction>()
			.HasOne(pma => pma.Mod)
			.WithMany()
			.HasForeignKey(pma => pma.ModId);

		modelBuilder.Entity<PostModAction>()
			.HasOne(pma => pma.Post)
			.WithMany()
			.HasForeignKey(pma => pma.PostId);

		#endregion
	}
}
