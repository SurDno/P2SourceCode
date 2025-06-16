// Decompiled with JetBrains decompiler
// Type: Externals.LipSync.LipsyncUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Scripts.Utility;
using System.Collections.Generic;
using System.IO;

#nullable disable
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
        using (BinaryReader binaryReader = new BinaryReader((Stream) input))
        {
          while (true)
          {
            byte length = binaryReader.ReadByte();
            if (length != (byte) 0)
            {
              PhonemeMixtureArticulationData articulationData = new PhonemeMixtureArticulationData();
              articulationData.Constituents = new PhonemeData[(int) length];
              for (int index = 0; index < (int) length; ++index)
              {
                articulationData.Constituents[index].Phoneme = (int) binaryReader.ReadByte();
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
        new PhonemeData() { Phoneme = 0, Weight = 1f }
      };
      LipsyncUtility.DefaultPhoneme = articulationData;
      LipsyncUtility.Vowels = new HashSet<int>()
      {
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
