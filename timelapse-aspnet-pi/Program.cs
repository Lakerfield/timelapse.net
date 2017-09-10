﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApp1
{
  public class Program
  {
    public static void Main(string[] args)
    {
      foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
      {
        Console.Write($"{Environment.NewLine}{networkInterface.Id},{networkInterface.Description},{networkInterface.Name}{Environment.NewLine}");

        foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
        {
          Console.Write($"- {address.Address}{Environment.NewLine}");
        }
      }


      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
  }
}