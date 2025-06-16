using System.Collections;
using System.Collections.Generic;

public class POIUtility {
	public static List<POIAnimationEnum> GetAnimationListFromAnimationEnum(POIAnimationEnum animations) {
		var bitArray = new BitArray(new int[1] {
			(int)animations
		});
		var fromAnimationEnum = new List<POIAnimationEnum>();
		for (var index = 0; index < bitArray.Length; ++index)
			if (bitArray[index])
				fromAnimationEnum.Add((POIAnimationEnum)(1 << index));
		return fromAnimationEnum;
	}
}