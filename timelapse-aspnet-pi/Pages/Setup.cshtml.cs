using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Timelapse.Webapp.Models;
using Timelapse.Webapp.Tools;

namespace Timelapse.Webapp.Pages
{
  public class SetupModel : PageModel
  {
    private readonly TimelapseService _timelapseController;

    [BindProperty]
    public TimelapseSettings Settings { get; set; }

    public SetupModel(TimelapseService timelapseController)
    {
      _timelapseController = timelapseController;
    }

    public void OnGet()
    {
      Settings = _timelapseController.Settings;
    }

    public IActionResult OnPostAsync()
    {
      if (!ModelState.IsValid)
        return Page();

      _timelapseController.Settings = Settings;
      return RedirectToPage("/Index");
    }

  }
}