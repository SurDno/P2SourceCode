using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class AnnoReader
{
  private List<PhonemeMixtureArticulation> m_artMarkers = new List<PhonemeMixtureArticulation>();
  private static PhonemeMixtureArticulation m_default;
  private static readonly char[] splitSpace = new char[1]
  {
    ' '
  };

  public AnnoReader()
  {
    if (AnnoReader.m_default != null)
      return;
    AnnoReader.m_default = new PhonemeMixtureArticulation();
    AnnoReader.m_default.m_nConstituents = 1L;
    AnnoReader.m_default.msStart = 0L;
    AnnoReader.m_default.msEnd = 0L;
    AnnoReader.m_default.m_constituents[0] = new t_phoneme();
    AnnoReader.m_default.m_constituents[0].strPhoneme = new char[1]
    {
      'x'
    };
    AnnoReader.m_default.m_constituents[0].weight = 1f;
  }

  public PhonemeMixtureArticulation GetLipsyncAtTime(int ms)
  {
    foreach (PhonemeMixtureArticulation artMarker in this.m_artMarkers)
    {
      if ((long) ms >= artMarker.msStart && (long) ms < artMarker.msEnd)
        return artMarker;
    }
    return AnnoReader.m_default;
  }

  public void LoadAnnoFormatted(string sAnnoFormatted)
  {
    StringReader stringReader = new StringReader(sAnnoFormatted);
    string str;
    while ((str = stringReader.ReadLine()) != null)
    {
      string[] strArray = str.Trim().Split(AnnoReader.splitSpace);
      PhonemeMixtureArticulation mixtureArticulation = new PhonemeMixtureArticulation()
      {
        msStart = (long) int.Parse(strArray[1]),
        msEnd = (long) int.Parse(strArray[2]),
        m_nConstituents = (long) int.Parse(strArray[3])
      };
      mixtureArticulation.m_constituents = new t_phoneme[mixtureArticulation.m_nConstituents];
      for (int index = 0; (long) index < mixtureArticulation.m_nConstituents; ++index)
      {
        mixtureArticulation.m_constituents[index] = new t_phoneme();
        mixtureArticulation.m_constituents[index].strPhoneme = strArray[4 + index * 2].ToCharArray();
        string s = strArray[5 + index * 2];
        mixtureArticulation.m_constituents[index].weight = float.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      this.m_artMarkers.Add(mixtureArticulation);
    }
  }
}
