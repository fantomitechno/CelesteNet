using System;
using System.Collections.Generic;
using System.Threading;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.CelesteNet.Server.Chat.Cmd
{
    public class CmdCountdown : ChatCmd
    {

        public override string Info => "Play a countdown";

        public override bool MustAuth => true;

        public override void Init(ChatModule chat)
        {
            Chat = chat;

            ArgParser parser = new(chat, this);
            parser.AddParameter(new ParamInt(chat, null, ParamFlags.NonNegative, 5));
            ArgParsers.Add(parser);
        }

        public override void Run(CmdEnv env, List<ICmdArg>? args)
        {
            if (args == null || args.Count == 0 || args[0] is not CmdArgInt argTime)
                throw new CommandRunException("No time.");
            var th = new Thread(ExecuteInForeground);
            List<object> list = [argTime, env.Server];
            th.Start(new CountdownThreadInfo()
            {
                steps = argTime.Int,
                server = env.Server
            });
        }

        private static void ExecuteInForeground(object? obj)
        {
            if (obj == null) return;
            CountdownThreadInfo values;
            try
            {
                values = (CountdownThreadInfo)obj;
            }
            catch (InvalidCastException)
            {
                Logger.Log(LogLevel.DEV, "CmdCountdown(Thread)", obj.ToString() ?? "null");
                return;
            }

            for (int i = values.steps; i >= 0; i--)
            {
                values.server?.BroadcastAsync(new DataServerStatus
                {
                    Text = "Countdown: " + (i == 0 ? "GO" : i.ToString()),
                    Spin = true,
                    Time = 1
                });


                Thread.Sleep(1000);
            }
        }
    }

    public class CountdownThreadInfo
    {
        public int steps { get; set; } = 5;
        public CelesteNetServer? server { get; set; } = null;
    }
}
