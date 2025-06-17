using System.Collections.Generic;

namespace Externals.LipSync
{
  public class CustomAnnoReader
  {
    private List<PhonemeMixtureArticulationData> markers = [];

    public CustomAnnoReader(byte[] data) => LipsyncUtility.LoadAnnoFormatted(data, markers);

    public PhonemeMixtureArticulationData GetLipsyncAtTime(int ms)
    {
      int num1 = 0;
      int num2 = 33;
      for (int index = 0; index < markers.Count; ++index)
      {
        PhonemeMixtureArticulationData marker = markers[index];
        if (ms >= num1 && ms < num2)
          return marker;
        num1 = num2;
        num2 += 33;
        if (num2 % 100 == 99)
          ++num2;
      }
      return LipsyncUtility.DefaultPhoneme;
    }
  }
}
