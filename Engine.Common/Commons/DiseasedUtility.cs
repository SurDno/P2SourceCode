// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.DiseasedUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Commons
{
  public static class DiseasedUtility
  {
    public static DiseasedStateEnum GetStateByLevel(int diseaseLevel)
    {
      if (diseaseLevel < 0)
        return DiseasedStateEnum.None;
      if (diseaseLevel < 1)
        return DiseasedStateEnum.Normal;
      if (diseaseLevel < 3)
        return DiseasedStateEnum.Diseased;
      if (diseaseLevel < 4)
        return DiseasedStateEnum.Blocked;
      return diseaseLevel < 5 ? DiseasedStateEnum.Shelter : DiseasedStateEnum.None;
    }
  }
}
