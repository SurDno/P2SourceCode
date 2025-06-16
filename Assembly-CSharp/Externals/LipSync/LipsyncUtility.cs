using System.Collections.Generic;
using System.IO;
using Scripts.Utility;

namespace Externals.LipSync
{
  public static class LipsyncUtility
  {
    private static readonly char[] splitSpace = new char[1]
    {
      ' '
    };
    public static readonly PhonemeMixtureArticulationData DefaultPhoneme;
    public static readonly HashSet<int> Vowels;

    public static void LoadAnnoFormatted(byte[] data, List<PhonemeMixtureArticulationData> markers)
    {
      data = CompressUtility.DecompressData(data);
      using (MemoryStream input = new MemoryStream(data))
      {
        using (BinaryReader binaryReader = new BinaryReader(input))
        {
          while (true)
          {
            byte length = binaryReader.ReadByte();
            if (length != 0)
            {
              PhonemeMixtureArticulationData articulationData = new PhonemeMixtureArticulationData();
              articulationData.Constituents = new PhonemeData[length];
              for (int index = 0; index < length; ++index)
              {
                articulationData.Constituents[index].Phoneme = binaryReader.ReadByte();
                articulationData.Constituents[index].Weight = binaryReader.ReadSingle();
              }
              markers.Add(articulationData);
            }
            else
              break;
          }
        }
      }
    }

    static LipsyncUtility()
    {
      PhonemeMixtureArticulationData articulationData = new PhonemeMixtureArticulationData();
      articulationData.Constituents = new PhonemeData[1]
      {
        new PhonemeData { Phoneme = 0, Weight = 1f }
      };
      DefaultPhoneme = articulationData;
      Vowels = new HashSet<int> {
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        14
      };
    }
  }
}
