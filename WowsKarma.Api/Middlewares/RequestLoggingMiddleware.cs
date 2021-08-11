﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WowsKarma.Api.Middlewares
{
	public class RequestLoggingMiddleware
	{
		private const string MessageTemplate = "{Protocol} {RequestMethod} {RequestPath} by {RemoteUser}, responded {StatusCode} in {Elapsed:0.00} ms";

		private static readonly ILogger logger = Log.ForContext<RequestLoggingMiddleware>();
		private static readonly HashSet<string> HeaderWhitelist = new() { "Content-Type", "Content-Length", "User-Agent" };

		private readonly RequestDelegate next;

		public RequestLoggingMiddleware(RequestDelegate next)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));

		}

		public async Task Invoke(HttpContext context)
		{
			_ = context ?? throw new ArgumentNullException(nameof(context));

			long start = Stopwatch.GetTimestamp();

			try
			{
				await next(context);
				double elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

				int? statusCode = context.Response?.StatusCode;
				LogEventLevel level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

				ILogger log = level is LogEventLevel.Error 
					? LogForErrorContext(context) 
					: logger.ForContext("RequestUser", GetRemoteUser(context));

				log.Write(level, MessageTemplate, context.Request.Protocol, context.Request.Method, GetPath(context), GetRemoteUser(context), statusCode, elapsedMs);
			}

			// Never caught, because `LogException()` returns false.
			catch (Exception e) when (LogException(context, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), e)) { }
		}

		private static bool LogException(HttpContext httpContext, double elapsedMs, Exception ex)
		{
			LogForErrorContext(httpContext).Error(ex, MessageTemplate, httpContext.Request.Protocol, httpContext.Request.Method, GetPath(httpContext), GetRemoteUser(httpContext), 500, elapsedMs);

			return false;
		}

		private static ILogger LogForErrorContext(HttpContext context)
		{
			HttpRequest request = context.Request;

			Dictionary<string, string> loggedHeaders = request.Headers
				.Where(h => HeaderWhitelist.Contains(h.Key))
				.ToDictionary(h => h.Key, h => h.Value.ToString());

			return logger
				.ForContext("RequestHeaders", loggedHeaders, destructureObjects: true)
				.ForContext("RequestHost", request.Host)
				.ForContext("RequestProtocol", request.Protocol);
		}

		private static double GetElapsedMilliseconds(long start, long stop) => (stop - start) * 1000 / (double)Stopwatch.Frequency;

		private static string GetPath(HttpContext context) => context.Features.Get<IHttpRequestFeature>()?.RawTarget ?? context.Request.Path.ToString();

		private static string GetRemoteUser(HttpContext context) => context.User?.FindFirstValue(ClaimTypes.Name) ?? context.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
	}
}
