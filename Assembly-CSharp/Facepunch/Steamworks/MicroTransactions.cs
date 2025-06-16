using System;
using SteamNative;

namespace Facepunch.Steamworks
{
  public class MicroTransactions : IDisposable
  {
    internal Client client;

    public event AuthorizationResponse OnAuthorizationResponse;

    internal MicroTransactions(Client c)
    {
      client = c;
      MicroTxnAuthorizationResponse_t.RegisterCallback(client, onMicroTxnAuthorizationResponse);
    }

    private void onMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t arg1, bool arg2)
    {
      if (OnAuthorizationResponse == null)
        return;
      OnAuthorizationResponse(arg1.Authorized == 1, (int) arg1.AppID, arg1.OrderID);
    }

    public void Dispose() => client = null;

    public delegate void AuthorizationResponse(bool authorized, int appId, ulong orderId);
  }
}
