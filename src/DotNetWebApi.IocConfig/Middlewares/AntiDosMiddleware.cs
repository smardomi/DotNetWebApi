using System;
using System.Net;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Security.Http;
using DotNetWebApi.IocConfig.Api;
using DotNetWebApi.IocConfig.Firewall;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotNetWebApi.IocConfig.Middlewares
{
    /// <summary>
    /// AntiDos Middleware Extensions
    /// </summary>
    public static class AntiDosMiddlewareExtensions
    {
        /// <summary>
        /// Adds AntiDosMiddleware to the pipeline.
        /// </summary>
        public static IApplicationBuilder UseAntiDos(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiDosMiddleware>();
        }
    }
    /// <summary>
    /// AntiDos Middleware
    /// </summary>
    public class AntiDosMiddleware
    {
        private readonly RequestDelegate _next;
        private IOptionsSnapshot<AntiDosConfig> _antiDosConfig;

        /// <summary>
        /// AntiDos Middleware
        /// </summary>
        public AntiDosMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// AntiDos Middleware Pipeline
        /// </summary>
        public async Task Invoke(
             HttpContext context,
             IOptionsSnapshot<AntiDosConfig> antiDosConfig,
             IAntiDosFirewall antiDosFirewall)
        {
            _antiDosConfig = antiDosConfig ?? throw new ArgumentNullException(nameof(antiDosConfig));
            if (_antiDosConfig.Value == null)
            {
                throw new ArgumentNullException(nameof(antiDosConfig), "Please add AntiDosConfig to your appsettings.json file.");
            }

            var requestInfo = GetHeadersInfo(context);

            var validationResult = antiDosFirewall.ShouldBlockClient(requestInfo);
            if (validationResult.ShouldBlockClient)
            {
                antiDosFirewall.LogIt(validationResult.ThrottleInfo, requestInfo);
                AddResetHeaders(context, validationResult.ThrottleInfo);
                await BlockClient(context);
                return;
            }
            await _next(context);
        }

        private AntiDosFirewallRequestInfo GetHeadersInfo(HttpContext context)
        {
            return new AntiDosFirewallRequestInfo
            {
                Ip = context.GetIp(),
                UserAgent = context.GetUserAgent(),
                UrlReferrer = context.GetReferrerUri(),
                RawUrl = context.GetRawUrl(),
                IsLocal = context.IsLocal(),
                RequestHeaders = context.Request.Headers
            };
        }

        private Task BlockClient(HttpContext context)
        {
            // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html

            var result = new ApiResult(false, ApiResultStatusCode.Conflict, _antiDosConfig.Value.ErrorMessage);
            var json = JsonConvert.SerializeObject(result);

            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(json);
        }

        private void AddResetHeaders(HttpContext context, ThrottleInfo throttleInfo)
        {
            if (throttleInfo == null)
            {
                return;
            }
            context.Response.Headers["X-RateLimit-Limit"] = _antiDosConfig.Value.AllowedRequests.ToString();
            var requestsRemaining = Math.Max(_antiDosConfig.Value.AllowedRequests - throttleInfo.RequestsCount, 0);
            context.Response.Headers["X-RateLimit-Remaining"] = requestsRemaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = throttleInfo.ExpiresAt.ToUnixTimeSeconds().ToString();
            context.Response.Headers["Retry-After"] = context.Response.Headers["X-RateLimit-Reset"];
        }
    }
}