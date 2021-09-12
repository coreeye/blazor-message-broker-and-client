using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MessageBroker.Server.Networking
{
    public static class WebSocketManagerExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, 
                                                              PathString path,
                                                              IWebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
    }
}