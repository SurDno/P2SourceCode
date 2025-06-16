namespace TriangleNet.Data;

internal class BadSubseg {
	private static int hashSeed;
	public Osub encsubseg;
	internal int Hash;
	public Vertex subsegorg;
	public Vertex subsegdest;

	public BadSubseg() {
		Hash = hashSeed++;
	}

	public override int GetHashCode() {
		return Hash;
	}

	public override string ToString() {
		return string.Format("B-SID {0}", encsubseg.seg.hash);
	}
}