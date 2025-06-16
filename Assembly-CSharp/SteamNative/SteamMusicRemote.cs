using System;
using Facepunch.Steamworks;

namespace SteamNative
{
  internal class SteamMusicRemote : IDisposable
  {
    internal Platform.Interface platform;
    internal BaseSteamworks steamworks;

    internal SteamMusicRemote(BaseSteamworks steamworks, IntPtr pointer)
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

    public bool BActivationSuccess(bool bValue)
    {
      return platform.ISteamMusicRemote_BActivationSuccess(bValue);
    }

    public bool BIsCurrentMusicRemote() => platform.ISteamMusicRemote_BIsCurrentMusicRemote();

    public bool CurrentEntryDidChange() => platform.ISteamMusicRemote_CurrentEntryDidChange();

    public bool CurrentEntryIsAvailable(bool bAvailable)
    {
      return platform.ISteamMusicRemote_CurrentEntryIsAvailable(bAvailable);
    }

    public bool CurrentEntryWillChange()
    {
      return platform.ISteamMusicRemote_CurrentEntryWillChange();
    }

    public bool DeregisterSteamMusicRemote()
    {
      return platform.ISteamMusicRemote_DeregisterSteamMusicRemote();
    }

    public bool EnableLooped(bool bValue) => platform.ISteamMusicRemote_EnableLooped(bValue);

    public bool EnablePlaylists(bool bValue)
    {
      return platform.ISteamMusicRemote_EnablePlaylists(bValue);
    }

    public bool EnablePlayNext(bool bValue)
    {
      return platform.ISteamMusicRemote_EnablePlayNext(bValue);
    }

    public bool EnablePlayPrevious(bool bValue)
    {
      return platform.ISteamMusicRemote_EnablePlayPrevious(bValue);
    }

    public bool EnableQueue(bool bValue) => platform.ISteamMusicRemote_EnableQueue(bValue);

    public bool EnableShuffled(bool bValue)
    {
      return platform.ISteamMusicRemote_EnableShuffled(bValue);
    }

    public bool PlaylistDidChange() => platform.ISteamMusicRemote_PlaylistDidChange();

    public bool PlaylistWillChange() => platform.ISteamMusicRemote_PlaylistWillChange();

    public bool QueueDidChange() => platform.ISteamMusicRemote_QueueDidChange();

    public bool QueueWillChange() => platform.ISteamMusicRemote_QueueWillChange();

    public bool RegisterSteamMusicRemote(string pchName)
    {
      return platform.ISteamMusicRemote_RegisterSteamMusicRemote(pchName);
    }

    public bool ResetPlaylistEntries() => platform.ISteamMusicRemote_ResetPlaylistEntries();

    public bool ResetQueueEntries() => platform.ISteamMusicRemote_ResetQueueEntries();

    public bool SetCurrentPlaylistEntry(int nID)
    {
      return platform.ISteamMusicRemote_SetCurrentPlaylistEntry(nID);
    }

    public bool SetCurrentQueueEntry(int nID)
    {
      return platform.ISteamMusicRemote_SetCurrentQueueEntry(nID);
    }

    public bool SetDisplayName(string pchDisplayName)
    {
      return platform.ISteamMusicRemote_SetDisplayName(pchDisplayName);
    }

    public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
      return platform.ISteamMusicRemote_SetPlaylistEntry(nID, nPosition, pchEntryText);
    }

    public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
      return platform.ISteamMusicRemote_SetPNGIcon_64x64(pvBuffer, cbBufferLength);
    }

    public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
      return platform.ISteamMusicRemote_SetQueueEntry(nID, nPosition, pchEntryText);
    }

    public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
      return platform.ISteamMusicRemote_UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
    }

    public bool UpdateCurrentEntryElapsedSeconds(int nValue)
    {
      return platform.ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(nValue);
    }

    public bool UpdateCurrentEntryText(string pchText)
    {
      return platform.ISteamMusicRemote_UpdateCurrentEntryText(pchText);
    }

    public bool UpdateLooped(bool bValue) => platform.ISteamMusicRemote_UpdateLooped(bValue);

    public bool UpdatePlaybackStatus(AudioPlayback_Status nStatus)
    {
      return platform.ISteamMusicRemote_UpdatePlaybackStatus(nStatus);
    }

    public bool UpdateShuffled(bool bValue)
    {
      return platform.ISteamMusicRemote_UpdateShuffled(bValue);
    }

    public bool UpdateVolume(float flValue)
    {
      return platform.ISteamMusicRemote_UpdateVolume(flValue);
    }
  }
}
