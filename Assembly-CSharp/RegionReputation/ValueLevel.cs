// Decompiled with JetBrains decompiler
// Type: RegionReputation.ValueLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace RegionReputation
{
  [Serializable]
  public struct ValueLevel
  {
    public string Signature;
    public float Threshold;

    public static string GetSignature(ValueLevel[] levels, float value, bool greater = false)
    {
      for (int index = 0; index < levels.Length; ++index)
      {
        if (greater ? (double) value > (double) levels[index].Threshold : (double) value < (double) levels[index].Threshold)
          return levels[index].Signature;
      }
      return (string) null;
    }
  }
}
