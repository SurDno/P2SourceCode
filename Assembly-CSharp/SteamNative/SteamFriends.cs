﻿// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamFriends
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  internal class SteamFriends : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamFriends(BaseSteamworks steamworks, IntPtr pointer)
    {
      this.steamworks = steamworks;
      if (Platform.IsWindows64)
        this.platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        this.platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        this.platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        this.platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        this.platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => this.platform != null && this.platform.IsValid;

    public virtual void Dispose()
    {
      if (this.platform == null)
        return;
      this.platform.Dispose();
      this.platform = (Platform.Interface) null;
    }

    public void ActivateGameOverlay(string pchDialog)
    {
      this.platform.ISteamFriends_ActivateGameOverlay(pchDialog);
    }

    public void ActivateGameOverlayInviteDialog(CSteamID steamIDLobby)
    {
      this.platform.ISteamFriends_ActivateGameOverlayInviteDialog(steamIDLobby.Value);
    }

    public void ActivateGameOverlayToStore(AppId_t nAppID, OverlayToStoreFlag eFlag)
    {
      this.platform.ISteamFriends_ActivateGameOverlayToStore(nAppID.Value, eFlag);
    }

    public void ActivateGameOverlayToUser(string pchDialog, CSteamID steamID)
    {
      this.platform.ISteamFriends_ActivateGameOverlayToUser(pchDialog, steamID.Value);
    }

    public void ActivateGameOverlayToWebPage(string pchURL)
    {
      this.platform.ISteamFriends_ActivateGameOverlayToWebPage(pchURL);
    }

    public void ClearRichPresence() => this.platform.ISteamFriends_ClearRichPresence();

    public bool CloseClanChatWindowInSteam(CSteamID steamIDClanChat)
    {
      return this.platform.ISteamFriends_CloseClanChatWindowInSteam(steamIDClanChat.Value);
    }

    public SteamAPICall_t DownloadClanActivityCounts(IntPtr psteamIDClans, int cClansToRequest)
    {
      return this.platform.ISteamFriends_DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
    }

    public CallbackHandle EnumerateFollowingList(
      uint unStartIndex,
      Action<FriendsEnumerateFollowingList_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamFriends_EnumerateFollowingList(unStartIndex);
      return CallbackFunction == null ? (CallbackHandle) null : FriendsEnumerateFollowingList_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public ulong GetChatMemberByIndex(CSteamID steamIDClan, int iUser)
    {
      return (ulong) this.platform.ISteamFriends_GetChatMemberByIndex(steamIDClan.Value, iUser);
    }

    public bool GetClanActivityCounts(
      CSteamID steamIDClan,
      out int pnOnline,
      out int pnInGame,
      out int pnChatting)
    {
      return this.platform.ISteamFriends_GetClanActivityCounts(steamIDClan.Value, out pnOnline, out pnInGame, out pnChatting);
    }

    public ulong GetClanByIndex(int iClan)
    {
      return (ulong) this.platform.ISteamFriends_GetClanByIndex(iClan);
    }

    public int GetClanChatMemberCount(CSteamID steamIDClan)
    {
      return this.platform.ISteamFriends_GetClanChatMemberCount(steamIDClan.Value);
    }

    public int GetClanChatMessage(
      CSteamID steamIDClanChat,
      int iMessage,
      IntPtr prgchText,
      int cchTextMax,
      out ChatEntryType peChatEntryType,
      out CSteamID psteamidChatter)
    {
      return this.platform.ISteamFriends_GetClanChatMessage(steamIDClanChat.Value, iMessage, prgchText, cchTextMax, out peChatEntryType, out psteamidChatter.Value);
    }

    public int GetClanCount() => this.platform.ISteamFriends_GetClanCount();

    public string GetClanName(CSteamID steamIDClan)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetClanName(steamIDClan.Value));
    }

    public ulong GetClanOfficerByIndex(CSteamID steamIDClan, int iOfficer)
    {
      return (ulong) this.platform.ISteamFriends_GetClanOfficerByIndex(steamIDClan.Value, iOfficer);
    }

    public int GetClanOfficerCount(CSteamID steamIDClan)
    {
      return this.platform.ISteamFriends_GetClanOfficerCount(steamIDClan.Value);
    }

    public ulong GetClanOwner(CSteamID steamIDClan)
    {
      return (ulong) this.platform.ISteamFriends_GetClanOwner(steamIDClan.Value);
    }

    public string GetClanTag(CSteamID steamIDClan)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetClanTag(steamIDClan.Value));
    }

    public ulong GetCoplayFriend(int iCoplayFriend)
    {
      return (ulong) this.platform.ISteamFriends_GetCoplayFriend(iCoplayFriend);
    }

    public int GetCoplayFriendCount() => this.platform.ISteamFriends_GetCoplayFriendCount();

    public CallbackHandle GetFollowerCount(
      CSteamID steamID,
      Action<FriendsGetFollowerCount_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t followerCount = this.platform.ISteamFriends_GetFollowerCount(steamID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : FriendsGetFollowerCount_t.CallResult(this.steamworks, followerCount, CallbackFunction);
    }

    public ulong GetFriendByIndex(int iFriend, int iFriendFlags)
    {
      return (ulong) this.platform.ISteamFriends_GetFriendByIndex(iFriend, iFriendFlags);
    }

    public AppId_t GetFriendCoplayGame(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendCoplayGame(steamIDFriend.Value);
    }

    public int GetFriendCoplayTime(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendCoplayTime(steamIDFriend.Value);
    }

    public int GetFriendCount(int iFriendFlags)
    {
      return this.platform.ISteamFriends_GetFriendCount(iFriendFlags);
    }

    public int GetFriendCountFromSource(CSteamID steamIDSource)
    {
      return this.platform.ISteamFriends_GetFriendCountFromSource(steamIDSource.Value);
    }

    public ulong GetFriendFromSourceByIndex(CSteamID steamIDSource, int iFriend)
    {
      return (ulong) this.platform.ISteamFriends_GetFriendFromSourceByIndex(steamIDSource.Value, iFriend);
    }

    public bool GetFriendGamePlayed(CSteamID steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
    {
      return this.platform.ISteamFriends_GetFriendGamePlayed(steamIDFriend.Value, ref pFriendGameInfo);
    }

    public int GetFriendMessage(
      CSteamID steamIDFriend,
      int iMessageID,
      IntPtr pvData,
      int cubData,
      out ChatEntryType peChatEntryType)
    {
      return this.platform.ISteamFriends_GetFriendMessage(steamIDFriend.Value, iMessageID, pvData, cubData, out peChatEntryType);
    }

    public string GetFriendPersonaName(CSteamID steamIDFriend)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetFriendPersonaName(steamIDFriend.Value));
    }

    public string GetFriendPersonaNameHistory(CSteamID steamIDFriend, int iPersonaName)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetFriendPersonaNameHistory(steamIDFriend.Value, iPersonaName));
    }

    public PersonaState GetFriendPersonaState(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendPersonaState(steamIDFriend.Value);
    }

    public FriendRelationship GetFriendRelationship(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendRelationship(steamIDFriend.Value);
    }

    public string GetFriendRichPresence(CSteamID steamIDFriend, string pchKey)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetFriendRichPresence(steamIDFriend.Value, pchKey));
    }

    public string GetFriendRichPresenceKeyByIndex(CSteamID steamIDFriend, int iKey)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetFriendRichPresenceKeyByIndex(steamIDFriend.Value, iKey));
    }

    public int GetFriendRichPresenceKeyCount(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendRichPresenceKeyCount(steamIDFriend.Value);
    }

    public int GetFriendsGroupCount() => this.platform.ISteamFriends_GetFriendsGroupCount();

    public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
    {
      return this.platform.ISteamFriends_GetFriendsGroupIDByIndex(iFG);
    }

    public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
    {
      return this.platform.ISteamFriends_GetFriendsGroupMembersCount(friendsGroupID.Value);
    }

    public void GetFriendsGroupMembersList(
      FriendsGroupID_t friendsGroupID,
      IntPtr pOutSteamIDMembers,
      int nMembersCount)
    {
      this.platform.ISteamFriends_GetFriendsGroupMembersList(friendsGroupID.Value, pOutSteamIDMembers, nMembersCount);
    }

    public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetFriendsGroupName(friendsGroupID.Value));
    }

    public int GetFriendSteamLevel(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetFriendSteamLevel(steamIDFriend.Value);
    }

    public int GetLargeFriendAvatar(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetLargeFriendAvatar(steamIDFriend.Value);
    }

    public int GetMediumFriendAvatar(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetMediumFriendAvatar(steamIDFriend.Value);
    }

    public string GetPersonaName()
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetPersonaName());
    }

    public PersonaState GetPersonaState() => this.platform.ISteamFriends_GetPersonaState();

    public string GetPlayerNickname(CSteamID steamIDPlayer)
    {
      return Marshal.PtrToStringAnsi(this.platform.ISteamFriends_GetPlayerNickname(steamIDPlayer.Value));
    }

    public int GetSmallFriendAvatar(CSteamID steamIDFriend)
    {
      return this.platform.ISteamFriends_GetSmallFriendAvatar(steamIDFriend.Value);
    }

    public uint GetUserRestrictions() => this.platform.ISteamFriends_GetUserRestrictions();

    public bool HasFriend(CSteamID steamIDFriend, int iFriendFlags)
    {
      return this.platform.ISteamFriends_HasFriend(steamIDFriend.Value, iFriendFlags);
    }

    public bool InviteUserToGame(CSteamID steamIDFriend, string pchConnectString)
    {
      return this.platform.ISteamFriends_InviteUserToGame(steamIDFriend.Value, pchConnectString);
    }

    public bool IsClanChatAdmin(CSteamID steamIDClanChat, CSteamID steamIDUser)
    {
      return this.platform.ISteamFriends_IsClanChatAdmin(steamIDClanChat.Value, steamIDUser.Value);
    }

    public bool IsClanChatWindowOpenInSteam(CSteamID steamIDClanChat)
    {
      return this.platform.ISteamFriends_IsClanChatWindowOpenInSteam(steamIDClanChat.Value);
    }

    public CallbackHandle IsFollowing(
      CSteamID steamID,
      Action<FriendsIsFollowing_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamFriends_IsFollowing(steamID.Value);
      return CallbackFunction == null ? (CallbackHandle) null : FriendsIsFollowing_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool IsUserInSource(CSteamID steamIDUser, CSteamID steamIDSource)
    {
      return this.platform.ISteamFriends_IsUserInSource(steamIDUser.Value, steamIDSource.Value);
    }

    public CallbackHandle JoinClanChatRoom(
      CSteamID steamIDClan,
      Action<JoinClanChatRoomCompletionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamFriends_JoinClanChatRoom(steamIDClan.Value);
      return CallbackFunction == null ? (CallbackHandle) null : JoinClanChatRoomCompletionResult_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public bool LeaveClanChatRoom(CSteamID steamIDClan)
    {
      return this.platform.ISteamFriends_LeaveClanChatRoom(steamIDClan.Value);
    }

    public bool OpenClanChatWindowInSteam(CSteamID steamIDClanChat)
    {
      return this.platform.ISteamFriends_OpenClanChatWindowInSteam(steamIDClanChat.Value);
    }

    public bool ReplyToFriendMessage(CSteamID steamIDFriend, string pchMsgToSend)
    {
      return this.platform.ISteamFriends_ReplyToFriendMessage(steamIDFriend.Value, pchMsgToSend);
    }

    public CallbackHandle RequestClanOfficerList(
      CSteamID steamIDClan,
      Action<ClanOfficerListResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamFriends_RequestClanOfficerList(steamIDClan.Value);
      return CallbackFunction == null ? (CallbackHandle) null : ClanOfficerListResponse_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void RequestFriendRichPresence(CSteamID steamIDFriend)
    {
      this.platform.ISteamFriends_RequestFriendRichPresence(steamIDFriend.Value);
    }

    public bool RequestUserInformation(CSteamID steamIDUser, bool bRequireNameOnly)
    {
      return this.platform.ISteamFriends_RequestUserInformation(steamIDUser.Value, bRequireNameOnly);
    }

    public bool SendClanChatMessage(CSteamID steamIDClanChat, string pchText)
    {
      return this.platform.ISteamFriends_SendClanChatMessage(steamIDClanChat.Value, pchText);
    }

    public void SetInGameVoiceSpeaking(CSteamID steamIDUser, bool bSpeaking)
    {
      this.platform.ISteamFriends_SetInGameVoiceSpeaking(steamIDUser.Value, bSpeaking);
    }

    public bool SetListenForFriendsMessages(bool bInterceptEnabled)
    {
      return this.platform.ISteamFriends_SetListenForFriendsMessages(bInterceptEnabled);
    }

    public CallbackHandle SetPersonaName(
      string pchPersonaName,
      Action<SetPersonaNameResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = (SteamAPICall_t) 0UL;
      SteamAPICall_t call = this.platform.ISteamFriends_SetPersonaName(pchPersonaName);
      return CallbackFunction == null ? (CallbackHandle) null : SetPersonaNameResponse_t.CallResult(this.steamworks, call, CallbackFunction);
    }

    public void SetPlayedWith(CSteamID steamIDUserPlayedWith)
    {
      this.platform.ISteamFriends_SetPlayedWith(steamIDUserPlayedWith.Value);
    }

    public bool SetRichPresence(string pchKey, string pchValue)
    {
      return this.platform.ISteamFriends_SetRichPresence(pchKey, pchValue);
    }
  }
}
