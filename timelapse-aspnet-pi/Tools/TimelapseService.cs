using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timelapse.Webapp.Models;

namespace Timelapse.Webapp.Tools
{
  public class TimelapseService
  {
    private readonly object _timelapseTimerLockObject = new object();
    private CancellationTokenSource _timelapseCancellationTokenSource;
    private TimelapseTimer _timelapseTimer;

    public bool IsTimerRunning => _timelapseTimer != null;
    public string PhotoPath { get; }

    public FileInfo LatestPhoto { get; private set; }
    public TimelapseSettings Settings
    {
      get => new TimelapseSettings()
      {
        Every = _timelapseTimer?.RunEvery ?? TimeSpan.FromSeconds(150),
        From = _timelapseTimer?.RunFrom ?? new TimeSpan(06, 55, 00),
        Till = _timelapseTimer?.RunTill ?? new TimeSpan(17, 01, 00),
      };
      set => _timelapseTimer?.Update(value);
    }

    public TimelapseService()
    {
      PhotoPath = "/data"; // TODO: PhotoPath from config
    }

    public async Task Init()
    {
      var directoryInfo = new DirectoryInfo(PhotoPath);
      if (directoryInfo.Exists)
        LatestPhoto = directoryInfo.GetFiles("*.jpg").OrderByDescending(f => f.Name).FirstOrDefault();

      var executer = new Gphoto2Executer();
      var results = await executer.SyncDateTime();
    }

    public bool StartTimer()
    {
      bool isStarted = false;
      lock (_timelapseTimerLockObject)
      {
        if (_timelapseTimer == null)
        {
          _timelapseCancellationTokenSource = new CancellationTokenSource();
          _timelapseTimer = new TimelapseTimer(
            Settings,
            async () =>
            {
              await Init();
            },
            async () =>
            {
              Console.WriteLine($"Start action on {DateTime.Now.ToLongTimeString()}");

              var executer = new Gphoto2Executer();
              var results = await executer.CaptureImageAndDownload();
            });

          var runTask = _timelapseTimer.Run(_timelapseCancellationTokenSource.Token);
          isStarted = true;
        }
      }
      return isStarted;
    }

    public bool StopTimer()
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
