﻿using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WowsKarma.Api.Data;
using WowsKarma.Api.Data.Models.Notifications;
using WowsKarma.Api.Services.Discord;



namespace WowsKarma.Api.Services;

public class ModService
{
	private readonly ILogger<ModService> _logger;
	private readonly ModActionWebhookService _webhookService;
	private readonly PostService _postService;
	private readonly NotificationService _notifications;
	private readonly ApiDbContext _context;

	public ModService(ILogger<ModService> logger, ApiDbContext context, ModActionWebhookService webhookService, PostService postService, NotificationService notifications)
	{
		_context = context;
		_logger = logger;
		_webhookService = webhookService;
		_postService = postService;
		_notifications = notifications;
	}

	public Task<PostModAction> GetModActionAsync(Guid id) => _context.PostModActions.AsNoTracking().FirstOrDefaultAsync(ma => ma.Id == id);

	public IQueryable<PostModAction> GetPostModActions(Guid postId) => _context.PostModActions.AsNoTracking()
		.Include(ma => ma.Post)
		.Include(ma => ma.Mod)
		.Where(ma => ma.PostId == postId);

	public IQueryable<PostModAction> GetPostModActions(uint playerId) => _context.PostModActions.AsNoTracking()
		.Include(ma => ma.Post)
		.Include(ma => ma.Mod)
		.Where(ma => ma.Post.AuthorId == playerId);

	public async Task SubmitModActionAsync(PostModActionDTO modAction)
	{
		EntityEntry<PostModAction> entityEntry = await _context.PostModActions.AddAsync(modAction.Adapt<PostModAction>());

		switch (modAction.ActionType)
		{
			case ModActionType.Deletion:
				await _postService.DeletePostAsync(modAction.PostId, true);
				await _notifications.SendNewNotification(PostModDeletedNotification.FromModAction(entityEntry.Entity));
				break;

			case ModActionType.Update:
				PlayerPostDTO current = _postService.GetPost(modAction.PostId).Adapt<PlayerPostDTO>();

				await _postService.EditPostAsync(modAction.PostId, current with
				{
					Content = modAction.UpdatedPost.Content ?? current.Content,
					Flairs = modAction.UpdatedPost.Flairs
				});

				break;
		}

		await _context.SaveChangesAsync();

		await entityEntry.Reference(pma => pma.Mod).LoadAsync();
		await entityEntry.Reference(pma => pma.Post).Query().Include(p => p.Author).LoadAsync();

		_ = _webhookService.SendModActionWebhookAsync(entityEntry.Entity);
	}

	public Task RevertModActionAsync(Guid modActionId)
	{
		PostModAction stub = new() { Id = modActionId };

		_context.PostModActions.Attach(stub);
		_context.PostModActions.Remove(stub);

		return _context.SaveChangesAsync();
	}
}
