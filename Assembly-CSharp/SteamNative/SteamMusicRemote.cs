// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamMusicRemote
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;

#nullable disable
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

    public bool BActivationSuccess(bool bValue)
    {
      return this.platform.ISteamMusicRemote_BActivationSuccess(bValue);
    }

    public bool BIsCurrentMusicRemote() => this.platform.ISteamMusicRemote_BIsCurrentMusicRemote();

    public bool CurrentEntryDidChange() => this.platform.ISteamMusicRemote_CurrentEntryDidChange();

    public bool CurrentEntryIsAvailable(bool bAvailable)
    {
      return this.platform.ISteamMusicRemote_CurrentEntryIsAvailable(bAvailable);
    }

    public bool CurrentEntryWillChange()
    {
      return this.platform.ISteamMusicRemote_CurrentEntryWillChange();
    }

    public bool DeregisterSteamMusicRemote()
    {
      return this.platform.ISteamMusicRemote_DeregisterSteamMusicRemote();
    }

    public bool EnableLooped(bool bValue) => this.platform.ISteamMusicRemote_EnableLooped(bValue);

    public bool EnablePlaylists(bool bValue)
    {
      return this.platform.ISteamMusicRemote_EnablePlaylists(bValue);
    }

    public bool EnablePlayNext(bool bValue)
    {
      return this.platform.ISteamMusicRemote_EnablePlayNext(bValue);
    }

    public bool EnablePlayPrevious(bool bValue)
    {
      return this.platform.ISteamMusicRemote_EnablePlayPrevious(bValue);
    }

    public bool EnableQueue(bool bValue) => this.platform.ISteamMusicRemote_EnableQueue(bValue);

    public bool EnableShuffled(bool bValue)
    {
      return this.platform.ISteamMusicRemote_EnableShuffled(bValue);
    }

    public bool PlaylistDidChange() => this.platform.ISteamMusicRemote_PlaylistDidChange();

    public bool PlaylistWillChange() => this.platform.ISteamMusicRemote_PlaylistWillChange();

    public bool QueueDidChange() => this.platform.ISteamMusicRemote_QueueDidChange();

    public bool QueueWillChange() => this.platform.ISteamMusicRemote_QueueWillChange();

    public bool RegisterSteamMusicRemote(string pchName)
    {
      return this.platform.ISteamMusicRemote_RegisterSteamMusicRemote(pchName);
    }

    public bool ResetPlaylistEntries() => this.platform.ISteamMusicRemote_ResetPlaylistEntries();

    public bool ResetQueueEntries() => this.platform.ISteamMusicRemote_ResetQueueEntries();

    public bool SetCurrentPlaylistEntry(int nID)
    {
      return this.platform.ISteamMusicRemote_SetCurrentPlaylistEntry(nID);
    }

    public bool SetCurrentQueueEntry(int nID)
    {
      return this.platform.ISteamMusicRemote_SetCurrentQueueEntry(nID);
    }

    public bool SetDisplayName(string pchDisplayName)
    {
      return this.platform.ISteamMusicRemote_SetDisplayName(pchDisplayName);
    }

    public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
      return this.platform.ISteamMusicRemote_SetPlaylistEntry(nID, nPosition, pchEntryText);
    }

    public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
      return this.platform.ISteamMusicRemote_SetPNGIcon_64x64(pvBuffer, cbBufferLength);
    }

    public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
      return this.platform.ISteamMusicRemote_SetQueueEntry(nID, nPosition, pchEntryText);
    }

    public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
      return this.platform.ISteamMusicRemote_UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
    }

    public bool UpdateCurrentEntryElapsedSeconds(int nValue)
    {
      return this.platform.ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(nValue);
    }

    public bool UpdateCurrentEntryText(string pchText)
    {
      return this.platform.ISteamMusicRemote_UpdateCurrentEntryText(pchText);
    }

    public bool UpdateLooped(bool bValue) => this.platform.ISteamMusicRemote_UpdateLooped(bValue);

    public bool UpdatePlaybackStatus(AudioPlayback_Status nStatus)
    {
      return this.platform.ISteamMusicRemote_UpdatePlaybackStatus(nStatus);
    }

    public bool UpdateShuffled(bool bValue)
    {
      return this.platform.ISteamMusicRemote_UpdateShuffled(bValue);
    }

    public bool UpdateVolume(float flValue)
    {
      return this.platform.ISteamMusicRemote_UpdateVolume(flValue);
    }
  }
}
