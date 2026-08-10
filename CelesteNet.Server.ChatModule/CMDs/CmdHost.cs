using System.Collections.Generic;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.CelesteNet.Server.Chat.Cmd
{
  public class CmdHostMe : ChatCmd
  {

    public override string Info => "Become an host";

    public override bool MustAuth => true;

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      if (env.Session == null)
        throw new CommandRunException("Are you trying to be an host as the server?");

      env.Session.IsHost = !env.Session.IsHost;

      env.Send($"You are {(env.Session.IsHost ? "now" : "no longer")} an host");
      if (!env.Server.HostMode) return;
      if (!env.Session.IsHost)
      {
        using (env.Server.ConLock.R())
          foreach (CelesteNetPlayerSession other in env.Server.Sessions)
          {
            if (other == env.Session)
              continue;

            DataPlayerInfo? otherInfo = other.PlayerInfo;
            if (otherInfo == null)
              continue;

            env.Session.Con.Send(DataInternalBlob.For(env.Server.Data, new DataPlayerState()
            {
              Player = new DataPlayerInfo()
              {
                ID = other.SessionID
              },
            }));
          }
      }
      else
      {
        using (env.Server.ConLock.R())
          foreach (CelesteNetPlayerSession other in env.Server.Sessions)
          {
            if (other == env.Session)
              continue;

            DataPlayerInfo? otherInfo = other.PlayerInfo;
            if (otherInfo == null)
              continue;
            env.Server.Data.TryGetBoundRef(other.PlayerInfo, out DataPlayerState? state);
            env.Session.Con.Send(DataInternalBlob.For(env.Server.Data, state));
          }
      }
    }
  }

  public class CmdHostGame : ChatCmd
  {
    public override string Info => "Toogle the host mode";

    public override bool MustAuth => true;

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      env.Server.HostMode = !env.Server.HostMode;

      env.Send($"Server is {(env.Server.HostMode ? "now" : "no longer")} in host mode");

      foreach (var session in env.Server.Sessions)
      {
        DataInternalBlob? blobPlayerInfo = null;
        if (env.Server.HostMode)
        {
          blobPlayerInfo = DataInternalBlob.For(env.Server.Data, new DataPlayerState()
          {
            Player = new DataPlayerInfo()
            {
              ID = session.SessionID,
            },
            SID = session.IsHost ? ":celestenet_debugmap:" : "",
            Level = session.IsHost ? ":celestenet_debugmap:" : "",
          });
        }
        else
        {
          if (session.PlayerInfo != null)
          {
            env.Server.Data.TryGetBoundRef(session.PlayerInfo, out DataPlayerState? state);
            blobPlayerInfo = DataInternalBlob.For(env.Server.Data, state);
          }
        }
        if (blobPlayerInfo == null)
        {
          env.Send($"An error happened???");
          return;
        }

        if (!env.Server.HostMode)
          session.ResendPlayerStates();
        using (env.Server.ConLock.R())
          foreach (CelesteNetPlayerSession other in env.Server.Sessions)
          {
            if (other == session || other.IsHost || (other.Channel.Name == session.Channel.Name && session.Channel.Name.StartsWith("team")))
              continue;

            DataPlayerInfo? otherInfo = other.PlayerInfo;
            if (otherInfo == null)
              continue;

            other.Con.Send(blobPlayerInfo);
            Logger.Log(LogLevel.DEV, "vanish", $"Sent {other.Name} that I connected??");

            if (!other.ClientOptions.AvatarsDisabled && !env.Server.HostMode)
            {
              foreach (DataInternalBlob fragBlob in session.AvatarFragments)
              {
                other.Con.Send(fragBlob);
              }
            }
          }
      }
    }
  }
}
