using Engine.Common;
using Inspectors;

namespace Engine.Impl.UI.Controls;

public abstract class EntityViewBase : EntityView {
	[Inspected] private IEntity value;

	public override IEntity Value {
		get => value;
		set {
			if (this.value == value)
				return;
			this.value = value;
			ApplyValue();
		}
	}

	protected abstract void ApplyValue();
}