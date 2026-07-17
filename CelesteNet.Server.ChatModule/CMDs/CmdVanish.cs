using System.Collections.Generic;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.CelesteNet.Server.Chat.Cmd
{
  public class CmdVanish : ChatCmd
  {

    public override string Info => "Vanish or unvanish";

    public override bool MustAuth => true;

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      if (env.Session == null)
        throw new CommandRunException("Are you trying to vanish as the server?");

      env.Session.Vanished = !env.Session.Vanished;

      env.Send($"You are now {(env.Session.Vanished ? "Hidden" : "Shown")}");
      DataInternalBlob? blobPlayerInfo = null;
      if (env.Session.Vanished)
      {
        blobPlayerInfo = DataInternalBlob.For(env.Server.Data, new DataPlayerState()
        {
          Player = new DataPlayerInfo()
          {
            ID = env.Session.SessionID
          },
          SID = "fantomitechno/jail",
          Level = "1",
        });
      }
      else
      {
        if (env.Session.PlayerInfo != null)
        {
          blobPlayerInfo = DataInternalBlob.For(env.Server.Data, env.Session.PlayerInfo);
        }
      }
      if (blobPlayerInfo == null)
      {
        env.Send($"An error happened???");
        return;
      }

      if (!env.Session.Vanished)
        env.Session.ResendPlayerStates();
      using (env.Server.ConLock.R())
        foreach (CelesteNetPlayerSession other in env.Server.Sessions)
        {
          if (other == env.Session)
            continue;

          DataPlayerInfo? otherInfo = other.PlayerInfo;
          if (otherInfo == null)
            continue;

          other.Con.Send(blobPlayerInfo);
          Logger.Log(LogLevel.DEV, "vanish", $"Sent {other.Name} that I connected??");

          if (!other.ClientOptions.AvatarsDisabled && env.Session.Vanished)
          {
            foreach (DataInternalBlob fragBlob in env.Session.AvatarFragments)
            {
              other.Con.Send(fragBlob);
            }
          }
        }
    }
  }
}
