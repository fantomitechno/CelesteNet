
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace Celeste.Mod.CelesteNet.Server.API
{
  public class APIUserData : UserData
  {
    internal readonly HttpClient Client;

    internal UserData Fallback;

    internal readonly APISettings Settings;

    public APIUserData(APIModule module) : base(module.Server)
    {
      Client = new();
      Fallback = new FileSystemUserData(Server);
      Settings = module.Settings;

      Client.DefaultRequestHeaders.UserAgent.Clear();
      Client.DefaultRequestHeaders.UserAgent.ParseAdd("CelesteNet APIModule for " + Settings.InstanceNameForUserAgent + "/" + CelesteNetUtils.LoadedVersion);
    }

    public override void CopyTo(UserData other)
    {
      Fallback.CopyTo(other);
    }

    public override string Create(string uid, bool forceNewKey)
    {
      return Fallback.Create(uid, forceNewKey);
    }

    public override void Delete<T>(string uid)
    {
      Fallback.Delete<T>(uid);
    }

    public override void DeleteFile(string uid, string name)
    {
      Fallback.DeleteFile(uid, name);
    }

    public override void Dispose()
    {
      Fallback.Dispose();
      Client.Dispose();
    }

    public override string[] GetAll()
    {
      return Fallback.GetAll();
    }

    public override int GetAllCount()
    {
      return Fallback.GetAllCount();
    }

    public override string GetKey(string uid)
    {
      return Fallback.GetKey(uid);
    }

    public override string[] GetRegistered()
    {
      return Fallback.GetRegistered();
    }

    public override int GetRegisteredCount()
    {
      var res = Client.GetAsync(Settings.ApiBase + "/status").Await();
      var json = res.Content.ReadFromJsonAsync<RequestStats>().Await();

      if (json == null)
      {
        return 0;
      }

      return json.Registered;
    }

    public override string GetUID(string key)
    {
      if (key.IsNullOrEmpty())
        return "";
      var res = Client.GetAsync(Settings.ApiBase + "/userinfo?key=" + key).Await();
      var json = res.Content.ReadFromJsonAsync<RequestData>().Await();

      if (json == null)
      {
        return "";
      }

      Fallback.Insert(json.UID, json.Key, json.Key, true);

      return json.UID;
    }

    public override bool HasFile(string uid, string name)
    {
      return name == "avatar.png" || Fallback.HasFile(uid, name);
    }

    public override void Insert(string uid, string key, string keyFull, bool registered)
    {
      Fallback.Insert(uid, key, keyFull, registered);
    }

    public override void InsertData(string uid, string name, Type? type, Stream stream)
    {
      Fallback.InsertData(uid, name, type, stream);
    }

    public override void InsertFile(string uid, string name, Stream stream)
    {
      Fallback.InsertFile(uid, name, stream);
    }

    public override Dictionary<string, T> LoadAll<T>()
    {
      return Fallback.LoadAll<T>();
    }

    public override Dictionary<string, T> LoadRegistered<T>()
    {
      return Fallback.LoadRegistered<T>();
    }

    public override Stream? ReadFile(string uid, string name)
    {
      var stream = Fallback.ReadFile(uid, name);
      if (name == "avatar.png" && stream == null)
      {
        var file = Client.GetStreamAsync(Settings.ApiBase + "/avatar?fallback=true&uid=" + uid).Await();
        Fallback.InsertFile(uid, name, file);
        return Fallback.ReadFile(uid, name);
      }
      return stream;
    }

    public override void RevokeKey(string key)
    {
      Fallback.RevokeKey(key);
      Client.GetAsync(Settings.ApiBase + "/revokekey?key=" + key);
    }

    public override void Save<T>(string uid, T value)
    {
      Fallback.Save(uid, value);
    }

    public override bool TryLoad<T>(string uid, out T value)
    {
      value = new();
      if (uid.IsNullOrEmpty())
        return false;

      var fback = Fallback.TryLoad(uid, out value);

      if (typeof(T) == typeof(BasicUserInfo) && !fback)
      {

        var res = Client.GetAsync(Settings.ApiBase + "/userinfo?uid=" + uid).Await();
        var json = res.Content.ReadFromJsonAsync<T>().Await();

        if (json == null)
        {
          return false;
        }
        value = json;
        var toModify = (BasicUserInfo)(Object)value;

        if (Settings.InstanceMods.Contains(uid))
        {
          toModify.Tags.Add(BasicUserInfo.TAG_AUTH);
        }
        else
        {
          toModify.Tags.Remove(BasicUserInfo.TAG_AUTH);
        }

        if (Settings.InstanceAdmins.Contains(uid))
        {
          toModify.Tags.Add(BasicUserInfo.TAG_AUTH_EXEC);
        }
        else
        {
          toModify.Tags.Remove(BasicUserInfo.TAG_AUTH_EXEC);
        }

        value = (T)(Object)toModify;

        Fallback.Save(uid, value);

        return true;
      }

      return fback;
    }

    public override void Wipe(string uid)
    {
      Fallback.Wipe(uid);
    }

    public override Stream WriteFile(string uid, string name)
    {
      return Fallback.WriteFile(uid, name);
    }



    private class RequestData : IUserDataType
    {
      public string UID { get; set; } = "";
      public string Name { get; set; } = "";
      public string Discrim { get; set; } = "";
      public HashSet<string> Tags { get; set; } = new();
      public string Key { get; set; } = "";
    }

    private class RequestStats
    {
      public int PlayerCounter { get; set; } = 0;
      public int Registered { get; set; } = 0;
      public int Banned { get; set; } = 0;
    }
  }
}