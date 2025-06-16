using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Inspectors;

namespace Engine.Source.Commons;

public abstract class EngineComponent : IComponent, IInjectable, IEngineComponent {
	[Inspected(Header = true)] private string OwnerHierarchyPath => Owner.GetHierarchyPath();

	[Inspected] public bool IsDisposed => Owner == null || Owner.IsDisposed;

	[Inspected] public IEntity Owner { get; set; }

	public virtual void OnChangeEnabled() { }

	public virtual void PrepareAdded() {
		MetaService.Compute(this, FromThisAttribute.Id, this);
		MetaService.Compute(this, FromLocatorAttribute.Id, this);
	}

	public virtual void OnAdded() { }

	public virtual void OnRemoved() { }

	public virtual void PostRemoved() {
		MetaService.Compute(this, FromThisAttribute.ClearId, this);
	}
}