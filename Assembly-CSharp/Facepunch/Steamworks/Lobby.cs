// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Lobby
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Lobby : IDisposable
  {
    internal Client client;
    public Action<bool> OnLobbyJoined;
    internal Lobby.Type createdLobbyType;
    public Action<bool> OnLobbyCreated;
    public Action OnLobbyDataUpdated;
    public Action<ulong> OnLobbyMemberDataUpdated;
    private static byte[] chatMessageData = new byte[4096];
    public Action<ulong, byte[], int> OnChatMessageRecieved;
    public Action<ulong, string> OnChatStringRecieved;
    public Action<Lobby.MemberStateChange, ulong, ulong> OnLobbyStateChanged;
    public Action<ulong, ulong> OnUserInvitedToLobby;

    public Lobby(Client c)
    {
      this.client = c;
      LobbyDataUpdate_t.RegisterCallback((BaseSteamworks) this.client, new Action<LobbyDataUpdate_t, bool>(this.OnLobbyDataUpdatedAPI));
      LobbyChatMsg_t.RegisterCallback((BaseSteamworks) this.client, new Action<LobbyChatMsg_t, bool>(this.OnLobbyChatMessageRecievedAPI));
      LobbyChatUpdate_t.RegisterCallback((BaseSteamworks) this.client, new Action<LobbyChatUpdate_t, bool>(this.OnLobbyStateUpdatedAPI));
      GameLobbyJoinRequested_t.RegisterCallback((BaseSteamworks) this.client, new Action<GameLobbyJoinRequested_t, bool>(this.OnLobbyJoinRequestedAPI));
      LobbyInvite_t.RegisterCallback((BaseSteamworks) this.client, new Action<LobbyInvite_t, bool>(this.OnUserInvitedToLobbyAPI));
      PersonaStateChange_t.RegisterCallback((BaseSteamworks) this.client, new Action<PersonaStateChange_t, bool>(this.OnLobbyMemberPersonaChangeAPI));
    }

    public ulong CurrentLobby { get; private set; }

    public Lobby.LobbyData CurrentLobbyData { get; private set; }

    public bool IsValid => this.CurrentLobby > 0UL;

    public void Join(ulong lobbyID)
    {
      this.Leave();
      this.client.native.matchmaking.JoinLobby((CSteamID) lobbyID, new Action<LobbyEnter_t, bool>(this.OnLobbyJoinedAPI));
    }

    private void OnLobbyJoinedAPI(LobbyEnter_t callback, bool error)
    {
      if (error || callback.EChatRoomEnterResponse != 1U)
      {
        if (this.OnLobbyJoined == null)
          return;
        this.OnLobbyJoined(false);
      }
      else
      {
        this.CurrentLobby = callback.SteamIDLobby;
        this.UpdateLobbyData();
        if (this.OnLobbyJoined == null)
          return;
        this.OnLobbyJoined(true);
      }
    }

    public void Create(Lobby.Type lobbyType, int maxMembers)
    {
      this.client.native.matchmaking.CreateLobby((SteamNative.LobbyType) lobbyType, maxMembers, new Action<LobbyCreated_t, bool>(this.OnLobbyCreatedAPI));
      this.createdLobbyType = lobbyType;
    }

    internal void OnLobbyCreatedAPI(LobbyCreated_t callback, bool error)
    {
      if (error || callback.Result != SteamNative.Result.OK)
      {
        if (this.OnLobbyCreated == null)
          return;
        this.OnLobbyCreated(false);
      }
      else
      {
        this.Owner = this.client.SteamId;
        this.CurrentLobby = callback.SteamIDLobby;
        this.CurrentLobbyData = new Lobby.LobbyData(this.client, this.CurrentLobby);
        this.Name = this.client.Username + "'s Lobby";
        this.CurrentLobbyData.SetData("appid", this.client.AppId.ToString());
        this.LobbyType = this.createdLobbyType;
        this.CurrentLobbyData.SetData("lobbytype", this.LobbyType.ToString());
        this.Joinable = true;
        if (this.OnLobbyCreated == null)
          return;
        this.OnLobbyCreated(true);
      }
    }

    public void SetMemberData(string key, string value)
    {
      if (this.CurrentLobby == 0UL)
        return;
      this.client.native.matchmaking.SetLobbyMemberData((CSteamID) this.CurrentLobby, key, value);
    }

    public string GetMemberData(ulong steamID, string key)
    {
      return this.CurrentLobby == 0UL ? "ERROR: NOT IN ANY LOBBY" : this.client.native.matchmaking.GetLobbyMemberData((CSteamID) this.CurrentLobby, (CSteamID) steamID, key);
    }

    internal void OnLobbyDataUpdatedAPI(LobbyDataUpdate_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) this.CurrentLobby)
        return;
      if ((long) callback.SteamIDLobby == (long) this.CurrentLobby)
        this.UpdateLobbyData();
      if (!this.UserIsInCurrentLobby(callback.SteamIDMember) || this.OnLobbyMemberDataUpdated == null)
        return;
      this.OnLobbyMemberDataUpdated(callback.SteamIDMember);
    }

    internal void UpdateLobbyData()
    {
      int lobbyDataCount = this.client.native.matchmaking.GetLobbyDataCount((CSteamID) this.CurrentLobby);
      this.CurrentLobbyData = new Lobby.LobbyData(this.client, this.CurrentLobby);
      for (int iLobbyData = 0; iLobbyData < lobbyDataCount; ++iLobbyData)
      {
        string pchKey;
        string pchValue;
        if (this.client.native.matchmaking.GetLobbyDataByIndex((CSteamID) this.CurrentLobby, iLobbyData, out pchKey, out pchValue))
          this.CurrentLobbyData.SetData(pchKey, pchValue);
      }
      if (this.OnLobbyDataUpdated == null)
        return;
      this.OnLobbyDataUpdated();
    }

    public Lobby.Type LobbyType
    {
      get
      {
        if (!this.IsValid)
          return Lobby.Type.Error;
        switch (this.CurrentLobbyData.GetData("lobbytype"))
        {
          case "Private":
            return Lobby.Type.Private;
          case "FriendsOnly":
            return Lobby.Type.FriendsOnly;
          case "Invisible":
            return Lobby.Type.Invisible;
          case "Public":
            return Lobby.Type.Public;
          default:
            return Lobby.Type.Error;
        }
      }
      set
      {
        if (!this.IsValid || !this.client.native.matchmaking.SetLobbyType((CSteamID) this.CurrentLobby, (SteamNative.LobbyType) value))
          return;
        this.CurrentLobbyData.SetData("lobbytype", value.ToString());
      }
    }

    private unsafe void OnLobbyChatMessageRecievedAPI(LobbyChatMsg_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) this.CurrentLobby)
        return;
      CSteamID pSteamIDUser = (CSteamID) 1UL;
      int lobbyChatEntry;
      fixed (byte* pvData = Lobby.chatMessageData)
        lobbyChatEntry = this.client.native.matchmaking.GetLobbyChatEntry((CSteamID) this.CurrentLobby, (int) callback.ChatID, out pSteamIDUser, (IntPtr) (void*) pvData, Lobby.chatMessageData.Length, out ChatEntryType _);
      Action<ulong, byte[], int> chatMessageRecieved = this.OnChatMessageRecieved;
      if (chatMessageRecieved != null)
        chatMessageRecieved((ulong) pSteamIDUser, Lobby.chatMessageData, lobbyChatEntry);
      Action<ulong, string> chatStringRecieved = this.OnChatStringRecieved;
      if (chatStringRecieved == null)
        return;
      chatStringRecieved((ulong) pSteamIDUser, Encoding.UTF8.GetString(Lobby.chatMessageData));
    }

    public unsafe bool SendChatMessage(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      fixed (byte* pvMsgBody = bytes)
        return this.client.native.matchmaking.SendLobbyChatMsg((CSteamID) this.CurrentLobby, (IntPtr) (void*) pvMsgBody, bytes.Length);
    }

    internal void OnLobbyStateUpdatedAPI(LobbyChatUpdate_t callback, bool error)
    {
      if (error || (long) callback.SteamIDLobby != (long) this.CurrentLobby)
        return;
      Lobby.MemberStateChange memberStateChange = (Lobby.MemberStateChange) callback.GfChatMemberStateChange;
      ulong steamIdMakingChange = callback.SteamIDMakingChange;
      ulong steamIdUserChanged = callback.SteamIDUserChanged;
      Action<Lobby.MemberStateChange, ulong, ulong> lobbyStateChanged = this.OnLobbyStateChanged;
      if (lobbyStateChanged == null)
        return;
      lobbyStateChanged(memberStateChange, steamIdMakingChange, steamIdUserChanged);
    }

    public string Name
    {
      get => !this.IsValid ? "" : this.CurrentLobbyData.GetData("name");
      set
      {
        if (!this.IsValid)
          return;
        this.CurrentLobbyData.SetData("name", value);
      }
    }

    public ulong Owner
    {
      get
      {
        return this.IsValid ? this.client.native.matchmaking.GetLobbyOwner((CSteamID) this.CurrentLobby) : 0UL;
      }
      set
      {
        if ((long) this.Owner == (long) value)
          return;
        this.client.native.matchmaking.SetLobbyOwner((CSteamID) this.CurrentLobby, (CSteamID) value);
      }
    }

    public bool Joinable
    {
      get
      {
        if (!this.IsValid)
          return false;
        switch (this.CurrentLobbyData.GetData("joinable"))
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
        if (!this.IsValid || !this.client.native.matchmaking.SetLobbyJoinable((CSteamID) this.CurrentLobby, value))
          return;
        this.CurrentLobbyData.SetData("joinable", value.ToString());
      }
    }

    public int MaxMembers
    {
      get
      {
        return !this.IsValid ? 0 : this.client.native.matchmaking.GetLobbyMemberLimit((CSteamID) this.CurrentLobby);
      }
      set
      {
        if (!this.IsValid)
          return;
        this.client.native.matchmaking.SetLobbyMemberLimit((CSteamID) this.CurrentLobby, value);
      }
    }

    public int NumMembers
    {
      get => this.client.native.matchmaking.GetNumLobbyMembers((CSteamID) this.CurrentLobby);
    }

    public void Leave()
    {
      if (this.CurrentLobby > 0UL)
        this.client.native.matchmaking.LeaveLobby((CSteamID) this.CurrentLobby);
      this.CurrentLobby = 0UL;
      this.CurrentLobbyData = (Lobby.LobbyData) null;
    }

    public void Dispose() => this.client = (Client) null;

    public ulong[] GetMemberIDs()
    {
      ulong[] memberIds = new ulong[this.NumMembers];
      for (int iMember = 0; iMember < this.NumMembers; ++iMember)
        memberIds[iMember] = this.client.native.matchmaking.GetLobbyMemberByIndex((CSteamID) this.CurrentLobby, iMember);
      return memberIds;
    }

    public bool UserIsInCurrentLobby(ulong steamID)
    {
      if (this.CurrentLobby == 0UL)
        return false;
      foreach (long memberId in this.GetMemberIDs())
      {
        if (memberId == (long) steamID)
          return true;
      }
      return false;
    }

    public bool InviteUserToLobby(ulong friendID)
    {
      return this.client.native.matchmaking.InviteUserToLobby((CSteamID) this.CurrentLobby, (CSteamID) friendID);
    }

    internal void OnUserInvitedToLobbyAPI(LobbyInvite_t callback, bool error)
    {
      if (error || (long) callback.GameID != (long) this.client.AppId || this.OnUserInvitedToLobby == null)
        return;
      this.OnUserInvitedToLobby(callback.SteamIDLobby, callback.SteamIDUser);
    }

    internal void OnLobbyJoinRequestedAPI(GameLobbyJoinRequested_t callback, bool error)
    {
      if (error)
        return;
      this.Join(callback.SteamIDLobby);
    }

    internal void OnLobbyMemberPersonaChangeAPI(PersonaStateChange_t callback, bool error)
    {
      if (error || !this.UserIsInCurrentLobby(callback.SteamID) || this.OnLobbyMemberDataUpdated == null)
        return;
      this.OnLobbyMemberDataUpdated(callback.SteamID);
    }

    public enum Type
    {
      Private,
      FriendsOnly,
      Public,
      Invisible,
      Error,
    }

    public class LobbyData
    {
      internal Client client;
      internal ulong lobby;
      internal Dictionary<string, string> data;

      public LobbyData(Client c, ulong l)
      {
        this.client = c;
        this.lobby = l;
        this.data = new Dictionary<string, string>();
      }

      public string GetData(string k)
      {
        return this.data.ContainsKey(k) ? this.data[k] : "ERROR: key not found";
      }

      public Dictionary<string, string> GetAllData()
      {
        Dictionary<string, string> allData = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> keyValuePair in this.data)
          allData.Add(keyValuePair.Key, keyValuePair.Value);
        return allData;
      }

      public bool SetData(string k, string v)
      {
        if (this.data.ContainsKey(k))
        {
          if (this.data[k] == v)
            return true;
          if (this.client.native.matchmaking.SetLobbyData((CSteamID) this.lobby, k, v))
          {
            this.data[k] = v;
            return true;
          }
        }
        else if (this.client.native.matchmaking.SetLobbyData((CSteamID) this.lobby, k, v))
        {
          this.data.Add(k, v);
          return true;
        }
        return false;
      }

      public bool RemoveData(string k)
      {
        if (!this.data.ContainsKey(k) || !this.client.native.matchmaking.DeleteLobbyData((CSteamID) this.lobby, k))
          return false;
        this.data.Remove(k);
        return true;
      }
    }

    public enum MemberStateChange
    {
      Entered = 1,
      Left = 2,
      Disconnected = 4,
      Kicked = 8,
      Banned = 16, // 0x00000010
    }
  }
}
