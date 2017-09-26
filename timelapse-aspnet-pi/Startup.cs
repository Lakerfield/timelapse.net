using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Timelapse.Webapp.Tools;

namespace Timelapse.Webapp
{
  public class Startup
  {
    public static DateTime StartupDateTime;

    private object _timelapseTimerLockObject = new object();
    private CancellationTokenSource _timelapseCancellationTokenSource;
    private TimelapseTimer _timelapseTimer;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/start"), subApp =>
      {
        subApp.Run(async context =>
        {
          context.Response.ContentType = "text/plain";
          var isStarted = StartTimer();
          await context.Response.WriteAsync(isStarted ? "Timelapse started" : "Timelapse already running");
        });
      });

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/stop"), subApp =>
      {
        subApp.Run(async context =>
        {
          context.Response.ContentType = "text/plain";
          var isStopped = StopTimer();
          await context.Response.WriteAsync(isStopped ? "Timelapse stopped" : "Timelapse was not running");
        });
      });

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/test"), subApp =>
      {
        subApp.Run(async context =>
        {
          var executer = new Gphoto2Executer();

          var info = await executer.Summary();
          context.Response.ContentType = "text/plain";
          await context.Response.WriteAsync(info);
        });
      });

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/date"), subApp =>
      {
        subApp.Run(async context =>
        {
          var executer = new Gphoto2Executer();

          var dateTime = await executer.GetDateTime();
          await context.Response.WriteAsync($"Current date/time of the camera is {dateTime.ToLongDateString()} {dateTime.ToLongTimeString()}");
        });
      });

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/capture"), subApp =>
      {
        subApp.Run(async context =>
        {
          var executer = new Gphoto2Executer();
          var results = await executer.CaptureImageAndDownload();
          context.Response.ContentType = results.MimeType;
          await context.Response.SendFileAsync(results.Filename);
        });
      });

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/latest"), subApp =>
      {
        subApp.Run(async context =>
        {
          var directoryInfo = new DirectoryInfo("/data");
          var files = directoryInfo.GetFiles("*.jpg");
          var latestFile = files.OrderByDescending(f => f.Name).FirstOrDefault();
          if (latestFile == null)
          {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("No files found");
            return;
          }
          context.Response.ContentType = "image/jpeg";
          await context.Response.SendFileAsync(latestFile.FullName);
        });
      });

      app.UseMvc();

      app.MapWhen(context => context.Request.Path.StartsWithSegments("/health"), subApp =>
      {
        subApp.Run(async context =>
        {
          var healty = _timelapseTimer != null;
          if (!healty)
          {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Sick on {DateTime.Now}");
            return;
          }

          context.Response.ContentType = "text/plain";
          await context.Response.WriteAsync($"Alive on {DateTime.Now}");
        });
      });

      app.Run(async (context) =>
      {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"Resource not found on {DateTime.Now}");
      });

      //app.Run(async (context) =>
      //{
      //  var httpConnectionFeature = context.Features.Get<IHttpConnectionFeature>();
      //  var localIpAddress = httpConnectionFeature?.LocalIpAddress;

      //  context.Response.ContentType = "text/plain";

      //  await context.Response.WriteAsync($"Hello World from webapp 1 on host {localIpAddress}!{Environment.NewLine}");
      //  foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
      //  {
      //    await context.Response.WriteAsync($"{Environment.NewLine}{networkInterface.Id},{networkInterface.Description},{networkInterface.Name}{Environment.NewLine}");
      //    foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
      //      await context.Response.WriteAsync($"- {address.Address}{Environment.NewLine}");
      //  }
      //});

      StartupDateTime = DateTime.Now;
      StartTimer();
    }

    private bool StartTimer()
    {
      bool isStarted = false;
      lock (_timelapseTimerLockObject)
      {
        if (_timelapseTimer == null)
        {
          _timelapseCancellationTokenSource = new CancellationTokenSource();
          _timelapseTimer = new TimelapseTimer(
            TimeSpan.FromSeconds(150), 
            new TimeSpan(06, 55, 00), 
            new TimeSpan(17, 01, 00), 
            async () =>
            {
              Console.WriteLine($"Start action on {DateTime.Now.ToLongTimeString()}");

              var executer = new Gphoto2Executer();
              var results = await executer.CaptureImageAndDownload();
            });

          _timelapseTimer.Run(_timelapseCancellationTokenSource.Token);
          isStarted = true;
        }
      }
      return isStarted;
    }

    private bool StopTimer()
    {
      bool isStopped = false;
      lock (_timelapseTimerLockObject)
      {
        if (_timelapseTimer != null)
        {
          _timelapseCancellationTokenSource.Cancel();
          _timelapseTimer = null;
          _timelapseCancellationTokenSource = null;
          isStopped = true;
        }
      }
      return isStopped;
    }

  }
}
