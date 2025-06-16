using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Consoles;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class ConsoleGroupDebug {
	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action)(() =>
			InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(
				new UpdatableProxy((Action)(() => Update()))));
	}

	private static void Update() {
		TryExecByHotKey(KeyCode.Q);
		TryExecByHotKey(KeyCode.W);
		TryExecByHotKey(KeyCode.E);
		TryExecByHotKey(KeyCode.R);
		TryExecByHotKey(KeyCode.T);
		TryExecByHotKey(KeyCode.Y);
		TryExecByHotKey(KeyCode.U);
		TryExecByHotKey(KeyCode.I);
		TryExecByHotKey(KeyCode.O);
		TryExecByHotKey(KeyCode.P);
	}

	private static void TryExecByHotKey(KeyCode key) {
		if (!InputUtility.IsKeyDown(key, KeyModifficator.Control | KeyModifficator.Shift))
			return;
		ServiceLocator.GetService<ConsoleService>()
			.ExecuteCommand("exec hotkey_" + key.ToString().ToLowerInvariant() + ".txt");
	}
}