// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Overlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Overlay
  {
    internal Client client;

    public void OpenUserPage(string name, ulong steamid)
    {
      this.client.native.friends.ActivateGameOverlayToUser(name, (CSteamID) steamid);
    }

    public void OpenProfile(ulong steamid) => this.OpenUserPage(nameof (steamid), steamid);

    public void OpenChat(ulong steamid) => this.OpenUserPage("chat", steamid);

    public void OpenTrade(ulong steamid) => this.OpenUserPage("jointrade", steamid);

    public void OpenStats(ulong steamid) => this.OpenUserPage("stats", steamid);

    public void OpenAchievements(ulong steamid) => this.OpenUserPage("achievements", steamid);

    public void AddFriend(ulong steamid) => this.OpenUserPage("friendadd", steamid);

    public void RemoveFriend(ulong steamid) => this.OpenUserPage("friendremove", steamid);

    public void AcceptFriendRequest(ulong steamid)
    {
      this.OpenUserPage("friendrequestaccept", steamid);
    }

    public void IgnoreFriendRequest(ulong steamid)
    {
      this.OpenUserPage("friendrequestignore", steamid);
    }

    public void OpenUrl(string url) => this.client.native.friends.ActivateGameOverlayToWebPage(url);
  }
}
