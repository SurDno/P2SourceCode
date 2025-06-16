// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.ICrowdPointsComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Movable;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public interface ICrowdPointsComponent : IComponent
  {
    IEnumerable<CrowdPointInfo> Points { get; }

    void GetEnabledPoints(AreaEnum area, int count, List<CrowdPointInfo> result);
  }
}
