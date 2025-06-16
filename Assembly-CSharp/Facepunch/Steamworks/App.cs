using System;

namespace Facepunch.Steamworks
{
  public class App : IDisposable
  {
    internal Client client;

    internal App(Client c) => client = c;

    public void Dispose() => client = null;

    public void MarkContentCorrupt(bool missingFilesOnly = false)
    {
      client.native.apps.MarkContentCorrupt(missingFilesOnly);
    }

    public void InstallDlc(uint appId) => client.native.apps.InstallDLC(appId);

    public void UninstallDlc(uint appId) => client.native.apps.UninstallDLC(appId);

    public DateTime PurchaseTime(uint appId)
    {
      uint purchaseUnixTime = client.native.apps.GetEarliestPurchaseUnixTime(appId);
      return purchaseUnixTime == 0U ? DateTime.MinValue : Utility.Epoch.ToDateTime(purchaseUnixTime);
    }

    public bool IsSubscribed(uint appId)
    {
      return client.native.apps.BIsSubscribedApp(appId);
    }

    public bool IsInstalled(uint appId) => client.native.apps.BIsAppInstalled(appId);
  }
}
