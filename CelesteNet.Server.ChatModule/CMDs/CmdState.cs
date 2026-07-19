using System.Collections.Generic;

namespace Celeste.Mod.CelesteNet.Server.Chat.Cmd
{
  public class CmdState : ChatCmd
  {

    public override string Info => "Get server state.";

    public override bool MustAuth => true;

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      List<string> hosts = [];
      List<string> vanished = [];
      foreach (var session in env.Server.Sessions)
      {
        if (session.IsHost)
          hosts.Add(session.Name);
        if (session.Vanished)
          vanished.Add(session.Name);
      }
      env.Send($"State of the server\nHost Mode: {env.Server.HostMode} ({string.Join(", ", hosts)})\nVanished players: {string.Join(", ", vanished)}");
    }

  }
}
