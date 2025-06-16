using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class AbilitiesComponent : EngineComponent, INeedSave {
	[DataReadProxy(Name = "Abilities")]
	[DataWriteProxy(Name = "Abilities")]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy()]
	protected List<Typed<IAbility>> resourceAbilities = new();

	[Inspected] private List<IAbility> abilities = new();

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			if (template.GetComponent<AbilitiesComponent>() != null)
				return false;
			Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
			return true;
		}
	}

	public override void OnAdded() {
		base.OnAdded();
		foreach (var resourceAbility in resourceAbilities) {
			var source = resourceAbility.Value;
			if (source != null) {
				var ability = CloneableObjectUtility.Clone(source);
				abilities.Add(ability);
				((Ability)ability).Initialise(Owner);
			}
		}
	}

	public override void OnRemoved() {
		foreach (var ability in abilities) {
			((Ability)ability).Shutdown();
			((EngineObject)ability).Dispose();
		}

		abilities.Clear();
		base.OnRemoved();
	}
}