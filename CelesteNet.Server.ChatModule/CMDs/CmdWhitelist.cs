using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.CelesteNet.DataTypes;
using MonoMod.Utils;

namespace Celeste.Mod.CelesteNet.Server.Chat.Cmd
{
  public class CmdWlAdd : ChatCmd
  {

    public override string Info => "Add a player to the whitelist.";

    public override bool MustAuth => true;

    public override void Init(ChatModule chat)
    {
      Chat = chat;

      ArgParser parser = new(chat, this);
      parser.AddParameter(new ParamString(chat));
      ArgParsers.Add(parser);
    }

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      if (args == null || args.Count == 0)
        throw new CommandRunException("No user.");

      if (args[0] is not CmdArgString uid)
      {
        throw new CommandRunException("Invalid username or ID.");
      }

      WhiteListInfo whiteList = new()
      {
        UID = uid
      };
      env.Server.UserData.Save(uid, whiteList);

      env.Send($"Whitelisted {uid}");
    }
  }

  public class CmdWlRm : ChatCmd
  {

    public override string Info => "Remove a player from the whitelist.";

    public override bool MustAuth => true;

    public override void Init(ChatModule chat)
    {
      Chat = chat;

      ArgParser parser = new(chat, this);
      parser.AddParameter(new ParamString(chat));
      ArgParsers.Add(parser);
    }

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      if (args == null || args.Count == 0)
        throw new CommandRunException("No user.");

      if (args[0] is not CmdArgString uid)
      {
        throw new CommandRunException("Invalid username or ID.");
      }

      env.Server.UserData.Delete<WhiteListInfo>(uid);

      env.Send($"Unwhitelisted {uid}");
    }
  }

  public class CmdWlList : ChatCmd
  {

    public override string Info => "List the whitelist.";

    public override bool MustAuth => true;

    public override void Run(CmdEnv env, List<ICmdArg>? args)
    {
      List<string> whiteList = env.Server.UserData.LoadAll<WhiteListInfo>().Keys.ToList();

      env.Send($"Whitelist is {whiteList}");
    }
  }
}
