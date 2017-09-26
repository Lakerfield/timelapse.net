using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Timelapse.Webapp.Tools;

namespace Timelapse.Webapp.Pages
{
  public class HealthModel : PageModel
  {
    private readonly TimelapseService _timelapseController;

    public HealthModel(TimelapseService timelapseController)
    {
      _timelapseController = timelapseController;
    }

    public ActionResult OnGet()
    {
      //https://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers/2068407#2068407
      HttpContext.Response.Headers[HeaderNames.CacheControl] = "no-store, must-revalidate";
      
      if (_timelapseController.IsTimerRunning)
        return Page();

      return StatusCode(500);
    }
  }
}