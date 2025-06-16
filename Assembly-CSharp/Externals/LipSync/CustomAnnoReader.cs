// Decompiled with JetBrains decompiler
// Type: Externals.LipSync.CustomAnnoReader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Externals.LipSync
{
  public class CustomAnnoReader
  {
    private List<PhonemeMixtureArticulationData> markers = new List<PhonemeMixtureArticulationData>();

    public CustomAnnoReader(byte[] data) => LipsyncUtility.LoadAnnoFormatted(data, this.markers);

    public PhonemeMixtureArticulationData GetLipsyncAtTime(int ms)
    {
      int num1 = 0;
      int num2 = 33;
      for (int index = 0; index < this.markers.Count; ++index)
      {
        PhonemeMixtureArticulationData marker = this.markers[index];
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
