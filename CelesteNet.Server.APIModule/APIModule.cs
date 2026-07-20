using Celeste.Mod.CelesteNet.Server.Sqlite;

namespace Celeste.Mod.CelesteNet.Server.API
{
  public class APIModule : CelesteNetServerModule<APISettings>
  {

    public override void LoadSettings()
    {
      base.LoadSettings();

      UpdateUserData(Settings.Enabled);
    }

    public override void SaveSettings()
    {
      base.SaveSettings();

      UpdateUserData(Settings.Enabled);
    }

    public override void Dispose()
    {
      base.Dispose();

      UpdateUserData(false);
    }

    public void UpdateUserData(bool enabled)
    {
      bool active = Server.UserData is APIUserData;

      Logger.Log(LogLevel.VVV, "apiModule", $"UpdateUserData {active} {enabled}");

      if (!active && enabled)
      {
        Server.UserData.Dispose();
        Server.UserData = new APIUserData(this);
      }
      else if (active && !enabled)
      {
        Server.UserData.Dispose();
        Server.UserData = new FileSystemUserData(Server);
      }

      if (Settings.UseSqliteFallback && Server.UserData is APIUserData userData)
      {
        bool sqliteActive = userData.Fallback is SqliteUserData;

        if (!sqliteActive && enabled)
        {
          Server.ModuleMap.TryGetValue(typeof(SqliteModule), out CelesteNetServerModule? module);
          if (module is SqliteModule sqliteModule)
          {
            userData.Fallback.Dispose();
            userData.Fallback = new SqliteUserData(sqliteModule);
          }
        }
        else if (sqliteActive && !enabled)
        {
          userData.Fallback.Dispose();
          userData.Fallback = new FileSystemUserData(Server);
        }
      }
    }
  }
}
