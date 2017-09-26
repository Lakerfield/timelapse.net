using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timelapse.Webapp.Models;

namespace Timelapse.Webapp.Tools
{
  public class TimelapseTimer
  {
    private readonly Func<Task> _initAction;
    private readonly Func<Task> _action;
    public TimeSpan RunEvery { get; private set; }
    public TimeSpan RunFrom { get; private set; }
    public TimeSpan RunTill { get; private set; }

    public TimelapseTimer(TimelapseSettings settings, Func<Task> initAction, Func<Task> action)
    {
      _initAction = initAction;
      _action = action;

      Update(settings);
    }

    public void Update(TimelapseSettings settings)
    {
      RunEvery = settings.Every;
      RunFrom = settings.From;
      RunTill = settings.Till;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
      Console.WriteLine($"{DateTime.Now} Timer started every {RunEvery} between {RunFrom} and {RunTill}");
      await _initAction();
      Console.WriteLine($"{DateTime.Now} Init complete");

      again:
      try
      {
        var time = DateTime.Now.TimeOfDay;
        if (time >= RunFrom && time <= RunTill)
          await _action();
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }

      // delay to prevent loop if action is executed in less than a millisecond
      await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

      var now = DateTime.Now;
      var waitFor = RunEvery - TimeSpan.FromMilliseconds(now.TimeOfDay.TotalMilliseconds % RunEvery.TotalMilliseconds);

      if (cancellationToken.IsCancellationRequested)
        return;

      await Task.Delay(waitFor, cancellationToken);

      if (cancellationToken.IsCancellationRequested)
        return;

      goto again;
    }

  }
}
