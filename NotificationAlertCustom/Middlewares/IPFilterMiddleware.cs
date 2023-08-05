using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NotificationAlertCustom.Middlewares
{
    public class IPFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationOptions _applicationOptions;

        public IPFilterMiddleware(RequestDelegate next, IOptions<ApplicationOptions> applicationOptionsAccessor, IHttpContextAccessor accessor)
        {
            this._next = next;
            this._accessor = accessor;
            this._applicationOptions = applicationOptionsAccessor.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var ipAddress = GetClientIp();
            List<string> whiteListIPList = _applicationOptions.Whitelist;
            var isInwhiteListIPList = whiteListIPList
                .Where(a => IPAddress.Parse(a)
                .Equals(ipAddress))
                .Any();

            if (!isInwhiteListIPList)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next.Invoke(httpContext);
        }

        public IPAddress GetClientIp()
        {
            IPAddress remoteIpAddress = _accessor.HttpContext.Connection.RemoteIpAddress;

            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
            }

            return remoteIpAddress;
        }
    }

    public static class IPFilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseIPFilterMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPFilterMiddleware>();
        }
    }

    public class ApplicationOptions
    {
        public List<string> Whitelist { get; set; }
    }
}
