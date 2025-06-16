using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public static class VMMath
  {
    private static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static int GetRandomInt(int maxIntVal = 2147483647) => VMMath.random.Next(maxIntVal);

    public static int GetRandomInt(int minIntVal, int maxIntVal)
    {
      return minIntVal + VMMath.random.Next(maxIntVal - minIntVal);
    }

    public static double GetRandomDouble() => VMMath.random.NextDouble();

    public static List<int> GetRandomIndexes(
      int randomMinIndex,
      int randomMaxIndex,
      int indexesCount)
    {
      List<int> randomIndexes1 = new List<int>(randomMaxIndex - randomMinIndex);
      for (int index = randomMinIndex; index < randomMaxIndex; ++index)
        randomIndexes1[index] = index;
      int count = randomIndexes1.Count;
      while (count > 1)
      {
        --count;
        int index = VMMath.random.Next(count);
        int num = randomIndexes1[index];
        randomIndexes1[index] = randomIndexes1[count];
        randomIndexes1[count] = num;
      }
      if (indexesCount >= randomMaxIndex - randomMinIndex)
        return randomIndexes1;
      List<int> randomIndexes2 = new List<int>(indexesCount);
      for (int index = 0; index < indexesCount; ++index)
        randomIndexes2.Add(randomIndexes1[index]);
      return randomIndexes2;
    }
  }
}
