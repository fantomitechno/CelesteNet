
using System.Net;
using Celeste.Mod.CelesteNet.Server.API;
using WebSocketSharp.Server;

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
          var file = userData.Client.GetStreamAsync(userData.Settings.ApiBase + "/avatar?fallback=true&uid=" + uid).Await();
          userData.Fallback.InsertFile(uid, "avatar.png", file);
        }
        f.Respond(c, "Success");
      }
      else
        f.Respond(c, "Not using the API UserData");
    }
  }
}
