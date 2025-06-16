// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.App
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class App : IDisposable
  {
    internal Client client;

    internal App(Client c) => this.client = c;

    public void Dispose() => this.client = (Client) null;

    public void MarkContentCorrupt(bool missingFilesOnly = false)
    {
      this.client.native.apps.MarkContentCorrupt(missingFilesOnly);
    }

    public void InstallDlc(uint appId) => this.client.native.apps.InstallDLC((AppId_t) appId);

    public void UninstallDlc(uint appId) => this.client.native.apps.UninstallDLC((AppId_t) appId);

    public DateTime PurchaseTime(uint appId)
    {
      uint purchaseUnixTime = this.client.native.apps.GetEarliestPurchaseUnixTime((AppId_t) appId);
      return purchaseUnixTime == 0U ? DateTime.MinValue : Utility.Epoch.ToDateTime((Decimal) purchaseUnixTime);
    }

    public bool IsSubscribed(uint appId)
    {
      return this.client.native.apps.BIsSubscribedApp((AppId_t) appId);
    }

    public bool IsInstalled(uint appId) => this.client.native.apps.BIsAppInstalled((AppId_t) appId);
  }
}
