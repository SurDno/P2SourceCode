using System;
using TriangleNet.Log;

namespace TriangleNet;

public class Behavior {
	private TriangulationAlgorithm algorithm = TriangulationAlgorithm.Dwyer;
	private bool boundaryMarkers = true;
	private bool conformDel;
	private bool convex;
	internal bool fixedArea;
	internal double goodAngle;
	private bool jettison;
	private double maxAngle;
	private double maxArea = -1.0;
	internal double maxGoodAngle;
	private double minAngle;
	private int noBisect;
	private bool noHoles;
	internal double offconstant;
	private bool poly;
	private bool quality;
	private int steiner = -1;
	internal bool useRegions = false;
	private bool usertest;
	internal bool useSegments = true;
	private bool varArea;

	public static bool NoExact { get; set; }

	public static bool Verbose { get; set; }

	public bool Quality {
		get => quality;
		set {
			quality = value;
			if (!quality)
				return;
			Update();
		}
	}

	public double MinAngle {
		get => minAngle;
		set {
			minAngle = value;
			Update();
		}
	}

	public double MaxAngle {
		get => maxAngle;
		set {
			maxAngle = value;
			Update();
		}
	}

	public double MaxArea {
		get => maxArea;
		set {
			maxArea = value;
			fixedArea = value > 0.0;
		}
	}

	public bool VarArea {
		get => varArea;
		set => varArea = value;
	}

	public bool Poly {
		get => poly;
		set => poly = value;
	}

	public bool Usertest {
		get => usertest;
		set => usertest = value;
	}

	public bool Convex {
		get => convex;
		set => convex = value;
	}

	public bool ConformingDelaunay {
		get => conformDel;
		set => conformDel = value;
	}

	public TriangulationAlgorithm Algorithm {
		get => algorithm;
		set => algorithm = value;
	}

	public int NoBisect {
		get => noBisect;
		set {
			noBisect = value;
			if (noBisect >= 0 && noBisect <= 2)
				return;
			noBisect = 0;
		}
	}

	public int SteinerPoints {
		get => steiner;
		set => steiner = value;
	}

	public bool UseBoundaryMarkers {
		get => boundaryMarkers;
		set => boundaryMarkers = value;
	}

	public bool NoHoles {
		get => noHoles;
		set => noHoles = value;
	}

	public bool Jettison {
		get => jettison;
		set => jettison = value;
	}

	public Behavior(bool quality = false, double minAngle = 20.0) {
		if (!quality)
			return;
		this.quality = true;
		this.minAngle = minAngle;
		Update();
	}

	private void Update() {
		quality = true;
		if (minAngle < 0.0 || minAngle > 60.0) {
			minAngle = 0.0;
			quality = false;
			SimpleLog.Instance.Warning("Invalid quality option (minimum angle).", "Mesh.Behavior");
		}

		if ((maxAngle != 0.0 && maxAngle < 90.0) || maxAngle > 180.0) {
			maxAngle = 0.0;
			quality = false;
			SimpleLog.Instance.Warning("Invalid quality option (maximum angle).", "Mesh.Behavior");
		}

		useSegments = Poly || Quality || Convex;
		goodAngle = Math.Cos(MinAngle * Math.PI / 180.0);
		maxGoodAngle = Math.Cos(MaxAngle * Math.PI / 180.0);
		offconstant = goodAngle != 1.0 ? 0.475 * Math.Sqrt((1.0 + goodAngle) / (1.0 - goodAngle)) : 0.0;
		goodAngle *= goodAngle;
	}
}