using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Timelapse.Webapp.Tools
{
  public class ProcessExecuter : IDisposable
  {
    private TaskCompletionSource<bool> _processExited;
    private Process _process;
    private StringBuilder _results;

    public ProcessExecuter(string filename, string arguments)
    {

      _processExited = new TaskCompletionSource<bool>();
      _results = new StringBuilder();
      
      //* Create your Process
      _process = new Process();
      _process.StartInfo.FileName = filename;
      _process.StartInfo.Arguments = arguments;
      _process.StartInfo.UseShellExecute = false;
      _process.StartInfo.RedirectStandardOutput = true;
      _process.StartInfo.RedirectStandardError = false;
      _process.StartInfo.RedirectStandardInput = true;
      _process.StartInfo.CreateNoWindow = false;

      //* Set your output and error (asynchronous) handlers
      _process.Exited += ProcessOnExited;
      
      _process.EnableRaisingEvents = true;
    }

    public async Task<string> Execute()
    {
      //* Start process and handlers
      _process.Start();

      var output = HandleStream(_process.StandardOutput);
      //var error = HandleStream(_process.StandardError, true);

      await Task.WhenAll(_processExited.Task, output);//, error);

      return _results.ToString();
    }

    private async Task HandleStream(StreamReader output, bool error = false)
    {
      while (!output.EndOfStream)
      {
        var line = await output.ReadLineAsync();
        _results.AppendLine(line);
        HandleLine(line, error);
      }
    }

    private void HandleLine(string line, bool error)
    {
      lock (_processExited)
      {
        Console.WriteLine(line);
      }
    }

    private void ProcessOnExited(object o, EventArgs eventArgs)
    {
      _processExited.TrySetResult(true);
    }

    public void Dispose()
    {
      _process.Exited -= ProcessOnExited;

      _process.Dispose();
    }
  }
}
