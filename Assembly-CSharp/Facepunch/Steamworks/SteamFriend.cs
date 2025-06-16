using SteamNative;

namespace Facepunch.Steamworks
{
  public class SteamFriend
  {
    public string Name;
    private PersonaState personaState;
    private FriendRelationship relationship;

    public ulong Id { get; internal set; }

    public bool IsBlocked => relationship == FriendRelationship.Blocked;

    public bool IsFriend => relationship == FriendRelationship.Friend;

    public bool IsOnline => personaState != 0;

    public bool IsAway => personaState == PersonaState.Away;

    public bool IsBusy => personaState == PersonaState.Busy;

    public bool IsSnoozing => personaState == PersonaState.Snooze;

    public bool IsPlayingThisGame => (long) CurrentAppId == Client.AppId;

    public bool IsPlaying => CurrentAppId > 0UL;

    public ulong CurrentAppId { get; internal set; }

    public uint ServerIp { get; internal set; }

    public int ServerGamePort { get; internal set; }

    public int ServerQueryPort { get; internal set; }

    public ulong ServerLobbyId { get; internal set; }

    internal Client Client { get; set; }

    public string GetRichPresence(string key)
    {
      string friendRichPresence = Client.native.friends.GetFriendRichPresence(Id, key);
      return string.IsNullOrEmpty(friendRichPresence) ? null : friendRichPresence;
    }

    public void Refresh()
    {
      Name = Client.native.friends.GetFriendPersonaName(Id);
      relationship = Client.native.friends.GetFriendRelationship(Id);
      personaState = Client.native.friends.GetFriendPersonaState(Id);
      CurrentAppId = 0UL;
      ServerIp = 0U;
      ServerGamePort = 0;
      ServerQueryPort = 0;
      ServerLobbyId = 0UL;
      FriendGameInfo_t pFriendGameInfo = new FriendGameInfo_t();
      if (Client.native.friends.GetFriendGamePlayed(Id, ref pFriendGameInfo) && pFriendGameInfo.GameID > 0UL)
      {
        CurrentAppId = pFriendGameInfo.GameID;
        ServerIp = pFriendGameInfo.GameIP;
        ServerGamePort = pFriendGameInfo.GamePort;
        ServerQueryPort = pFriendGameInfo.QueryPort;
        ServerLobbyId = pFriendGameInfo.SteamIDLobby;
      }
      Client.native.friends.RequestFriendRichPresence(Id);
    }

    public Image GetAvatar(Friends.AvatarSize size)
    {
      return Client.Friends.GetCachedAvatar(size, Id);
    }
  }
}
