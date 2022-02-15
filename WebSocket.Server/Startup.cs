using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket.Server.Helpers;

namespace WebSocket.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseWebSockets();
            // a middleware to handle in comming requests
            app.Use(async (httpContext, next) =>
            {
                #region Guard Closures
                if (httpContext.Request.Path != "/ws")
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (!httpContext.WebSockets.IsWebSocketRequest)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }
                #endregion

                await WebSocketHelper.AcceptTcpClientConnection(httpContext);
            });
        }
    }
}
