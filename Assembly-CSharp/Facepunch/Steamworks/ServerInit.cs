// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.ServerInit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class ServerInit
  {
    public uint IpAddress = 0;
    public ushort SteamPort;
    public ushort GamePort = 27015;
    public ushort QueryPort = 27016;
    public bool Secure = true;
    public string VersionString = "2.0.0.0";
    public string ModDir = "unset";
    public string GameDescription = "unset";

    public ServerInit(string modDir, string gameDesc)
    {
      this.ModDir = modDir;
      this.GameDescription = gameDesc;
    }

    public ServerInit RandomSteamPort()
    {
      this.SteamPort = (ushort) new Random().Next(10000, 60000);
      return this;
    }

    public ServerInit QueryShareGamePort()
    {
      this.QueryPort = ushort.MaxValue;
      return this;
    }
  }
}
