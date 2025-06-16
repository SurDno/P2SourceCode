using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Selectors;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Required(typeof(LocationItemComponent))]
[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class SelectorComponent : EngineComponent {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<SelectorPreset> presets = new();

	private bool initialise;
	[FromThis] private LocationItemComponent locationItemComponent;

	public override void OnAdded() {
		base.OnAdded();
		locationItemComponent.OnHibernationChanged += LocationItemComponent_OnChangeHibernation;
	}

	public override void OnRemoved() {
		locationItemComponent.OnHibernationChanged -= LocationItemComponent_OnChangeHibernation;
		base.OnRemoved();
	}

	private void LocationItemComponent_OnChangeHibernation(ILocationItemComponent sender) {
		if (initialise || locationItemComponent.IsHibernation)
			return;
		initialise = true;
		if (presets.Count == 0)
			return;
		var sceneEntity = ((IEntityHierarchy)Owner).SceneEntity;
		if (sceneEntity == null)
			Debug.LogError("SceneEntity not found : " + Owner.GetInfo());
		else {
			var num = Random.Range(0, presets.Count);
			for (var index = 0; index < presets.Count; ++index)
				foreach (var sceneGameObject in presets[index].Objects) {
					var entityByTemplate = EntityUtility.GetEntityByTemplate(sceneEntity, sceneGameObject.Id);
					if (entityByTemplate != null)
						entityByTemplate.IsEnabled = num == index;
					else
						Debug.LogError("SelectorComponent - EntityByTemplate not found , id : " + sceneGameObject.Id +
						               " , owner : " + Owner.GetInfo());
				}
		}
	}
}