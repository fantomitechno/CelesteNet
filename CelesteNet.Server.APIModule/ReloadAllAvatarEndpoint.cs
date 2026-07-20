using System.IO;
using System.Net;
using Celeste.Mod.CelesteNet.Server.API;
using WebSocketSharp.Server;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Celeste.Mod.CelesteNet.Server.Control
{
  public static partial class RCEndpoints
  {
    [RCEndpoint(true, "/reloadallavatar", "", "", "Reload all Avatar", "")]
    public static void ReloadAllAvatars(Frontend f, HttpRequestEventArgs c)
    {
      if (!f.IsAuthorizedExec(c))
      {
        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        f.Respond(c, "Unauthorized!");
        return;
      }

      string[] uids = f.Server.UserData.GetRegistered();
      if (f.Server.UserData is APIUserData userData)
      {
        foreach (string uid in uids)
        {
          BasicUserInfo info = f.Server.UserData.Load<BasicUserInfo>(uid);
          if (!info.Tags.Contains("fromAPI")) continue;

          var file = userData.Client.GetStreamAsync(userData.Settings.ApiBase + "/avatar?fallback=true&uid=" + uid).Await();

          Image avatarOrig;
          using (Stream? data = userData.Client.GetStreamAsync(userData.Settings.ApiBase + "/avatar?fallback=true&uid=" + uid).Await())
          {
            if (data == null)
              continue;

            avatarOrig = Image.Load<Rgba32>(data);
          }

          if (avatarOrig == null)
            continue;

          Logger.Log(LogLevel.INF, "frontend", $"({uid}) Loaded avatar.orig.png...");
          using (Image avatarFinal = avatarOrig.Clone(x => x.ApplyTagOverlays(f, info)))
          {
            Logger.Log(LogLevel.INF, "frontend", $"({uid}) Processing done, saving...");

            using (Stream s = f.Server.UserData.WriteFile(uid, "avatar.png"))
              avatarFinal.SaveAsPng(s, new PngEncoder() { ColorType = PngColorType.RgbWithAlpha });

            Logger.Log(LogLevel.INF, "frontend", $"({uid}) Saved avatar.png. ");
          }
        }
        f.Respond(c, "Success");
      }
      else
        f.Respond(c, "Not using the API UserData");
    }

    private static IImageProcessingContext ApplyTagOverlays(this IImageProcessingContext context, Frontend f, BasicUserInfo info)
    {
      foreach (string tagName in info.Tags)
      {
        using Stream? asset = f.OpenContent($"frontend/assets/tags/{tagName}.png", out _, out _, out _);
        if (asset == null)
          continue;

        using Image tag = Image.Load(asset);
        context.DrawImage(tag, 1.0f);
      }

      return context;
    }
  }
}
