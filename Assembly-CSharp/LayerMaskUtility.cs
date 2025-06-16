// Decompiled with JetBrains decompiler
// Type: LayerMaskUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class LayerMaskUtility
{
  public static bool Contains(this LayerMask mask, int layer) => (mask.value & 1 << layer) != 0;

  public static int GetMask(this LayerMask mask) => 1 << (int) mask;

  public static int GetIndex(this LayerMask mask)
  {
    int index = 0;
    int num = mask.value;
    while (num > 1)
    {
      num >>= 1;
      ++index;
    }
    return index;
  }
}
