// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.ServerAuth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class ServerAuth
  {
    internal Server server;
    public Action<ulong, ulong, ServerAuth.Status> OnAuthChange;

    internal ServerAuth(Server s)
    {
      this.server = s;
      ValidateAuthTicketResponse_t.RegisterCallback((BaseSteamworks) this.server, new Action<ValidateAuthTicketResponse_t, bool>(this.OnAuthTicketValidate));
    }

    private void OnAuthTicketValidate(ValidateAuthTicketResponse_t data, bool b)
    {
      if (this.OnAuthChange == null)
        return;
      this.OnAuthChange(data.SteamID, data.OwnerSteamID, (ServerAuth.Status) data.AuthSessionResponse);
    }

    public unsafe bool StartSession(byte[] data, ulong steamid)
    {
      fixed (byte* pAuthTicket = data)
        return this.server.native.gameServer.BeginAuthSession((IntPtr) (void*) pAuthTicket, data.Length, (CSteamID) steamid) == BeginAuthSessionResult.OK;
    }

    public void EndSession(ulong steamid)
    {
      this.server.native.gameServer.EndAuthSession((CSteamID) steamid);
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
