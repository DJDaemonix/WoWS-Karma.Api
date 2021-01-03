﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wargaming.WebAPI.Models.WorldOfWarships.Responses;
using Wargaming.WebAPI.Requests;
using WowsKarma.Api.Data;
using WowsKarma.Api.Data.Models;
using WowsKarma.Api.Utilities;
using WowsKarma.Common.Models.DTOs;

namespace WowsKarma.Api.Services
{
	public class PlayerService
	{
		public static TimeSpan DataUpdateSpan => new(1, 0, 0);	// 1 hour

		private readonly ApiDbContext context;
		private readonly WorldOfWarshipsHandler wgApi;
		private readonly VortexApiHandler vortex;


		public PlayerService(IDbContextFactory<ApiDbContext> contextFactory, WorldOfWarshipsHandler wgApi, VortexApiHandler vortex)
		{
			context = contextFactory.CreateDbContext() ?? throw new ArgumentNullException(nameof(contextFactory));
			this.wgApi = wgApi ?? throw new ArgumentNullException(nameof(wgApi));
			this.vortex = vortex ?? throw new ArgumentNullException(nameof(vortex));
		}

		public async Task<Player> GetPlayerAsync(uint accountId)
		{
			if (accountId is 0)
			{
				return null;
			}
			
			Player player = await context.Players.FindAsync(accountId);

			try
			{
				if (player is null)
				{
					return await UpdatePlayerRecordAsync(accountId, true);
				}
				else if (UpdateNeeded(player))
				{
					return await UpdatePlayerRecordAsync(accountId, false);
				}
				else
				{
					return player;
				}
			}
			catch (ApplicationException)
			{
				return null;
			}
		}

		public async Task<IEnumerable<AccountListingDTO>> ListPlayersAsync(string search)
		{
			IEnumerable<AccountListing> result = await wgApi.ListPlayersAsync(search);

			return result.Count() is not 0 
				? result.Select(listing => listing.ToDTO()) 
				: null;
		}

		internal async Task<Player> UpdatePlayerRecordAsync(uint accountId, bool firstEntry)
		{
			Player player = (await vortex.FetchAccountAsync(accountId)).ToDbModel() ?? throw new ApplicationException("Account returned null.");
			player.UpdatedAt = DateTime.UtcNow; // Forcing UpdatedAt refresh

			if (firstEntry)
			{
				context.Players.Add(player);
			}
			else
			{
				Player.Map(await context.Players.FindAsync(accountId), player);
			}

			await context.SaveChangesAsync();
			return player;
		}

		internal static bool UpdateNeeded(Player player) => player.UpdatedAt.Add(DataUpdateSpan) < DateTime.Now;


	}
}
