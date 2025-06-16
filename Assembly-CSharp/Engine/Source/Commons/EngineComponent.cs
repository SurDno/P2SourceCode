// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.EngineComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Inspectors;

#nullable disable
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
