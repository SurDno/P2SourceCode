using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components;

[Factory(typeof(IMessangerStationaryComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class MessangerStationaryComponent :
	EngineComponent,
	IMessangerStationaryComponent,
	IComponent,
	INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy]
	protected SpawnpointKindEnum spawnpointKindEnum;

	[StateSaveProxy] [StateLoadProxy()] [Inspected]
	protected bool registred;

	public SpawnpointKindEnum SpawnpointKind {
		get => spawnpointKindEnum;
		set {
			spawnpointKindEnum = value;
			if (!registred)
				return;
			StopTeleporting();
			StartTeleporting();
		}
	}

	public bool NeedSave => true;

	public override void OnAdded() { }

	public override void OnRemoved() {
		if (!registred)
			return;
		StopTeleporting();
	}

	public void StartTeleporting() {
		ServiceLocator.GetService<PostmanStaticTeleportService>().RegisterPostman(Owner, spawnpointKindEnum);
		registred = true;
	}

	public void StopTeleporting() {
		ServiceLocator.GetService<PostmanStaticTeleportService>().UnregisterPostman(Owner);
		registred = false;
	}

	[OnLoaded]
	protected void OnLoaded() {
		if (!registred)
			return;
		ServiceLocator.GetService<PostmanStaticTeleportService>().RegisterPostman(Owner, spawnpointKindEnum);
	}
}