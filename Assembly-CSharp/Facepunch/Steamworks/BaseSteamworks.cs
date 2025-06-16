// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.BaseSteamworks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Facepunch.Steamworks
{
  public class BaseSteamworks : IDisposable
  {
    internal NativeInterface native;
    private List<CallbackHandle> CallbackHandles = new List<CallbackHandle>();

    public uint AppId { get; internal set; }

    public Networking Networking { get; internal set; }

    public Inventory Inventory { get; internal set; }

    public Workshop Workshop { get; internal set; }

    internal event Action OnUpdate;

    public virtual void Dispose()
    {
      foreach (CallbackHandle callbackHandle in this.CallbackHandles)
        callbackHandle.Dispose();
      this.CallbackHandles.Clear();
      if (this.Workshop != null)
      {
        this.Workshop.Dispose();
        this.Workshop = (Workshop) null;
      }
      if (this.Inventory != null)
      {
        this.Inventory.Dispose();
        this.Inventory = (Inventory) null;
      }
      if (this.Networking != null)
      {
        this.Networking.Dispose();
        this.Networking = (Networking) null;
      }
      if (this.native == null)
        return;
      this.native.Dispose();
      this.native = (NativeInterface) null;
    }

    protected void SetupCommonInterfaces()
    {
      this.Networking = new Networking(this, this.native.networking);
      this.Inventory = new Inventory(this, this.native.inventory, this.IsGameServer);
      this.Workshop = new Workshop(this, this.native.ugc, this.native.remoteStorage);
    }

    public bool IsValid => this.native != null;

    internal virtual bool IsGameServer => false;

    internal void RegisterCallbackHandle(CallbackHandle handle) => this.CallbackHandles.Add(handle);

    public virtual void Update()
    {
      this.Inventory.Update();
      this.Networking.Update();
      this.RunUpdateCallbacks();
    }

    public void RunUpdateCallbacks()
    {
      if (this.OnUpdate == null)
        return;
      this.OnUpdate();
    }

    public void UpdateWhile(Func<bool> func)
    {
      while (func())
      {
        this.Update();
        Thread.Sleep(1);
      }
    }
  }
}
