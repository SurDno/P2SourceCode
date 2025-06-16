using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Inspectors;

namespace Engine.Source.Commons
{
  public abstract class EngineComponent : IComponent, IInjectable, IEngineComponent
  {
    [Inspected(Header = true)]
    private string OwnerHierarchyPath => this.Owner.GetHierarchyPath();

    [Inspected]
    public bool IsDisposed => this.Owner == null || this.Owner.IsDisposed;

    [Inspected]
    public IEntity Owner { get; set; }

    public virtual void OnChangeEnabled()
    {
    }

    public virtual void PrepareAdded()
    {
      MetaService.Compute((object) this, FromThisAttribute.Id, (object) this);
      MetaService.Compute((object) this, FromLocatorAttribute.Id, (object) this);
    }

    public virtual void OnAdded()
    {
    }

    public virtual void OnRemoved()
    {
    }

    public virtual void PostRemoved()
    {
      MetaService.Compute((object) this, FromThisAttribute.ClearId, (object) this);
    }
  }
}
