using System;
using System.Collections.Generic;
using System.Text;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class Lobby : IDisposable
  {
    internal Client client;
    public Action<bool> OnLobbyJoined;
    internal Type createdLobbyType;
    public Action<bool> OnLobbyCreated;
    public Action OnLobbyDataUpdated;
    public Action<ulong> OnLobbyMemberDataUpdated;
    private static byte[] chatMessageData = new byte[4096];
    public Action<ulong, byte[], int> OnChatMessageRecieved;
    public Action<ulong, string> OnChatStringRecieved;
    public Action<MemberStateChange, ulong, ulong> OnLobbyStateChanged;
    public Action<ulong, ulong> OnUserInvitedToLobby;

    public Lobby(Client c)
    {
      client = c;
      LobbyDataUpdate_t.RegisterCallback(client, OnLobbyDataUpdatedAPI);
      LobbyChatMsg_t.RegisterCallback(client, OnLobbyChatMessageRecievedAPI);
      LobbyChatUpdate_t.RegisterCallback(client, OnLobbyStateUpdatedAPI);
      GameLobbyJoinRequested_t.RegisterCallback(client, OnLobbyJoinRequestedAPI);
      LobbyInvite_t.RegisterCallback(client, OnUserInvitedToLobbyAPI);
      PersonaStateChange_t.RegisterCallback(client, OnLobbyMemberPersonaChangeAPI);
    }

    public ulong CurrentLobby { get; private set; }

    public LobbyData CurrentLobbyData { get; private set; }

    public bool IsValid => CurrentLobby > 0UL;

    public void Join(ulong lobbyID)
    {
      Leave();
      client.native.matchmaking.JoinLobby(lobbyID, OnLobbyJoinedAPI);
    }

    private void OnLobbyJoinedAPI(LobbyEnter_t callback, bool error)
    {
      if (error || callback.EChatRoomEnterResponse != 1U)
      {
        if (OnLobbyJoined == null)
          return;
        OnLobbyJoined(false);
      }
      else
      {
        CurrentLobby = callback.SteamIDLobby;
        UpdateLobbyData();
        if (OnLobbyJoined == null)
          return;
        OnLobbyJoined(true);
      }
    }

    public void Create(Type lobbyType, int maxMembers)
    {
      client.native.matchmaking.CreateLobby((LobbyType) lobbyType, maxMembers, OnLobbyCreatedAPI);
      createdLobbyType = lobbyType;
    }

    internal void OnLobbyCreatedAPI(LobbyCreated_t callback, bool error)
    {
      if (error || callback.Result != Result.OK)
      {
        if (OnLobbyCreated == null)
          return;
        OnLobbyCreated(false);
      }
      else
      {
        Owner = client.SteamId;
        CurrentLobby = callback.SteamIDLobby;
        CurrentLobbyData = new LobbyData(client, CurrentLobby);
        Name = client.Username + "'s Lobby";
        CurrentLobbyData.SetData("appid", client.AppId.ToString());
        LobbyType = createdLobbyType;
        CurrentLobbyData.SetData("lobbytype", LobbyType.ToString());
        Joinable = true;
        if (OnLobbyCreated == null)
          return;
        OnLobbyCreated(true);
      }
    }

    public void SetMemberData(string key, string value)
    {
      if (CurrentLobby == 0UL)
        return;
      client.native.matchmaking.SetLobbyMemberData(CurrentLobby, key, value);
    }

    public string GetMemberData(ulong steamID, string key)
    {
      return CurrentLobby == 0UL ? "ERROR: NOT IN ANY LOBBY" : client.native.matchmaking.GetLobbyMemberData(CurrentLobby, steamID, key);
    }

    internal void OnLobbyDataUpdatedAPI(LobbyDataUpdate_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) CurrentLobby)
        return;
      if ((long) callback.SteamIDLobby == (long) CurrentLobby)
        UpdateLobbyData();
      if (!UserIsInCurrentLobby(callback.SteamIDMember) || OnLobbyMemberDataUpdated == null)
        return;
      OnLobbyMemberDataUpdated(callback.SteamIDMember);
    }

    internal void UpdateLobbyData()
    {
      int lobbyDataCount = client.native.matchmaking.GetLobbyDataCount(CurrentLobby);
      CurrentLobbyData = new LobbyData(client, CurrentLobby);
      for (int iLobbyData = 0; iLobbyData < lobbyDataCount; ++iLobbyData)
      {
        if (client.native.matchmaking.GetLobbyDataByIndex(CurrentLobby, iLobbyData, out string pchKey, out string pchValue))
          CurrentLobbyData.SetData(pchKey, pchValue);
      }
      if (OnLobbyDataUpdated == null)
        return;
      OnLobbyDataUpdated();
    }

    public Type LobbyType
    {
      get
      {
        if (!IsValid)
          return Type.Error;
        switch (CurrentLobbyData.GetData("lobbytype"))
        {
          case "Private":
            return Type.Private;
          case "FriendsOnly":
            return Type.FriendsOnly;
          case "Invisible":
            return Type.Invisible;
          case "Public":
            return Type.Public;
          default:
            return Type.Error;
        }
      }
      set
      {
        if (!IsValid || !client.native.matchmaking.SetLobbyType(CurrentLobby, (LobbyType) value))
          return;
        CurrentLobbyData.SetData("lobbytype", value.ToString());
      }
    }

    private unsafe void OnLobbyChatMessageRecievedAPI(LobbyChatMsg_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) CurrentLobby)
        return;
      CSteamID pSteamIDUser = 1UL;
      int lobbyChatEntry;
      fixed (byte* pvData = chatMessageData)
        lobbyChatEntry = client.native.matchmaking.GetLobbyChatEntry(CurrentLobby, (int) callback.ChatID, out pSteamIDUser, (IntPtr) pvData, chatMessageData.Length, out ChatEntryType _);
      Action<ulong, byte[], int> chatMessageRecieved = OnChatMessageRecieved;
      if (chatMessageRecieved != null)
        chatMessageRecieved(pSteamIDUser, chatMessageData, lobbyChatEntry);
      Action<ulong, string> chatStringRecieved = OnChatStringRecieved;
      if (chatStringRecieved == null)
        return;
      chatStringRecieved(pSteamIDUser, Encoding.UTF8.GetString(chatMessageData));
    }

    public unsafe bool SendChatMessage(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      fixed (byte* pvMsgBody = bytes)
        return client.native.matchmaking.SendLobbyChatMsg(CurrentLobby, (IntPtr) pvMsgBody, bytes.Length);
    }

    internal void OnLobbyStateUpdatedAPI(LobbyChatUpdate_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) CurrentLobby)
        return;
      MemberStateChange memberStateChange = (MemberStateChange) callback.GfChatMemberStateChange;
      ulong steamIdMakingChange = callback.SteamIDMakingChange;
      ulong steamIdUserChanged = callback.SteamIDUserChanged;
      Action<MemberStateChange, ulong, ulong> lobbyStateChanged = OnLobbyStateChanged;
      if (lobbyStateChanged == null)
        return;
      lobbyStateChanged(memberStateChange, steamIdMakingChange, steamIdUserChanged);
    }

    public string Name
    {
      get => !IsValid ? "" : CurrentLobbyData.GetData("name");
      set
      {
        if (!IsValid)
          return;
        CurrentLobbyData.SetData("name", value);
      }
    }

    public ulong Owner
    {
      get => IsValid ? client.native.matchmaking.GetLobbyOwner(CurrentLobby) : 0UL;
      set
      {
        if ((long) Owner == (long) value)
          return;
        client.native.matchmaking.SetLobbyOwner(CurrentLobby, value);
      }
    }

    public bool Joinable
    {
      get
      {
        if (!IsValid)
          return false;
        switch (CurrentLobbyData.GetData("joinable"))
        {
          case "true":
            return true;
          case "false":
            return false;
          default:
            return false;
        }
      }
      set
      {
        if (!IsValid || !client.native.matchmaking.SetLobbyJoinable(CurrentLobby, value))
          return;
        CurrentLobbyData.SetData("joinable", value.ToString());
      }
    }

    public int MaxMembers
    {
      get => !IsValid ? 0 : client.native.matchmaking.GetLobbyMemberLimit(CurrentLobby);
      set
      {
        if (!IsValid)
          return;
        client.native.matchmaking.SetLobbyMemberLimit(CurrentLobby, value);
      }
    }

    public int NumMembers => client.native.matchmaking.GetNumLobbyMembers(CurrentLobby);

    public void Leave()
    {
      if (CurrentLobby > 0UL)
        client.native.matchmaking.LeaveLobby(CurrentLobby);
      CurrentLobby = 0UL;
      CurrentLobbyData = null;
    }

    public void Dispose() => client = null;

    public ulong[] GetMemberIDs()
    {
      ulong[] memberIds = new ulong[NumMembers];
      for (int iMember = 0; iMember < NumMembers; ++iMember)
        memberIds[iMember] = client.native.matchmaking.GetLobbyMemberByIndex(CurrentLobby, iMember);
      return memberIds;
    }

    public bool UserIsInCurrentLobby(ulong steamID)
    {
      if (CurrentLobby == 0UL)
        return false;
      foreach (long memberId in GetMemberIDs())
      {
        if (memberId == (long) steamID)
          return true;
      }
      return false;
    }

    public bool InviteUserToLobby(ulong friendID)
    {
      return client.native.matchmaking.InviteUserToLobby(CurrentLobby, friendID);
    }

    internal void OnUserInvitedToLobbyAPI(LobbyInvite_t callback, bool error)
    {
      if (error || (long) callback.GameID != client.AppId || OnUserInvitedToLobby == null)
        return;
      OnUserInvitedToLobby(callback.SteamIDLobby, callback.SteamIDUser);
    }

    internal void OnLobbyJoinRequestedAPI(GameLobbyJoinRequested_t callback, bool error)
    {
      if (error)
        return;
      Join(callback.SteamIDLobby);
    }

    internal void OnLobbyMemberPersonaChangeAPI(PersonaStateChange_t callback, bool error)
    {
      if (error || !UserIsInCurrentLobby(callback.SteamID) || OnLobbyMemberDataUpdated == null)
        return;
      OnLobbyMemberDataUpdated(callback.SteamID);
    }

    public enum Type
    {
      Private,
      FriendsOnly,
      Public,
      Invisible,
      Error,
    }

    public class LobbyData(Client c, ulong l) {
      internal Client client = c;
      internal ulong lobby = l;
      internal Dictionary<string, string> data = new();

      public string GetData(string k)
      {
        return data.ContainsKey(k) ? data[k] : "ERROR: key not found";
      }

      public Dictionary<string, string> GetAllData()
      {
        Dictionary<string, string> allData = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> keyValuePair in data)
          allData.Add(keyValuePair.Key, keyValuePair.Value);
        return allData;
      }

      public bool SetData(string k, string v)
      {
        if (data.ContainsKey(k))
        {
          if (data[k] == v)
            return true;
          if (client.native.matchmaking.SetLobbyData(lobby, k, v))
          {
            data[k] = v;
            return true;
          }
        }
        else if (client.native.matchmaking.SetLobbyData(lobby, k, v))
        {
          data.Add(k, v);
          return true;
        }
        return false;
      }

      public bool RemoveData(string k)
      {
        if (!data.ContainsKey(k) || !client.native.matchmaking.DeleteLobbyData(lobby, k))
          return false;
        data.Remove(k);
        return true;
      }
    }

    public enum MemberStateChange
    {
      Entered = 1,
      Left = 2,
      Disconnected = 4,
      Kicked = 8,
      Banned = 16,
    }
  }
}
