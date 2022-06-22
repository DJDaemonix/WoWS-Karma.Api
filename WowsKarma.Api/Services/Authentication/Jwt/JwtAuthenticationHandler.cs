﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WowsKarma.Api.Services.Authentication.Jwt
{
	public class JwtAuthenticationHandler : JwtBearerHandler
	{
		private readonly UserService userService;

		public JwtAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, UserService userService)
			: base(options, logger, encoder, clock)
		{
			this.userService = userService;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			AuthenticateResult baseResult = await base.HandleAuthenticateAsync();

			if (!baseResult.Succeeded)
			{
				return baseResult;
			}

			bool isValid = false;
			Exception failure = default;

			try
			{
				if (new Guid(baseResult.Principal.FindFirstValue("seed")) is var seed
					&& await userService.ValidateUserSeedTokenAsync(uint.Parse(baseResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)), seed))
				{
					isValid = true;
				}
			}
			catch (Exception e)
			{
				isValid = false;
				failure = e;
			}

			return isValid
				? baseResult
				: failure == default
					? AuthenticateResult.NoResult()
					: AuthenticateResult.Fail(failure.Message);
		}
	}
}
