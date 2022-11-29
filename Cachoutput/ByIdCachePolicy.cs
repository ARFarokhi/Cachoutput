﻿using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace Cachoutput
{
    /// <summary>
    /// https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/OutputCaching/src/Policies/DefaultPolicy.cs
    /// </summary>
    public sealed class ByIdCachePolicy : IOutputCachePolicy
    {
        //public static readonly ByIdCachePolicy Instance = new();

        public ByIdCachePolicy() { }

        /// <inheritdoc />
        ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            //custome code
            var idRouteVal = context.HttpContext.Request.RouteValues["id"];
            if (idRouteVal is null)
                return ValueTask.CompletedTask;
            context.Tags.Add(idRouteVal.ToString()!);
            
            //default implementation
            var attemptOutputCaching = AttemptOutputCaching(context);
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = attemptOutputCaching;
            context.AllowCacheStorage = attemptOutputCaching;
            context.AllowLocking = true;

            // Vary by any query by default
            context.CacheVaryByRules.QueryKeys = "*";

            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            var response = context.HttpContext.Response;

            // Verify existence of cookie headers
            if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            // Check response code
            if (response.StatusCode != StatusCodes.Status200OK)
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            return ValueTask.CompletedTask;
        }

        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            // Check if the current request fulfills the requirements to be cached

            var request = context.HttpContext.Request;

            // Verify the method
            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
                return false;

            // Verify existence of authorization headers
            if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
                return false;

            return true;
        }
    }
}
