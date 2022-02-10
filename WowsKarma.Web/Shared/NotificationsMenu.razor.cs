﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using WowsKarma.Common.Hubs;
using WowsKarma.Common.Models;
using WowsKarma.Common.Models.DTOs.Notifications;
using WowsKarma.Web.Shared.Components;
using WowsKarma.Web.Shared.Components.CircuitBreaker;


namespace WowsKarma.Web.Shared;



public partial class NotificationsMenu : ComponentBaseAuth, IAsyncDisposable
{
	public SortedSet<INotification> Notifications { get; set; } = new(new ByMostRecentNotifications());

	protected static ConcurrentDictionary<string, Type> ResolvedTypes { get; } = new();
	[Inject] protected IConfiguration Configuration { get; set; }

	protected CircuitBreakerSection _breakerSection { get; set; } = new();	
	private readonly CancellationTokenSource _cts = new();
	private HubConnection _hub;
	private bool _disposedValue;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		try
		{
			_hub = new HubConnectionBuilder()
				.WithAutomaticReconnect()
				.WithUrl(Configuration[$"Api:{Utilities.CurrentRegion}:NotificationsHub"], options =>
				{
					options.AccessTokenProvider = () => Task.FromResult(CurrentToken);
				})
				.AddNewtonsoftJsonProtocol(config =>
				{
					config.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
				})
				.Build();

			HookHandlers();

			await _hub.StartAsync(_cts.Token);
		}
		catch (Exception e)
		{
			await _breakerSection.TripExceptionAsync(e);
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		_breakerSection.Recover();

		try
		{
			await foreach ((string dtoType, object notification) in _hub.StreamAsync<(string, object)>(nameof(INotificationsHubInvoke.GetPendingNotifications), _cts.Token))
			{
				Type type = GetType(dtoType);

				if (type.IsAssignableTo(typeof(NotificationBaseDTO)))
				{
					Notifications.Add(notification as NotificationBaseDTO);
				}
			}
		}
		catch (Exception e)
		{
			await _breakerSection.TripExceptionAsync(e);
		}

		await base.OnParametersSetAsync();
	}

	protected void HookHandlers()
	{
		_hub.On<INotification>(nameof(INotificationsHubPush.NewNotification), (notification) => Notifications.Add(notification));
		_hub.On<Guid>(nameof(INotificationsHubPush.DeletedNotification), (id) => Notifications.RemoveWhere(x => x.Id == id));
	}

	protected Task AcknowledgeNotificationAsync(INotification notification) => AcknowledgeNotificationsAsync(new[] { notification }, _cts.Token);
	protected Task AcknowledgeNotificationAsync(INotification notification, CancellationToken ct) => AcknowledgeNotificationsAsync(new[] { notification }, ct);

	protected Task ClearNotificationsAsync() => AcknowledgeNotificationsAsync(Notifications, _cts.Token);
	protected async Task AcknowledgeNotificationsAsync(IEnumerable<INotification> notifications, CancellationToken ct)
	{
		await _hub.SendAsync(nameof(INotificationsHubInvoke.AcknowledgeNotifications), notifications.Select(n => n.Id), ct);
		Notifications.ExceptWith(notifications);
	}

	private static Type GetType(string typeName)
	{
		if (ResolvedTypes.TryGetValue(typeName, out Type type))
		{
			return type;
		}

		type = Common.Utilities.GetType(typeName);
		ResolvedTypes.TryAdd(typeName, type);
		return type;
	}

	#region DisposeAsync
	public async ValueTask DisposeAsync()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		await DisposeAsync(true);
		GC.SuppressFinalize(this);
	}

	protected async virtual Task DisposeAsync(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_cts.Cancel();
				// TODO: dispose managed state (managed objects)
				await _hub.DisposeAsync();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposedValue = true;
		}
	}
	#endregion	// DisposeAsync

	private class ByMostRecentNotifications : IComparer<INotification>
	{
		public int Compare(INotification x, INotification y) => -x.EmittedAt.CompareTo(y.EmittedAt);
	}
}
