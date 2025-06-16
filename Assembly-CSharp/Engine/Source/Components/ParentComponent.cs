using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;

namespace Engine.Source.Components
{
  [Factory(typeof (ParentComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ParentComponent : EngineComponent
  {
    private IEntity parent;
    private IEntity rootParent;

    public void SetParent(IEntity parent)
    {
      this.parent = parent;
      rootParent = FindRootParent();
    }

    private IEntity FindRootParent()
    {
      if (parent == null)
        return Owner;
      ParentComponent component = parent.GetComponent<ParentComponent>();
      return component == null ? parent : component.GetRootParent();
    }

    public IEntity GetParent() => parent;

    public IEntity GetRootParent() => rootParent;
  }
}
