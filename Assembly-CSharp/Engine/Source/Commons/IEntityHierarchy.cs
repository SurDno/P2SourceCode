// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.IEntityHierarchy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Impl.Services.HierarchyServices;

#nullable disable
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
