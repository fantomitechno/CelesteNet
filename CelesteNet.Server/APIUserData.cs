
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace Celeste.Mod.CelesteNet.Server
{
  public class APIUserData(CelesteNetServer server) : UserData(server)
  {

    readonly HttpClient client = new();

    readonly string baseApi = "https://celestenet.0x0a.de/api";

    public override void CopyTo(UserData other)
    {
      throw new NotImplementedException();
    }

    public override string Create(string uid, bool forceNewKey)
    {
      throw new NotImplementedException();
    }

    public override void Delete<T>(string uid)
    {
      throw new NotImplementedException();
    }

    public override void DeleteFile(string uid, string name)
    {
      throw new NotImplementedException();
    }

    public override void Dispose()
    {
    }

    public override string[] GetAll()
    {
      return [];
    }

    public override int GetAllCount()
    {
      return 141;
    }

    public override string GetKey(string uid)
    {
      if (uid.IsNullOrEmpty())
        return "";
      var res = client.GetAsync(this.baseApi + "/userinfo?uid=" + uid).Await();
      var json = res.Content.ReadFromJsonAsync<RequestData>().Await();

      if (json == null)
      {
        return "";
      }

      return json.UID;
    }

    public override string[] GetRegistered()
    {
      throw new NotImplementedException();
    }

    public override int GetRegisteredCount()
    {
      var res = client.GetAsync(this.baseApi + "/status").Await();
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
      var res = client.GetAsync(this.baseApi + "/userinfo?key=" + key).Await();
      var json = res.Content.ReadFromJsonAsync<RequestData>().Await();

      if (json == null)
      {
        return "";
      }

      return json.UID;
    }

    public override bool HasFile(string uid, string name)
    {
      throw new NotImplementedException();
    }

    public override void Insert(string uid, string key, string keyFull, bool registered)
    {
      throw new NotImplementedException();
    }

    public override void InsertData(string uid, string name, Type? type, Stream stream)
    {
      throw new NotImplementedException();
    }

    public override void InsertFile(string uid, string name, Stream stream)
    {
      throw new NotImplementedException();
    }

    public override Dictionary<string, T> LoadAll<T>()
    {
      if (typeof(T) == typeof(BanInfo))
      {
        var res = client.GetAsync(this.baseApi + "/status").Await();
        var json = res.Content.ReadFromJsonAsync<RequestStats>().Await();

        if (json == null)
        {
          return [];
        }
        var list = new List<string>();

        for (int i = 0; i < json.Banned; i++)
        {
          list.Add(i.ToString());
        }

        return list.ToDictionary(d => d, d => new T());
      }

      throw new NotImplementedException();
    }

    public override Dictionary<string, T> LoadRegistered<T>()
    {
      throw new NotImplementedException();
    }

    public override Stream? ReadFile(string uid, string name)
    {
      throw new NotImplementedException();
    }

    public override void RevokeKey(string key)
    {
      throw new NotImplementedException();
    }

    public override void Save<T>(string uid, T value)
    {
      throw new NotImplementedException();
    }

    public override bool TryLoad<T>(string uid, out T value)
    {
      value = new();
      if (uid.IsNullOrEmpty())
        return false;
      var res = client.GetAsync(this.baseApi + "/userinfo?uid=" + uid).Await();
      var json = res.Content.ReadFromJsonAsync<T>().Await();

      if (json == null)
      {
        return false;
      }
      value = json;
      return true;
    }

    public override void Wipe(string uid)
    {
      throw new NotImplementedException();
    }

    public override Stream WriteFile(string uid, string name)
    {
      throw new NotImplementedException();
    }



    public class RequestData : IUserDataType
    {
      public string UID { get; set; } = "";
      public string Name { get; set; } = "";
      public string Discrim { get; set; } = "";
      public HashSet<string> Tags { get; set; } = new();
      public String Key { get; set; } = "";
    }

    public class RequestStats
    {
      public int PlayerCounter { get; set; } = 0;
      public int Registered { get; set; } = 0;
      public int Banned { get; set; } = 0;
    }
  }
}