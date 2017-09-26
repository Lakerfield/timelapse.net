using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Timelapse.Webapp.Models
{
  public class TimelapseSettings
  {

    [Required]
    public TimeSpan Every { get; set; }

    [Required]
    public TimeSpan From { get; set; }

    [Required]
    public TimeSpan Till { get; set; }

  }
}
