using System;
using System.Collections.Generic;

namespace Facepunch.Steamworks
{
  public class User : IDisposable
  {
    internal Client client;
    internal Dictionary<string, string> richPresence = new();

    internal User(Client c) => client = c;

    public void Dispose() => client = null;

    public string GetRichPresence(string key)
    {
      return richPresence.TryGetValue(key, out string str) ? str : null;
    }

    public void SetRichPresence(string key, string value)
    {
      richPresence[key] = value;
      client.native.friends.SetRichPresence(key, value);
    }

    public void ClearRichPresence()
    {
      richPresence.Clear();
      client.native.friends.ClearRichPresence();
    }
  }
}
