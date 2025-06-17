using System;
using System.Collections.Generic;
using System.Diagnostics;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class Networking : IDisposable
  {
    private static byte[] ReceiveBuffer = new byte[65536];
    public OnRecievedP2PData OnP2PData;
    public Func<ulong, bool> OnIncomingConnection;
    public Action<ulong, SessionError> OnConnectionFailed;
    private List<int> ListenChannels = [];
    private Stopwatch UpdateTimer = Stopwatch.StartNew();
    internal SteamNetworking networking;

    internal Networking(BaseSteamworks steamworks, SteamNetworking networking)
    {
      this.networking = networking;
      P2PSessionRequest_t.RegisterCallback(steamworks, onP2PConnectionRequest);
      P2PSessionConnectFail_t.RegisterCallback(steamworks, onP2PConnectionFailed);
    }

    public void Dispose()
    {
      networking = null;
      OnIncomingConnection = null;
      OnConnectionFailed = null;
      OnP2PData = null;
      ListenChannels.Clear();
    }

    public void Update()
    {
      if (OnP2PData == null || UpdateTimer.Elapsed.TotalSeconds < 1.0 / 60.0)
        return;
      UpdateTimer.Reset();
      UpdateTimer.Start();
      using (List<int>.Enumerator enumerator = ListenChannels.GetEnumerator())
      {
label_6:
        if (!enumerator.MoveNext())
          return;
        int current = enumerator.Current;
        while (ReadP2PPacket(current))
          ;
        goto label_6;
      }
    }

    public void SetListenChannel(int ChannelId, bool Listen)
    {
      ListenChannels.RemoveAll(x => x == ChannelId);
      if (!Listen)
        return;
      ListenChannels.Add(ChannelId);
    }

    private void onP2PConnectionRequest(P2PSessionRequest_t o, bool b)
    {
      if (OnIncomingConnection != null)
      {
        if (OnIncomingConnection(o.SteamIDRemote))
          networking.AcceptP2PSessionWithUser(o.SteamIDRemote);
        else
          networking.CloseP2PSessionWithUser(o.SteamIDRemote);
      }
      else
        networking.CloseP2PSessionWithUser(o.SteamIDRemote);
    }

    private void onP2PConnectionFailed(P2PSessionConnectFail_t o, bool b)
    {
      if (OnConnectionFailed == null)
        return;
      OnConnectionFailed(o.SteamIDRemote, (SessionError) o.P2PSessionError);
    }

    public unsafe bool SendP2PPacket(
      ulong steamid,
      byte[] data,
      int length,
      SendType eP2PSendType = SendType.Reliable,
      int nChannel = 0)
    {
      fixed (byte* pubData = data)
        return networking.SendP2PPacket(steamid, (IntPtr) pubData, (uint) length, (P2PSend) eP2PSendType, nChannel);
    }

    private unsafe bool ReadP2PPacket(int channel)
    {
      if (!networking.IsP2PPacketAvailable(out uint pcubMsgSize, channel))
        return false;
      if (ReceiveBuffer.Length < pcubMsgSize)
        ReceiveBuffer = new byte[(int) pcubMsgSize + 1024];
      fixed (byte* pubDest = ReceiveBuffer)
      {
        if (!networking.ReadP2PPacket((IntPtr) pubDest, pcubMsgSize, out pcubMsgSize, out CSteamID psteamIDRemote, channel) || pcubMsgSize == 0U)
          return false;
        OnRecievedP2PData onP2Pdata = OnP2PData;
        if (onP2Pdata != null)
          onP2Pdata(psteamIDRemote, ReceiveBuffer, (int) pcubMsgSize, channel);
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
