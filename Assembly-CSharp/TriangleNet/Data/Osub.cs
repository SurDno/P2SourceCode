namespace TriangleNet.Data;

internal struct Osub {
	public Segment seg;
	public int orient;

	public override string ToString() {
		return seg == null ? "O-TID [null]" : string.Format("O-SID {0}", seg.hash);
	}

	public void Sym(ref Osub o2) {
		o2.seg = seg;
		o2.orient = 1 - orient;
	}

	public void SymSelf() {
		orient = 1 - orient;
	}

	public void Pivot(ref Osub o2) {
		o2 = seg.subsegs[orient];
	}

	public void PivotSelf() {
		this = seg.subsegs[orient];
	}

	public void Next(ref Osub o2) {
		o2 = seg.subsegs[1 - orient];
	}

	public void NextSelf() {
		this = seg.subsegs[1 - orient];
	}

	public Vertex Org() {
		return seg.vertices[orient];
	}

	public Vertex Dest() {
		return seg.vertices[1 - orient];
	}

	public void SetOrg(Vertex ptr) {
		seg.vertices[orient] = ptr;
	}

	public void SetDest(Vertex ptr) {
		seg.vertices[1 - orient] = ptr;
	}

	public Vertex SegOrg() {
		return seg.vertices[2 + orient];
	}

	public Vertex SegDest() {
		return seg.vertices[3 - orient];
	}

	public void SetSegOrg(Vertex ptr) {
		seg.vertices[2 + orient] = ptr;
	}

	public void SetSegDest(Vertex ptr) {
		seg.vertices[3 - orient] = ptr;
	}

	public int Mark() {
		return seg.boundary;
	}

	public void SetMark(int value) {
		seg.boundary = value;
	}

	public void Bond(ref Osub o2) {
		seg.subsegs[orient] = o2;
		o2.seg.subsegs[o2.orient] = this;
	}

	public void Dissolve() {
		seg.subsegs[orient].seg = Mesh.dummysub;
	}

	public void Copy(ref Osub o2) {
		o2.seg = seg;
		o2.orient = orient;
	}

	public bool Equal(Osub o2) {
		return seg == o2.seg && orient == o2.orient;
	}

	public static bool IsDead(Segment sub) {
		return sub.subsegs[0].seg == null;
	}

	public static void Kill(Segment sub) {
		sub.subsegs[0].seg = null;
		sub.subsegs[1].seg = null;
	}

	public void TriPivot(ref Otri ot) {
		ot = seg.triangles[orient];
	}

	public void TriDissolve() {
		seg.triangles[orient].triangle = Mesh.dummytri;
	}
}