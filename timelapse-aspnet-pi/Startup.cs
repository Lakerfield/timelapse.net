using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp1
{
  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.Run(async (context) =>
      {
        var httpConnectionFeature = context.Features.Get<IHttpConnectionFeature>();
        var localIpAddress = httpConnectionFeature?.LocalIpAddress;

        context.Response.ContentType = "text/plain";

        await context.Response.WriteAsync($"Hello World from webapp 1 on host {localIpAddress}!{Environment.NewLine}");

        foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
          await context.Response.WriteAsync($"{Environment.NewLine}{networkInterface.Id},{networkInterface.Description},{networkInterface.Name}{Environment.NewLine}");

          foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
          {
            await context.Response.WriteAsync($"- {address.Address}{Environment.NewLine}");

          }

        }


      });
    }
  }
}
