using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Timelapse.Webapp.Tools;

namespace Timelapse.Webapp.Tools
{
  public class CommandExecuter : ProcessExecuter
  {
    public CommandExecuter(string command) : base(GetOsFilename(), GetOsCommand(command))
    {
    }

    private static string GetOsFilename()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return "bash";
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return "cmd.exe";
      throw new NotSupportedException();
    }

    private static string GetOsCommand(string command)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return $"/c {command}";
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return $@"-c ""{command.Replace(@"""", @"""""")}""";
      throw new NotSupportedException();
    }

  }

}
