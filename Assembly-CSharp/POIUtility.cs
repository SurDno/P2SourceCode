using System.Collections;
using System.Collections.Generic;

public class POIUtility
{
  public static List<POIAnimationEnum> GetAnimationListFromAnimationEnum(POIAnimationEnum animations)
  {
    BitArray bitArray = new BitArray([
      (int) animations
    ]);
    List<POIAnimationEnum> fromAnimationEnum = [];
    for (int index = 0; index < bitArray.Length; ++index)
    {
      if (bitArray[index])
        fromAnimationEnum.Add((POIAnimationEnum) (1 << index));
    }
    return fromAnimationEnum;
  }
}
