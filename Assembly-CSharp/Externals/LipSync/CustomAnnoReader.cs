using System.Collections.Generic;

namespace Externals.LipSync;

public class CustomAnnoReader {
	private List<PhonemeMixtureArticulationData> markers = new();

	public CustomAnnoReader(byte[] data) {
		LipsyncUtility.LoadAnnoFormatted(data, markers);
	}

	public PhonemeMixtureArticulationData GetLipsyncAtTime(int ms) {
		var num1 = 0;
		var num2 = 33;
		for (var index = 0; index < markers.Count; ++index) {
			var marker = markers[index];
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