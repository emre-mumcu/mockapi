using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace MockAPI.AppLib.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClientIP(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext!.GetClientIP();
        }

        public static string GetClientIP(this HttpContext context)
        {
            /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-For
            /// The X-Forwarded-For (XFF) request header is a de-facto standard header for identifying the originating IP address 
            /// of a client connecting to a web server through a proxy server. 
            /// Proxy olduğu durumlarda X-Forwarded-For 1. sırada olsun.

            string clientIP =
                (IPAddress.TryParse(context?.GetHeaderValue("X-Forwarded-For"), out IPAddress? ip2) ? ip2 : null)?.ToString() ??
                (IPAddress.TryParse(context?.GetHeaderValue("X-Original-Forwarded-For"), out IPAddress? ip3) ? ip3 : null)?.ToString() ??
                (IPAddress.TryParse(context?.GetHeaderValue("X-Real-IP"), out IPAddress? ip4) ? ip4 : null)?.ToString() ??
                (IPAddress.TryParse(context?.GetHeaderValue("REMOTE-ADDR"), out IPAddress? ip5) ? ip5 : null)?.ToString() ??
                (IPAddress.TryParse(context?.GetHeaderValue("CF-Connecting-IP"), out IPAddress? ip1) ? ip1 : null)?.ToString() ??
                context?.Features?.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString() ??
                context?.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ??
                "cli.ip.unknown";

            return clientIP;
        }

        public static string? GetServerIP(this HttpContext context)
        {
            return
                // LocalIpAddress belongs the server
                context?.Features?.Get<IHttpConnectionFeature>()?.LocalIpAddress?.ToString() ??
                context?.Request?.HttpContext?.Connection?.LocalIpAddress?.ToString() ??
                "srv.ip.unknown";
        }

        public static (IPAddress? Ip, string IpList) GetServerIP()
        {
            //System.Net.NetworkInformation.NetworkInterface[] nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            //foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics)

            List<IPAddress> list = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList();

            StringBuilder ipAddressList = new StringBuilder();
            ipAddressList.AppendJoin(',', list);


            return (list.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork), ipAddressList.ToString());
        }

        public static string GetMachineName(this IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                string hostName = httpContextAccessor.HttpContext?.Features?.Get<IServerVariablesFeature>()?["REMOTE_HOST"]
                    ?? httpContextAccessor?.HttpContext?.Request?.Headers["REMOTE-ADDR"].FirstOrDefault()
                    ?? string.Empty;

                return hostName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string? GetProtocol(this HttpContext context)
        {
            return context.GetHeaderValue("x-forwarded-proto");
        }

        public static string? GetHost(this HttpContext context)
        {
            return context.GetHeaderValue("x-forwarded-host");
        }

        public static string? GetUserAgent(this HttpContext context)
        {
            return context.GetHeaderValue("user-agent");
        }

        public static string? GetHeaderValue(this HttpContext context, string HeaderKey)
        {
            if (context.Request.Headers.TryGetValue(HeaderKey, out StringValues HeaderValue))
                return HeaderValue;
            else
                return null;
        }
    }
}