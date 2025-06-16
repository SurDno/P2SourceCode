using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class EntityItemView : ItemView {
	[SerializeField] private EntityView view;

	public override StorableComponent Storable {
		get => view?.Value?.GetComponent<StorableComponent>();
		set {
			if (!(view != null))
				return;
			view.Value = value?.Owner;
		}
	}
}