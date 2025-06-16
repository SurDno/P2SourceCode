using Engine.Common;
using Engine.Impl.Services.HierarchyServices;

namespace Engine.Source.Commons
{
  public interface IEntityHierarchy
  {
    IEntity SceneEntity { get; }

    IEntity Parent { get; }

    HierarchyItem HierarchyItem { get; set; }

    void Add(IEntity entity);

    void Remove(IEntity entity);
  }
}
