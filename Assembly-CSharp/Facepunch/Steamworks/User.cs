// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.User
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Facepunch.Steamworks
{
  public class User : IDisposable
  {
    internal Client client;
    internal Dictionary<string, string> richPresence = new Dictionary<string, string>();

    internal User(Client c) => this.client = c;

    public void Dispose() => this.client = (Client) null;

    public string GetRichPresence(string key)
    {
      string str = (string) null;
      return this.richPresence.TryGetValue(key, out str) ? str : (string) null;
    }

    public void SetRichPresence(string key, string value)
    {
      this.richPresence[key] = value;
      this.client.native.friends.SetRichPresence(key, value);
    }

    public void ClearRichPresence()
    {
      this.richPresence.Clear();
      this.client.native.friends.ClearRichPresence();
    }
  }
}
