using System;

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
