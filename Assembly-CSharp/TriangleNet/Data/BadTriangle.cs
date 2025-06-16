namespace TriangleNet.Data;

internal class BadTriangle {
	public static int OTID;
	public int ID;
	public double key;
	public BadTriangle nexttriang;
	public Otri poortri;
	public Vertex triangorg;
	public Vertex triangdest;
	public Vertex triangapex;

	public BadTriangle() {
		ID = OTID++;
	}

	public override string ToString() {
		return string.Format("B-TID {0}", poortri.triangle.hash);
	}
}