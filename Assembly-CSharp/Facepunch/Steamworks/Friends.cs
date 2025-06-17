using System;
using System.Collections.Generic;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class Friends
  {
    internal Client client;
    private List<SteamFriend> _allFriends;
    private List<PersonaCallback> PersonaCallbacks = [];

    internal Friends(Client c)
    {
      client = c;
      PersonaStateChange_t.RegisterCallback(client, OnPersonaStateChange);
    }

    public bool UpdateInformation(ulong steamid)
    {
      return !client.native.friends.RequestUserInformation(steamid, false);
    }

    public string GetName(ulong steamid)
    {
      client.native.friends.RequestUserInformation(steamid, true);
      return client.native.friends.GetFriendPersonaName(steamid);
    }

    public IEnumerable<SteamFriend> All
    {
      get
      {
        if (_allFriends == null)
        {
          _allFriends = [];
          Refresh();
        }
        return _allFriends;
      }
    }

    public IEnumerable<SteamFriend> AllFriends
    {
      get
      {
        foreach (SteamFriend steamFriend in All)
        {
          SteamFriend friend = steamFriend;
          if (friend.IsFriend)
          {
            yield return friend;
            friend = null;
          }
        }
      }
    }

    public IEnumerable<SteamFriend> AllBlocked
    {
      get
      {
        foreach (SteamFriend steamFriend in All)
        {
          SteamFriend friend = steamFriend;
          if (friend.IsBlocked)
          {
            yield return friend;
            friend = null;
          }
        }
      }
    }

    public void Refresh()
    {
      if (_allFriends == null)
        _allFriends = [];
      _allFriends.Clear();
      int maxValue = ushort.MaxValue;
      int friendCount = client.native.friends.GetFriendCount(maxValue);
      for (int iFriend = 0; iFriend < friendCount; ++iFriend)
        _allFriends.Add(Get(client.native.friends.GetFriendByIndex(iFriend, maxValue)));
    }

    public Image GetCachedAvatar(AvatarSize size, ulong steamid)
    {
      int num = 0;
      switch (size)
      {
        case AvatarSize.Small:
          num = client.native.friends.GetSmallFriendAvatar(steamid);
          break;
        case AvatarSize.Medium:
          num = client.native.friends.GetMediumFriendAvatar(steamid);
          break;
        case AvatarSize.Large:
          num = client.native.friends.GetLargeFriendAvatar(steamid);
          break;
      }
      Image image = new Image { Id = num };
      return num != 0 && image.TryLoad(client.native.utils) ? image : null;
    }

    public void GetAvatar(AvatarSize size, ulong steamid, Action<Image> callback)
    {
      Image cachedAvatar = GetCachedAvatar(size, steamid);
      if (cachedAvatar != null)
        callback(cachedAvatar);
      else if (!client.native.friends.RequestUserInformation(steamid, false))
        callback(null);
      else
        PersonaCallbacks.Add(new PersonaCallback {
          SteamId = steamid,
          Callback = () => callback(GetCachedAvatar(size, steamid))
        });
    }

    public SteamFriend Get(ulong steamid)
    {
      SteamFriend steamFriend = new SteamFriend {
        Id = steamid,
        Client = client
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
