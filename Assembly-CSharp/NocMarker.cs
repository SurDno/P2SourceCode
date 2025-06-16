public class NocMarker {
	public int msStart;
	public int msEnd;
	public int weight;

	public NocMarker() {
		msStart = msEnd = 0;
		weight = 50;
	}

	public int getDuration() {
		return msEnd - msStart;
	}

	public int getEndMs() {
		return msEnd;
	}

	public int getStartMs() {
		return msStart;
	}

	public float getEnergy() {
		return weight;
	}
}