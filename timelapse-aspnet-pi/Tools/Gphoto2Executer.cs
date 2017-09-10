using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Timelapse.Webapp.Tools
{
  public class Gphoto2Executer
  {
    public Task<string> Summary()
    {
      using (var executer = new CommandExecuter("gphoto2 --summary"))
      {
        return executer.Execute();
      }
    }

    public async Task<DateTime> GetDateTime()
    {
      using (var executer = new CommandExecuter("gphoto2 --get-config /main/status/datetime"))
      { /*
Detected a 'Canon:EOS 350D (normal mode)'.
Label: Date and Time
Type: TEXT
Current: 2017-09-10 20:03:46
*/
        var lines = await executer.Execute();
        var date = lines.GetFirstLineStartingWith("Current: ");
        if (string.IsNullOrWhiteSpace(date))
          return DateTime.MinValue;

        DateTime result;
        if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
          return result;

        return DateTime.MinValue;
      }
    }

    public async Task<bool> SyncDateTime()
    {
      using (var executer = new CommandExecuter("gphoto2 --set-config /main/actions/syncdatetime=1"))
      {/*
Detected a 'Canon:EOS 350D (normal mode)'.
time set
*/
        var lines = await executer.Execute();
        return !string.IsNullOrWhiteSpace(lines.GetFirstLineStartingWith("time set"));
      }
    }

    public async Task<ImageInfo> CaptureImageAndDownload(bool returnImageData = false, string path = "/data/")
    {
      using (var executer = new CommandExecuter($"gphoto2 --capture-image-and-download --filename '{path}%Y%m%d-%H%M%S.jpg'"))
      {/*
Detected a 'Canon:EOS 350D (normal mode)'.
New file is in location /DCIM/351CANON/IMG_5101.JPG on the camera

*** Error ***
canon_usb_set_file_attributes: canon_usb_dialogue failed
Saving file as 20170910-203546.jpg
Deleting file /DCIM/351CANON/IMG_5101.JPG on the camera
*/
        var lines = await executer.Execute();
        var filename = lines.GetFirstLineStartingWith("Saving file as ", true);

        var result = new ImageInfo()
        {
          Filename = filename,
          MimeType = "image/jpeg"
        };

        if (returnImageData)
          result.Data = File.ReadAllBytes(filename);

        return result;
      }
    }

    public class ImageInfo
    {
      public string Filename { get; set; }

      public string MimeType { get; set; }
      public byte[] Data { get; set; }
    }
  }

  public static class StringExtensions
  {
    public static string GetFirstLineStartingWith(this string text, string startsWith, bool removeStartsWith = false)
    {
      var line = text
        .Split('\r', '\n')
        .FirstOrDefault(l => l.StartsWith(startsWith));
      if (removeStartsWith)
        line = line?.Substring(startsWith.Length);
      return line;
    }
  }


}
