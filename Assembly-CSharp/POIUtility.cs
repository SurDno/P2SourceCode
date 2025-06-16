// Decompiled with JetBrains decompiler
// Type: POIUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
public class POIUtility
{
  public static List<POIAnimationEnum> GetAnimationListFromAnimationEnum(POIAnimationEnum animations)
  {
    BitArray bitArray = new BitArray(new int[1]
    {
      (int) animations
    });
    List<POIAnimationEnum> fromAnimationEnum = new List<POIAnimationEnum>();
    for (int index = 0; index < bitArray.Length; ++index)
    {
      if (bitArray[index])
        fromAnimationEnum.Add((POIAnimationEnum) (1 << index));
    }
    return fromAnimationEnum;
  }
}
