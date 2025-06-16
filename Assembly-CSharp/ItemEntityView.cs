using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class ItemEntityView : EntityView {
	[SerializeField] private ItemView view;
	private IEntity entity;

	public override IEntity Value {
		get => entity;
		set {
			if (entity == value)
				return;
			entity = value;
			if (!(view != null))
				return;
			view.Storable = entity?.GetComponent<StorableComponent>();
		}
	}
}