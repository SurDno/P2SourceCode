// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.MicroTransactions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;

#nullable disable
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
