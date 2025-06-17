using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class AnnoReader
{
  private List<PhonemeMixtureArticulation> m_artMarkers = [];
  private static PhonemeMixtureArticulation m_default;
  private static readonly char[] splitSpace = [
    ' '
  ];

  public AnnoReader()
  {
    if (m_default != null)
      return;
    m_default = new PhonemeMixtureArticulation();
    m_default.m_nConstituents = 1L;
    m_default.msStart = 0L;
    m_default.msEnd = 0L;
    m_default.m_constituents[0] = new t_phoneme();
    m_default.m_constituents[0].strPhoneme = [
      'x'
    ];
    m_default.m_constituents[0].weight = 1f;
  }

  public PhonemeMixtureArticulation GetLipsyncAtTime(int ms)
  {
    foreach (PhonemeMixtureArticulation artMarker in m_artMarkers)
    {
      if (ms >= artMarker.msStart && ms < artMarker.msEnd)
        return artMarker;
    }
    return m_default;
  }

  public void LoadAnnoFormatted(string sAnnoFormatted)
  {
    StringReader stringReader = new StringReader(sAnnoFormatted);
    string str;
    while ((str = stringReader.ReadLine()) != null)
    {
      string[] strArray = str.Trim().Split(splitSpace);
      PhonemeMixtureArticulation mixtureArticulation = new PhonemeMixtureArticulation {
        msStart = int.Parse(strArray[1]),
        msEnd = int.Parse(strArray[2]),
        m_nConstituents = int.Parse(strArray[3])
      };
      mixtureArticulation.m_constituents = new t_phoneme[mixtureArticulation.m_nConstituents];
      for (int index = 0; index < mixtureArticulation.m_nConstituents; ++index)
      {
        mixtureArticulation.m_constituents[index] = new t_phoneme();
        mixtureArticulation.m_constituents[index].strPhoneme = strArray[4 + index * 2].ToCharArray();
        string s = strArray[5 + index * 2];
        mixtureArticulation.m_constituents[index].weight = float.Parse(s, CultureInfo.InvariantCulture);
      }
      m_artMarkers.Add(mixtureArticulation);
    }
  }
}
