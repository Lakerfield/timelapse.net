using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Timelapse.Webapp.Tools;

namespace Timelapse.Webapp.Pages
{
  public class IndexModel : PageModel
  {
    public TimelapseService TimelapseService { get; }

    public DateTime LatestPhotoDateTime => TimelapseService.LatestPhoto?.CreationTime ?? DateTime.MinValue;

    public IndexModel(TimelapseService timelapseService)
    {
      TimelapseService = timelapseService;
    }

    public void OnGet()
    {
      //https://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers/2068407#2068407
      HttpContext.Response.Headers[HeaderNames.CacheControl] = "no-store, must-revalidate";
    }


  }
}
