using SteamNative;

namespace Facepunch.Steamworks
{
  public class SteamFriend
  {
    public string Name;
    private PersonaState personaState;
    private FriendRelationship relationship;

    public ulong Id { get; internal set; }

    public bool IsBlocked => this.relationship == FriendRelationship.Blocked;

    public bool IsFriend => this.relationship == FriendRelationship.Friend;

    public bool IsOnline => this.personaState != 0;

    public bool IsAway => this.personaState == PersonaState.Away;

    public bool IsBusy => this.personaState == PersonaState.Busy;

    public bool IsSnoozing => this.personaState == PersonaState.Snooze;

    public bool IsPlayingThisGame => (long) this.CurrentAppId == (long) this.Client.AppId;

    public bool IsPlaying => this.CurrentAppId > 0UL;

    public ulong CurrentAppId { get; internal set; }

    public uint ServerIp { get; internal set; }

    public int ServerGamePort { get; internal set; }

    public int ServerQueryPort { get; internal set; }

    public ulong ServerLobbyId { get; internal set; }

    internal Client Client { get; set; }

    public string GetRichPresence(string key)
    {
      string friendRichPresence = this.Client.native.friends.GetFriendRichPresence((CSteamID) this.Id, key);
      return string.IsNullOrEmpty(friendRichPresence) ? (string) null : friendRichPresence;
    }

    public void Refresh()
    {
      this.Name = this.Client.native.friends.GetFriendPersonaName((CSteamID) this.Id);
      this.relationship = this.Client.native.friends.GetFriendRelationship((CSteamID) this.Id);
      this.personaState = this.Client.native.friends.GetFriendPersonaState((CSteamID) this.Id);
      this.CurrentAppId = 0UL;
      this.ServerIp = 0U;
      this.ServerGamePort = 0;
      this.ServerQueryPort = 0;
      this.ServerLobbyId = 0UL;
      FriendGameInfo_t pFriendGameInfo = new FriendGameInfo_t();
      if (this.Client.native.friends.GetFriendGamePlayed((CSteamID) this.Id, ref pFriendGameInfo) && pFriendGameInfo.GameID > 0UL)
      {
        this.CurrentAppId = pFriendGameInfo.GameID;
        this.ServerIp = pFriendGameInfo.GameIP;
        this.ServerGamePort = (int) pFriendGameInfo.GamePort;
        this.ServerQueryPort = (int) pFriendGameInfo.QueryPort;
        this.ServerLobbyId = pFriendGameInfo.SteamIDLobby;
      }
      this.Client.native.friends.RequestFriendRichPresence((CSteamID) this.Id);
    }

    public Image GetAvatar(Friends.AvatarSize size)
    {
      return this.Client.Friends.GetCachedAvatar(size, this.Id);
    }
  }
}
