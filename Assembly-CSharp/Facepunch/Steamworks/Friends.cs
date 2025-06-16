// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Friends
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Friends
  {
    internal Client client;
    private List<SteamFriend> _allFriends;
    private List<Friends.PersonaCallback> PersonaCallbacks = new List<Friends.PersonaCallback>();

    internal Friends(Client c)
    {
      this.client = c;
      PersonaStateChange_t.RegisterCallback((BaseSteamworks) this.client, new Action<PersonaStateChange_t, bool>(this.OnPersonaStateChange));
    }

    public bool UpdateInformation(ulong steamid)
    {
      return !this.client.native.friends.RequestUserInformation((CSteamID) steamid, false);
    }

    public string GetName(ulong steamid)
    {
      this.client.native.friends.RequestUserInformation((CSteamID) steamid, true);
      return this.client.native.friends.GetFriendPersonaName((CSteamID) steamid);
    }

    public IEnumerable<SteamFriend> All
    {
      get
      {
        if (this._allFriends == null)
        {
          this._allFriends = new List<SteamFriend>();
          this.Refresh();
        }
        return (IEnumerable<SteamFriend>) this._allFriends;
      }
    }

    public IEnumerable<SteamFriend> AllFriends
    {
      get
      {
        foreach (SteamFriend steamFriend in this.All)
        {
          SteamFriend friend = steamFriend;
          if (friend.IsFriend)
          {
            yield return friend;
            friend = (SteamFriend) null;
          }
        }
      }
    }

    public IEnumerable<SteamFriend> AllBlocked
    {
      get
      {
        foreach (SteamFriend steamFriend in this.All)
        {
          SteamFriend friend = steamFriend;
          if (friend.IsBlocked)
          {
            yield return friend;
            friend = (SteamFriend) null;
          }
        }
      }
    }

    public void Refresh()
    {
      if (this._allFriends == null)
        this._allFriends = new List<SteamFriend>();
      this._allFriends.Clear();
      int maxValue = (int) ushort.MaxValue;
      int friendCount = this.client.native.friends.GetFriendCount(maxValue);
      for (int iFriend = 0; iFriend < friendCount; ++iFriend)
        this._allFriends.Add(this.Get(this.client.native.friends.GetFriendByIndex(iFriend, maxValue)));
    }

    public Image GetCachedAvatar(Friends.AvatarSize size, ulong steamid)
    {
      int num = 0;
      switch (size)
      {
        case Friends.AvatarSize.Small:
          num = this.client.native.friends.GetSmallFriendAvatar((CSteamID) steamid);
          break;
        case Friends.AvatarSize.Medium:
          num = this.client.native.friends.GetMediumFriendAvatar((CSteamID) steamid);
          break;
        case Friends.AvatarSize.Large:
          num = this.client.native.friends.GetLargeFriendAvatar((CSteamID) steamid);
          break;
      }
      Image image = new Image() { Id = num };
      return num != 0 && image.TryLoad(this.client.native.utils) ? image : (Image) null;
    }

    public void GetAvatar(Friends.AvatarSize size, ulong steamid, Action<Image> callback)
    {
      Image cachedAvatar = this.GetCachedAvatar(size, steamid);
      if (cachedAvatar != null)
        callback(cachedAvatar);
      else if (!this.client.native.friends.RequestUserInformation((CSteamID) steamid, false))
        callback((Image) null);
      else
        this.PersonaCallbacks.Add(new Friends.PersonaCallback()
        {
          SteamId = steamid,
          Callback = (Action) (() => callback(this.GetCachedAvatar(size, steamid)))
        });
    }

    public SteamFriend Get(ulong steamid)
    {
      SteamFriend steamFriend = new SteamFriend()
      {
        Id = steamid,
        Client = this.client
      };
      steamFriend.Refresh();
      return steamFriend;
    }

    private void OnPersonaStateChange(PersonaStateChange_t data, bool error)
    {
    }

    public enum AvatarSize
    {
      Small,
      Medium,
      Large,
    }

    private class PersonaCallback
    {
      public ulong SteamId;
      public Action Callback;
    }
  }
}
