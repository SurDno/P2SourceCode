// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Networking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Networking : IDisposable
  {
    private static byte[] ReceiveBuffer = new byte[65536];
    public Networking.OnRecievedP2PData OnP2PData;
    public Func<ulong, bool> OnIncomingConnection;
    public Action<ulong, Networking.SessionError> OnConnectionFailed;
    private List<int> ListenChannels = new List<int>();
    private Stopwatch UpdateTimer = Stopwatch.StartNew();
    internal SteamNetworking networking;

    internal Networking(BaseSteamworks steamworks, SteamNetworking networking)
    {
      this.networking = networking;
      P2PSessionRequest_t.RegisterCallback(steamworks, new Action<P2PSessionRequest_t, bool>(this.onP2PConnectionRequest));
      P2PSessionConnectFail_t.RegisterCallback(steamworks, new Action<P2PSessionConnectFail_t, bool>(this.onP2PConnectionFailed));
    }

    public void Dispose()
    {
      this.networking = (SteamNetworking) null;
      this.OnIncomingConnection = (Func<ulong, bool>) null;
      this.OnConnectionFailed = (Action<ulong, Networking.SessionError>) null;
      this.OnP2PData = (Networking.OnRecievedP2PData) null;
      this.ListenChannels.Clear();
    }

    public void Update()
    {
      if (this.OnP2PData == null || this.UpdateTimer.Elapsed.TotalSeconds < 1.0 / 60.0)
        return;
      this.UpdateTimer.Reset();
      this.UpdateTimer.Start();
      using (List<int>.Enumerator enumerator = this.ListenChannels.GetEnumerator())
      {
label_6:
        if (!enumerator.MoveNext())
          return;
        int current = enumerator.Current;
        while (this.ReadP2PPacket(current))
          ;
        goto label_6;
      }
    }

    public void SetListenChannel(int ChannelId, bool Listen)
    {
      this.ListenChannels.RemoveAll((Predicate<int>) (x => x == ChannelId));
      if (!Listen)
        return;
      this.ListenChannels.Add(ChannelId);
    }

    private void onP2PConnectionRequest(P2PSessionRequest_t o, bool b)
    {
      if (this.OnIncomingConnection != null)
      {
        if (this.OnIncomingConnection(o.SteamIDRemote))
          this.networking.AcceptP2PSessionWithUser((CSteamID) o.SteamIDRemote);
        else
          this.networking.CloseP2PSessionWithUser((CSteamID) o.SteamIDRemote);
      }
      else
        this.networking.CloseP2PSessionWithUser((CSteamID) o.SteamIDRemote);
    }

    private void onP2PConnectionFailed(P2PSessionConnectFail_t o, bool b)
    {
      if (this.OnConnectionFailed == null)
        return;
      this.OnConnectionFailed(o.SteamIDRemote, (Networking.SessionError) o.P2PSessionError);
    }

    public unsafe bool SendP2PPacket(
      ulong steamid,
      byte[] data,
      int length,
      Networking.SendType eP2PSendType = Networking.SendType.Reliable,
      int nChannel = 0)
    {
      fixed (byte* pubData = data)
        return this.networking.SendP2PPacket((CSteamID) steamid, (IntPtr) (void*) pubData, (uint) length, (P2PSend) eP2PSendType, nChannel);
    }

    private unsafe bool ReadP2PPacket(int channel)
    {
      uint pcubMsgSize = 0;
      if (!this.networking.IsP2PPacketAvailable(out pcubMsgSize, channel))
        return false;
      if ((long) Networking.ReceiveBuffer.Length < (long) pcubMsgSize)
        Networking.ReceiveBuffer = new byte[(int) pcubMsgSize + 1024];
      fixed (byte* pubDest = Networking.ReceiveBuffer)
      {
        CSteamID psteamIDRemote = (CSteamID) 1UL;
        if (!this.networking.ReadP2PPacket((IntPtr) (void*) pubDest, pcubMsgSize, out pcubMsgSize, out psteamIDRemote, channel) || pcubMsgSize == 0U)
          return false;
        Networking.OnRecievedP2PData onP2Pdata = this.OnP2PData;
        if (onP2Pdata != null)
          onP2Pdata((ulong) psteamIDRemote, Networking.ReceiveBuffer, (int) pcubMsgSize, channel);
        return true;
      }
    }

    public delegate void OnRecievedP2PData(
      ulong steamid,
      byte[] data,
      int dataLength,
      int channel);

    public enum SessionError : byte
    {
      None,
      NotRunningApp,
      NoRightsToApp,
      DestinationNotLoggedIn,
      Timeout,
      Max,
    }

    public enum SendType
    {
      Unreliable,
      UnreliableNoDelay,
      Reliable,
      ReliableWithBuffering,
    }
  }
}
