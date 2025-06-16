using System;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class ServerAuth
  {
    internal Server server;
    public Action<ulong, ulong, Status> OnAuthChange;

    internal ServerAuth(Server s)
    {
      server = s;
      ValidateAuthTicketResponse_t.RegisterCallback(server, OnAuthTicketValidate);
    }

    private void OnAuthTicketValidate(ValidateAuthTicketResponse_t data, bool b)
    {
      if (OnAuthChange == null)
        return;
      OnAuthChange(data.SteamID, data.OwnerSteamID, (Status) data.AuthSessionResponse);
    }

    public unsafe bool StartSession(byte[] data, ulong steamid)
    {
      fixed (byte* pAuthTicket = data)
        return server.native.gameServer.BeginAuthSession((IntPtr) pAuthTicket, data.Length, steamid) == BeginAuthSessionResult.OK;
    }

    public void EndSession(ulong steamid)
    {
      server.native.gameServer.EndAuthSession(steamid);
    }

    public enum Status
    {
      OK,
      UserNotConnectedToSteam,
      NoLicenseOrExpired,
      VACBanned,
      LoggedInElseWhere,
      VACCheckTimedOut,
      AuthTicketCanceled,
      AuthTicketInvalidAlreadyUsed,
      AuthTicketInvalid,
      PublisherIssuedBan,
    }
  }
}
