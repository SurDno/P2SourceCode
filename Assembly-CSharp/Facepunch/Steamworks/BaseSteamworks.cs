using System;
using System.Collections.Generic;
using System.Threading;
using Facepunch.Steamworks.Interop;
using SteamNative;

namespace Facepunch.Steamworks;

public class BaseSteamworks : IDisposable {
	internal NativeInterface native;
	private List<CallbackHandle> CallbackHandles = new();

	public uint AppId { get; internal set; }

	public Networking Networking { get; internal set; }

	public Inventory Inventory { get; internal set; }

	public Workshop Workshop { get; internal set; }

	internal event Action OnUpdate;

	public virtual void Dispose() {
		foreach (var callbackHandle in CallbackHandles)
			callbackHandle.Dispose();
		CallbackHandles.Clear();
		if (Workshop != null) {
			Workshop.Dispose();
			Workshop = null;
		}

		if (Inventory != null) {
			Inventory.Dispose();
			Inventory = null;
		}

		if (Networking != null) {
			Networking.Dispose();
			Networking = null;
		}

		if (native == null)
			return;
		native.Dispose();
		native = null;
	}

	protected void SetupCommonInterfaces() {
		Networking = new Networking(this, native.networking);
		Inventory = new Inventory(this, native.inventory, IsGameServer);
		Workshop = new Workshop(this, native.ugc, native.remoteStorage);
	}

	public bool IsValid => native != null;

	internal virtual bool IsGameServer => false;

	internal void RegisterCallbackHandle(CallbackHandle handle) {
		CallbackHandles.Add(handle);
	}

	public virtual void Update() {
		Inventory.Update();
		Networking.Update();
		RunUpdateCallbacks();
	}

	public void RunUpdateCallbacks() {
		if (OnUpdate == null)
			return;
		OnUpdate();
	}

	public void UpdateWhile(Func<bool> func) {
		while (func()) {
			Update();
			Thread.Sleep(1);
		}
	}
}