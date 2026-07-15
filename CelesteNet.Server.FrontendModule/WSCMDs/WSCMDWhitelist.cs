using Newtonsoft.Json.Linq;
using System.Linq;

namespace Celeste.Mod.CelesteNet.Server.Control
{
  public class WSCMDWlAdd : WSCMD
  {
    public override bool MustAuth => true;
    public override object? Run(dynamic? input)
    {
      string? uidsRaw = input;
      string[]? uids = uidsRaw?.Split(",");

      if (uids == null) return false;

      foreach (var uid in uids)
      {
        WhiteListInfo whiteList = new()
        {
          UID = uid
        };
        Frontend.Server.UserData.Save(uid, whiteList);
      }

      return true;
    }
  }

  public class WSCMDWlRm : WSCMD
  {
    public override bool MustAuth => true;
    public override object? Run(dynamic? input)
    {
      string? uidsRaw = input;
      string[]? uids = uidsRaw?.Split(",");

      if (uids == null) return false;

      foreach (var uid in uids)
      {
        Frontend.Server.UserData.Delete<WhiteListInfo>(uid);
      }

      return true;
    }
  }
}
