﻿using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

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
        platform = (Platform.Interface) new Platform.Win64(pointer);
      else if (Platform.IsWindows32)
        platform = (Platform.Interface) new Platform.Win32(pointer);
      else if (Platform.IsLinux32)
        platform = (Platform.Interface) new Platform.Linux32(pointer);
      else if (Platform.IsLinux64)
      {
        platform = (Platform.Interface) new Platform.Linux64(pointer);
      }
      else
      {
        if (!Platform.IsOsx)
          return;
        platform = (Platform.Interface) new Platform.Mac(pointer);
      }
    }

    public bool IsValid => platform != null && platform.IsValid;

    public virtual void Dispose()
    {
      if (platform == null)
        return;
      platform.Dispose();
      platform = (Platform.Interface) null;
    }

    public void ActivateGameOverlay(string pchDialog)
    {
      platform.ISteamFriends_ActivateGameOverlay(pchDialog);
    }

    public void ActivateGameOverlayInviteDialog(CSteamID steamIDLobby)
    {
      platform.ISteamFriends_ActivateGameOverlayInviteDialog(steamIDLobby.Value);
    }

    public void ActivateGameOverlayToStore(AppId_t nAppID, OverlayToStoreFlag eFlag)
    {
      platform.ISteamFriends_ActivateGameOverlayToStore(nAppID.Value, eFlag);
    }

    public void ActivateGameOverlayToUser(string pchDialog, CSteamID steamID)
    {
      platform.ISteamFriends_ActivateGameOverlayToUser(pchDialog, steamID.Value);
    }

    public void ActivateGameOverlayToWebPage(string pchURL)
    {
      platform.ISteamFriends_ActivateGameOverlayToWebPage(pchURL);
    }

    public void ClearRichPresence() => platform.ISteamFriends_ClearRichPresence();

    public bool CloseClanChatWindowInSteam(CSteamID steamIDClanChat)
    {
      return platform.ISteamFriends_CloseClanChatWindowInSteam(steamIDClanChat.Value);
    }

    public SteamAPICall_t DownloadClanActivityCounts(IntPtr psteamIDClans, int cClansToRequest)
    {
      return platform.ISteamFriends_DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
    }

    public CallbackHandle EnumerateFollowingList(
      uint unStartIndex,
      Action<FriendsEnumerateFollowingList_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamFriends_EnumerateFollowingList(unStartIndex);
      return CallbackFunction == null ? null : FriendsEnumerateFollowingList_t.CallResult(steamworks, call, CallbackFunction);
    }

    public ulong GetChatMemberByIndex(CSteamID steamIDClan, int iUser)
    {
      return (ulong) platform.ISteamFriends_GetChatMemberByIndex(steamIDClan.Value, iUser);
    }

    public bool GetClanActivityCounts(
      CSteamID steamIDClan,
      out int pnOnline,
      out int pnInGame,
      out int pnChatting)
    {
      return platform.ISteamFriends_GetClanActivityCounts(steamIDClan.Value, out pnOnline, out pnInGame, out pnChatting);
    }

    public ulong GetClanByIndex(int iClan)
    {
      return (ulong) platform.ISteamFriends_GetClanByIndex(iClan);
    }

    public int GetClanChatMemberCount(CSteamID steamIDClan)
    {
      return platform.ISteamFriends_GetClanChatMemberCount(steamIDClan.Value);
    }

    public int GetClanChatMessage(
      CSteamID steamIDClanChat,
      int iMessage,
      IntPtr prgchText,
      int cchTextMax,
      out ChatEntryType peChatEntryType,
      out CSteamID psteamidChatter)
    {
      return platform.ISteamFriends_GetClanChatMessage(steamIDClanChat.Value, iMessage, prgchText, cchTextMax, out peChatEntryType, out psteamidChatter.Value);
    }

    public int GetClanCount() => platform.ISteamFriends_GetClanCount();

    public string GetClanName(CSteamID steamIDClan)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetClanName(steamIDClan.Value));
    }

    public ulong GetClanOfficerByIndex(CSteamID steamIDClan, int iOfficer)
    {
      return (ulong) platform.ISteamFriends_GetClanOfficerByIndex(steamIDClan.Value, iOfficer);
    }

    public int GetClanOfficerCount(CSteamID steamIDClan)
    {
      return platform.ISteamFriends_GetClanOfficerCount(steamIDClan.Value);
    }

    public ulong GetClanOwner(CSteamID steamIDClan)
    {
      return (ulong) platform.ISteamFriends_GetClanOwner(steamIDClan.Value);
    }

    public string GetClanTag(CSteamID steamIDClan)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetClanTag(steamIDClan.Value));
    }

    public ulong GetCoplayFriend(int iCoplayFriend)
    {
      return (ulong) platform.ISteamFriends_GetCoplayFriend(iCoplayFriend);
    }

    public int GetCoplayFriendCount() => platform.ISteamFriends_GetCoplayFriendCount();

    public CallbackHandle GetFollowerCount(
      CSteamID steamID,
      Action<FriendsGetFollowerCount_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t followerCount = platform.ISteamFriends_GetFollowerCount(steamID.Value);
      return CallbackFunction == null ? null : FriendsGetFollowerCount_t.CallResult(steamworks, followerCount, CallbackFunction);
    }

    public ulong GetFriendByIndex(int iFriend, int iFriendFlags)
    {
      return (ulong) platform.ISteamFriends_GetFriendByIndex(iFriend, iFriendFlags);
    }

    public AppId_t GetFriendCoplayGame(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendCoplayGame(steamIDFriend.Value);
    }

    public int GetFriendCoplayTime(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendCoplayTime(steamIDFriend.Value);
    }

    public int GetFriendCount(int iFriendFlags)
    {
      return platform.ISteamFriends_GetFriendCount(iFriendFlags);
    }

    public int GetFriendCountFromSource(CSteamID steamIDSource)
    {
      return platform.ISteamFriends_GetFriendCountFromSource(steamIDSource.Value);
    }

    public ulong GetFriendFromSourceByIndex(CSteamID steamIDSource, int iFriend)
    {
      return (ulong) platform.ISteamFriends_GetFriendFromSourceByIndex(steamIDSource.Value, iFriend);
    }

    public bool GetFriendGamePlayed(CSteamID steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
    {
      return platform.ISteamFriends_GetFriendGamePlayed(steamIDFriend.Value, ref pFriendGameInfo);
    }

    public int GetFriendMessage(
      CSteamID steamIDFriend,
      int iMessageID,
      IntPtr pvData,
      int cubData,
      out ChatEntryType peChatEntryType)
    {
      return platform.ISteamFriends_GetFriendMessage(steamIDFriend.Value, iMessageID, pvData, cubData, out peChatEntryType);
    }

    public string GetFriendPersonaName(CSteamID steamIDFriend)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetFriendPersonaName(steamIDFriend.Value));
    }

    public string GetFriendPersonaNameHistory(CSteamID steamIDFriend, int iPersonaName)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetFriendPersonaNameHistory(steamIDFriend.Value, iPersonaName));
    }

    public PersonaState GetFriendPersonaState(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendPersonaState(steamIDFriend.Value);
    }

    public FriendRelationship GetFriendRelationship(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendRelationship(steamIDFriend.Value);
    }

    public string GetFriendRichPresence(CSteamID steamIDFriend, string pchKey)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetFriendRichPresence(steamIDFriend.Value, pchKey));
    }

    public string GetFriendRichPresenceKeyByIndex(CSteamID steamIDFriend, int iKey)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetFriendRichPresenceKeyByIndex(steamIDFriend.Value, iKey));
    }

    public int GetFriendRichPresenceKeyCount(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendRichPresenceKeyCount(steamIDFriend.Value);
    }

    public int GetFriendsGroupCount() => platform.ISteamFriends_GetFriendsGroupCount();

    public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
    {
      return platform.ISteamFriends_GetFriendsGroupIDByIndex(iFG);
    }

    public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
    {
      return platform.ISteamFriends_GetFriendsGroupMembersCount(friendsGroupID.Value);
    }

    public void GetFriendsGroupMembersList(
      FriendsGroupID_t friendsGroupID,
      IntPtr pOutSteamIDMembers,
      int nMembersCount)
    {
      platform.ISteamFriends_GetFriendsGroupMembersList(friendsGroupID.Value, pOutSteamIDMembers, nMembersCount);
    }

    public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetFriendsGroupName(friendsGroupID.Value));
    }

    public int GetFriendSteamLevel(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetFriendSteamLevel(steamIDFriend.Value);
    }

    public int GetLargeFriendAvatar(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetLargeFriendAvatar(steamIDFriend.Value);
    }

    public int GetMediumFriendAvatar(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetMediumFriendAvatar(steamIDFriend.Value);
    }

    public string GetPersonaName()
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetPersonaName());
    }

    public PersonaState GetPersonaState() => platform.ISteamFriends_GetPersonaState();

    public string GetPlayerNickname(CSteamID steamIDPlayer)
    {
      return Marshal.PtrToStringAnsi(platform.ISteamFriends_GetPlayerNickname(steamIDPlayer.Value));
    }

    public int GetSmallFriendAvatar(CSteamID steamIDFriend)
    {
      return platform.ISteamFriends_GetSmallFriendAvatar(steamIDFriend.Value);
    }

    public uint GetUserRestrictions() => platform.ISteamFriends_GetUserRestrictions();

    public bool HasFriend(CSteamID steamIDFriend, int iFriendFlags)
    {
      return platform.ISteamFriends_HasFriend(steamIDFriend.Value, iFriendFlags);
    }

    public bool InviteUserToGame(CSteamID steamIDFriend, string pchConnectString)
    {
      return platform.ISteamFriends_InviteUserToGame(steamIDFriend.Value, pchConnectString);
    }

    public bool IsClanChatAdmin(CSteamID steamIDClanChat, CSteamID steamIDUser)
    {
      return platform.ISteamFriends_IsClanChatAdmin(steamIDClanChat.Value, steamIDUser.Value);
    }

    public bool IsClanChatWindowOpenInSteam(CSteamID steamIDClanChat)
    {
      return platform.ISteamFriends_IsClanChatWindowOpenInSteam(steamIDClanChat.Value);
    }

    public CallbackHandle IsFollowing(
      CSteamID steamID,
      Action<FriendsIsFollowing_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamFriends_IsFollowing(steamID.Value);
      return CallbackFunction == null ? null : FriendsIsFollowing_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool IsUserInSource(CSteamID steamIDUser, CSteamID steamIDSource)
    {
      return platform.ISteamFriends_IsUserInSource(steamIDUser.Value, steamIDSource.Value);
    }

    public CallbackHandle JoinClanChatRoom(
      CSteamID steamIDClan,
      Action<JoinClanChatRoomCompletionResult_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamFriends_JoinClanChatRoom(steamIDClan.Value);
      return CallbackFunction == null ? null : JoinClanChatRoomCompletionResult_t.CallResult(steamworks, call, CallbackFunction);
    }

    public bool LeaveClanChatRoom(CSteamID steamIDClan)
    {
      return platform.ISteamFriends_LeaveClanChatRoom(steamIDClan.Value);
    }

    public bool OpenClanChatWindowInSteam(CSteamID steamIDClanChat)
    {
      return platform.ISteamFriends_OpenClanChatWindowInSteam(steamIDClanChat.Value);
    }

    public bool ReplyToFriendMessage(CSteamID steamIDFriend, string pchMsgToSend)
    {
      return platform.ISteamFriends_ReplyToFriendMessage(steamIDFriend.Value, pchMsgToSend);
    }

    public CallbackHandle RequestClanOfficerList(
      CSteamID steamIDClan,
      Action<ClanOfficerListResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamFriends_RequestClanOfficerList(steamIDClan.Value);
      return CallbackFunction == null ? null : ClanOfficerListResponse_t.CallResult(steamworks, call, CallbackFunction);
    }

    public void RequestFriendRichPresence(CSteamID steamIDFriend)
    {
      platform.ISteamFriends_RequestFriendRichPresence(steamIDFriend.Value);
    }

    public bool RequestUserInformation(CSteamID steamIDUser, bool bRequireNameOnly)
    {
      return platform.ISteamFriends_RequestUserInformation(steamIDUser.Value, bRequireNameOnly);
    }

    public bool SendClanChatMessage(CSteamID steamIDClanChat, string pchText)
    {
      return platform.ISteamFriends_SendClanChatMessage(steamIDClanChat.Value, pchText);
    }

    public void SetInGameVoiceSpeaking(CSteamID steamIDUser, bool bSpeaking)
    {
      platform.ISteamFriends_SetInGameVoiceSpeaking(steamIDUser.Value, bSpeaking);
    }

    public bool SetListenForFriendsMessages(bool bInterceptEnabled)
    {
      return platform.ISteamFriends_SetListenForFriendsMessages(bInterceptEnabled);
    }

    public CallbackHandle SetPersonaName(
      string pchPersonaName,
      Action<SetPersonaNameResponse_t, bool> CallbackFunction = null)
    {
      SteamAPICall_t steamApiCallT = 0UL;
      SteamAPICall_t call = platform.ISteamFriends_SetPersonaName(pchPersonaName);
      return CallbackFunction == null ? null : SetPersonaNameResponse_t.CallResult(steamworks, call, CallbackFunction);
    }

    public void SetPlayedWith(CSteamID steamIDUserPlayedWith)
    {
      platform.ISteamFriends_SetPlayedWith(steamIDUserPlayedWith.Value);
    }

    public bool SetRichPresence(string pchKey, string pchValue)
    {
      return platform.ISteamFriends_SetRichPresence(pchKey, pchValue);
    }
  }
}
