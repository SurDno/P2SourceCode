using System;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Audio;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class RemoveEntityIfDurabilityZeroEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnityAsset<AudioClip> removeSound;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnitySubAsset<AudioMixerGroup> removeSoundMixer;

	public string Name => GetType().Name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		var item = AbilityItem.Item;
		var component = item.GetComponent<ParametersComponent>();
		if (component == null) {
			Debug.LogWarning(string.Format("{0} has no {1}", typeof(RemoveEntityIfDurabilityZeroEffect),
				typeof(ParametersComponent).Name));
			return false;
		}

		var byName = component.GetByName<float>(ParameterNameEnum.Durability);
		if (byName == null) {
			Debug.LogWarning(string.Format("{0} has no durability parameter",
				typeof(RemoveEntityIfDurabilityZeroEffect)));
			return false;
		}

		if (byName.Value <= 0.0)
			CoroutineService.Instance.WaitFrame((Action)(() => {
				if ((bool)(Object)removeSound.Value)
					SoundUtility.PlayAudioClip2D(removeSound.Value, removeSoundMixer.Value, 1f, 0.0f);
				item.Dispose();
			}));
		return false;
	}

	public void Cleanup() { }
}