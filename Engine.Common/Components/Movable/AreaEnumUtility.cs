// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Movable.AreaEnumUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Components.Movable
{
  public static class AreaEnumUtility
  {
    public static int ToMask(this AreaEnum area)
    {
      switch (area)
      {
        case AreaEnum.All:
          return AreaEnum.Road.ToMask() | AreaEnum.FootPath.ToMask() | AreaEnum.Walkable.ToMask() | AreaEnum.Outdoor.ToMask();
        case AreaEnum.RoadFootPath:
          return AreaEnum.Road.ToMask() | AreaEnum.FootPath.ToMask();
        case AreaEnum.RoadFootPathWalkable:
          return AreaEnum.Road.ToMask() | AreaEnum.FootPath.ToMask() | AreaEnum.Walkable.ToMask();
        default:
          return area > AreaEnum.Unknown && area < AreaEnum.All ? 1 << (int) (area - 1 & (AreaEnum) 31) : 0;
      }
    }

    public static int ToMask(this IEnumerable<AreaEnum> areas)
    {
      int mask = 0;
      foreach (AreaEnum area in areas)
        mask |= area.ToMask();
      return mask;
    }

    public static AreaEnum ToArea(int mask)
    {
      for (int index = 0; index < 32; ++index)
      {
        if ((mask & 1 << index) != 0)
          return (AreaEnum) (index + 1);
      }
      return AreaEnum.Unknown;
    }
  }
}
