// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.ParentComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;

#nullable disable
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
      this.rootParent = this.FindRootParent();
    }

    private IEntity FindRootParent()
    {
      if (this.parent == null)
        return this.Owner;
      ParentComponent component = this.parent.GetComponent<ParentComponent>();
      return component == null ? this.parent : component.GetRootParent();
    }

    public IEntity GetParent() => this.parent;

    public IEntity GetRootParent() => this.rootParent;
  }
}
