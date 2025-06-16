// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.CrowdItemComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Inspectors;

#nullable disable
namespace Engine.Source.Components
{
  [Factory(typeof (ICrowdItemComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdItemComponent : EngineComponent, ICrowdItemComponent, IComponent
  {
    [Inspected]
    public IEntity Crowd { get; private set; }

    public AreaEnum Area => this.Point == null ? AreaEnum.Unknown : this.Point.Area;

    [Inspected]
    public PointInfo Point { get; private set; }

    public void AttachToCrowd(IEntity crowd, PointInfo point)
    {
      this.Crowd = crowd;
      this.Point = point;
    }

    public void DetachFromCrowd()
    {
      this.Crowd = (IEntity) null;
      this.Point = (PointInfo) null;
    }
  }
}
