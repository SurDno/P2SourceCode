using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using UnityEngine;

namespace Engine.Source.Components.Saves;

[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class TeleportData {
	[StateSaveProxy(MemberEnum.CustomReference)] [StateLoadProxy(MemberEnum.CustomReference)]
	public ILocationComponent Location;

	[StateSaveProxy(MemberEnum.CustomReference)] [StateLoadProxy(MemberEnum.CustomReference)]
	public IEntity Target;

	[StateSaveProxy] [StateLoadProxy] public Vector3 Position;
	[StateSaveProxy] [StateLoadProxy()] public Quaternion Rotation;
}