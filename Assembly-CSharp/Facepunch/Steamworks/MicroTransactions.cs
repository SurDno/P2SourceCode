using SteamNative;
using System;

namespace Facepunch.Steamworks
{
  public class MicroTransactions : IDisposable
  {
    internal Client client;

    public event MicroTransactions.AuthorizationResponse OnAuthorizationResponse;

    internal MicroTransactions(Client c)
    {
      this.client = c;
      MicroTxnAuthorizationResponse_t.RegisterCallback((BaseSteamworks) this.client, new Action<MicroTxnAuthorizationResponse_t, bool>(this.onMicroTxnAuthorizationResponse));
    }

    private void onMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t arg1, bool arg2)
    {
      if (this.OnAuthorizationResponse == null)
        return;
      this.OnAuthorizationResponse(arg1.Authorized == (byte) 1, (int) arg1.AppID, arg1.OrderID);
    }

    public void Dispose() => this.client = (Client) null;

    public delegate void AuthorizationResponse(bool authorized, int appId, ulong orderId);
  }
}
