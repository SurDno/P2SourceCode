using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public static class VMMath {
	private static Random random = new(Guid.NewGuid().GetHashCode());

	public static int GetRandomInt(int maxIntVal = 2147483647) {
		return random.Next(maxIntVal);
	}

	public static int GetRandomInt(int minIntVal, int maxIntVal) {
		return minIntVal + random.Next(maxIntVal - minIntVal);
	}

	public static double GetRandomDouble() {
		return random.NextDouble();
	}

	public static List<int> GetRandomIndexes(
		int randomMinIndex,
		int randomMaxIndex,
		int indexesCount) {
		var randomIndexes1 = new List<int>(randomMaxIndex - randomMinIndex);
		for (var index = randomMinIndex; index < randomMaxIndex; ++index)
			randomIndexes1[index] = index;
		var count = randomIndexes1.Count;
		while (count > 1) {
			--count;
			var index = random.Next(count);
			var num = randomIndexes1[index];
			randomIndexes1[index] = randomIndexes1[count];
			randomIndexes1[count] = num;
		}

		if (indexesCount >= randomMaxIndex - randomMinIndex)
			return randomIndexes1;
		var randomIndexes2 = new List<int>(indexesCount);
		for (var index = 0; index < indexesCount; ++index)
			randomIndexes2.Add(randomIndexes1[index]);
		return randomIndexes2;
	}
}