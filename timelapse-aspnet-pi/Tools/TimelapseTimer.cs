using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Timelapse.Webapp.Tools
{
  public class TimelapseTimer
  {
    private readonly Func<Task> _action;
    public TimeSpan RunEvery { get; }
    public TimeSpan RunFrom { get; }
    public TimeSpan RunTill { get; }

    public TimelapseTimer(TimeSpan runEvery, TimeSpan runFrom, TimeSpan runTill, Func<Task> action)
    {
      _action = action;
      RunEvery = runEvery;
      RunFrom = runFrom;
      RunTill = runTill;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
      Console.WriteLine($"Timer started for every {RunEvery} between {RunFrom} and {RunTill}");
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
