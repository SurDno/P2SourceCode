using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class AnnoReader {
	private List<PhonemeMixtureArticulation> m_artMarkers = new();
	private static PhonemeMixtureArticulation m_default;

	private static readonly char[] splitSpace = new char[1] {
		' '
	};

	public AnnoReader() {
		if (m_default != null)
			return;
		m_default = new PhonemeMixtureArticulation();
		m_default.m_nConstituents = 1L;
		m_default.msStart = 0L;
		m_default.msEnd = 0L;
		m_default.m_constituents[0] = new t_phoneme();
		m_default.m_constituents[0].strPhoneme = new char[1] {
			'x'
		};
		m_default.m_constituents[0].weight = 1f;
	}

	public PhonemeMixtureArticulation GetLipsyncAtTime(int ms) {
		foreach (var artMarker in m_artMarkers)
			if (ms >= artMarker.msStart && ms < artMarker.msEnd)
				return artMarker;
		return m_default;
	}

	public void LoadAnnoFormatted(string sAnnoFormatted) {
		var stringReader = new StringReader(sAnnoFormatted);
		string str;
		while ((str = stringReader.ReadLine()) != null) {
			var strArray = str.Trim().Split(splitSpace);
			var mixtureArticulation = new PhonemeMixtureArticulation {
				msStart = int.Parse(strArray[1]),
				msEnd = int.Parse(strArray[2]),
				m_nConstituents = int.Parse(strArray[3])
			};
			mixtureArticulation.m_constituents = new t_phoneme[mixtureArticulation.m_nConstituents];
			for (var index = 0; index < mixtureArticulation.m_nConstituents; ++index) {
				mixtureArticulation.m_constituents[index] = new t_phoneme();
				mixtureArticulation.m_constituents[index].strPhoneme = strArray[4 + index * 2].ToCharArray();
				var s = strArray[5 + index * 2];
				mixtureArticulation.m_constituents[index].weight = float.Parse(s, CultureInfo.InvariantCulture);
			}

			m_artMarkers.Add(mixtureArticulation);
		}
	}
}