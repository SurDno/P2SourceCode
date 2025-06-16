using System;
using System.Collections.Generic;

namespace ClipperLib;

public class ClipperOffset {
	private const double two_pi = 6.2831853071795862;
	private const double def_arc_tolerance = 0.25;
	private double m_delta;
	private double m_sinA;
	private double m_sin;
	private double m_cos;
	private List<IntPoint> m_destPoly;
	private List<List<IntPoint>> m_destPolys;
	private IntPoint m_lowest;
	private double m_miterLim;
	private double m_StepsPerRad;
	private List<DoublePoint> m_normals = new();
	private PolyNode m_polyNodes = new();
	private List<IntPoint> m_srcPoly;

	public double ArcTolerance { get; set; }

	public double MiterLimit { get; set; }

	public ClipperOffset(double miterLimit = 2.0, double arcTolerance = 0.25) {
		MiterLimit = miterLimit;
		ArcTolerance = arcTolerance;
		m_lowest.X = -1L;
	}

	public void Clear() {
		m_polyNodes.Childs.Clear();
		m_lowest.X = -1L;
	}

	internal static long Round(double value) {
		return value < 0.0 ? (long)(value - 0.5) : (long)(value + 0.5);
	}

	public void AddPath(List<IntPoint> path, JoinType joinType, EndType endType) {
		var index1 = path.Count - 1;
		if (index1 < 0)
			return;
		var Child = new PolyNode();
		Child.m_jointype = joinType;
		Child.m_endtype = endType;
		if (endType == EndType.etClosedLine || endType == EndType.etClosedPolygon)
			while (index1 > 0 && path[0] == path[index1])
				--index1;
		Child.m_polygon.Capacity = index1 + 1;
		Child.m_polygon.Add(path[0]);
		var index2 = 0;
		var num = 0;
		for (var index3 = 1; index3 <= index1; ++index3)
			if (Child.m_polygon[index2] != path[index3]) {
				++index2;
				Child.m_polygon.Add(path[index3]);
				if (path[index3].Y > Child.m_polygon[num].Y || (path[index3].Y == Child.m_polygon[num].Y &&
				                                                path[index3].X < Child.m_polygon[num].X))
					num = index2;
			}

		if (endType == EndType.etClosedPolygon && index2 < 2)
			return;
		m_polyNodes.AddChild(Child);
		if (endType != 0)
			return;
		if (m_lowest.X < 0L)
			m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, num);
		else {
			var intPoint = m_polyNodes.Childs[(int)m_lowest.X].m_polygon[(int)m_lowest.Y];
			if (Child.m_polygon[num].Y > intPoint.Y ||
			    (Child.m_polygon[num].Y == intPoint.Y && Child.m_polygon[num].X < intPoint.X))
				m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, num);
		}
	}

	public void AddPaths(List<List<IntPoint>> paths, JoinType joinType, EndType endType) {
		foreach (var path in paths)
			AddPath(path, joinType, endType);
	}

	private void FixOrientations() {
		if (m_lowest.X >= 0L && !Clipper.Orientation(m_polyNodes.Childs[(int)m_lowest.X].m_polygon))
			for (var index = 0; index < m_polyNodes.ChildCount; ++index) {
				var child = m_polyNodes.Childs[index];
				if (child.m_endtype == EndType.etClosedPolygon ||
				    (child.m_endtype == EndType.etClosedLine && Clipper.Orientation(child.m_polygon)))
					child.m_polygon.Reverse();
			}
		else
			for (var index = 0; index < m_polyNodes.ChildCount; ++index) {
				var child = m_polyNodes.Childs[index];
				if (child.m_endtype == EndType.etClosedLine && !Clipper.Orientation(child.m_polygon))
					child.m_polygon.Reverse();
			}
	}

	internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2) {
		double num1 = pt2.X - pt1.X;
		double num2 = pt2.Y - pt1.Y;
		if (num1 == 0.0 && num2 == 0.0)
			return new DoublePoint();
		var num3 = 1.0 / Math.Sqrt(num1 * num1 + num2 * num2);
		var num4 = num1 * num3;
		return new DoublePoint(num2 * num3, -num4);
	}

	private void DoOffset(double delta) {
		m_destPolys = new List<List<IntPoint>>();
		m_delta = delta;
		if (ClipperBase.near_zero(delta)) {
			m_destPolys.Capacity = m_polyNodes.ChildCount;
			for (var index = 0; index < m_polyNodes.ChildCount; ++index) {
				var child = m_polyNodes.Childs[index];
				if (child.m_endtype == EndType.etClosedPolygon)
					m_destPolys.Add(child.m_polygon);
			}
		} else {
			m_miterLim = MiterLimit <= 2.0 ? 0.5 : 2.0 / (MiterLimit * MiterLimit);
			var num1 = Math.PI / Math.Acos(1.0 -
			                               (ArcTolerance > 0.0
				                               ? ArcTolerance <= Math.Abs(delta) * 0.25
					                               ? ArcTolerance
					                               : Math.Abs(delta) * 0.25
				                               : 0.25) / Math.Abs(delta));
			m_sin = Math.Sin(2.0 * Math.PI / num1);
			m_cos = Math.Cos(2.0 * Math.PI / num1);
			m_StepsPerRad = num1 / (2.0 * Math.PI);
			if (delta < 0.0)
				m_sin = -m_sin;
			m_destPolys.Capacity = m_polyNodes.ChildCount * 2;
			for (var index1 = 0; index1 < m_polyNodes.ChildCount; ++index1) {
				var child = m_polyNodes.Childs[index1];
				m_srcPoly = child.m_polygon;
				var count = m_srcPoly.Count;
				if (count != 0 && (delta > 0.0 || (count >= 3 && child.m_endtype == 0))) {
					m_destPoly = new List<IntPoint>();
					if (count == 1) {
						if (child.m_jointype == JoinType.jtRound) {
							var num2 = 1.0;
							var num3 = 0.0;
							for (var index2 = 1; index2 <= num1; ++index2) {
								m_destPoly.Add(new IntPoint(Round(m_srcPoly[0].X + num2 * delta),
									Round(m_srcPoly[0].Y + num3 * delta)));
								var num4 = num2;
								num2 = num2 * m_cos - m_sin * num3;
								num3 = num4 * m_sin + num3 * m_cos;
							}
						} else {
							var num5 = -1.0;
							var num6 = -1.0;
							for (var index3 = 0; index3 < 4; ++index3) {
								m_destPoly.Add(new IntPoint(Round(m_srcPoly[0].X + num5 * delta),
									Round(m_srcPoly[0].Y + num6 * delta)));
								if (num5 < 0.0)
									num5 = 1.0;
								else if (num6 < 0.0)
									num6 = 1.0;
								else
									num5 = -1.0;
							}
						}

						m_destPolys.Add(m_destPoly);
					} else {
						m_normals.Clear();
						m_normals.Capacity = count;
						for (var index4 = 0; index4 < count - 1; ++index4)
							m_normals.Add(GetUnitNormal(m_srcPoly[index4], m_srcPoly[index4 + 1]));
						if (child.m_endtype == EndType.etClosedLine || child.m_endtype == EndType.etClosedPolygon)
							m_normals.Add(GetUnitNormal(m_srcPoly[count - 1], m_srcPoly[0]));
						else
							m_normals.Add(new DoublePoint(m_normals[count - 2]));
						if (child.m_endtype == EndType.etClosedPolygon) {
							var k = count - 1;
							for (var j = 0; j < count; ++j)
								OffsetPoint(j, ref k, child.m_jointype);
							m_destPolys.Add(m_destPoly);
						} else if (child.m_endtype == EndType.etClosedLine) {
							var k1 = count - 1;
							for (var j = 0; j < count; ++j)
								OffsetPoint(j, ref k1, child.m_jointype);
							m_destPolys.Add(m_destPoly);
							m_destPoly = new List<IntPoint>();
							var normal = m_normals[count - 1];
							for (var index5 = count - 1; index5 > 0; --index5)
								m_normals[index5] = new DoublePoint(-m_normals[index5 - 1].X, -m_normals[index5 - 1].Y);
							m_normals[0] = new DoublePoint(-normal.X, -normal.Y);
							var k2 = 0;
							for (var j = count - 1; j >= 0; --j)
								OffsetPoint(j, ref k2, child.m_jointype);
							m_destPolys.Add(m_destPoly);
						} else {
							var k3 = 0;
							for (var j = 1; j < count - 1; ++j)
								OffsetPoint(j, ref k3, child.m_jointype);
							IntPoint intPoint;
							if (child.m_endtype == EndType.etOpenButt) {
								var index6 = count - 1;
								intPoint = new IntPoint(Round(m_srcPoly[index6].X + m_normals[index6].X * delta),
									Round(m_srcPoly[index6].Y + m_normals[index6].Y * delta));
								m_destPoly.Add(intPoint);
								intPoint = new IntPoint(Round(m_srcPoly[index6].X - m_normals[index6].X * delta),
									Round(m_srcPoly[index6].Y - m_normals[index6].Y * delta));
								m_destPoly.Add(intPoint);
							} else {
								var num7 = count - 1;
								var k4 = count - 2;
								m_sinA = 0.0;
								m_normals[num7] = new DoublePoint(-m_normals[num7].X, -m_normals[num7].Y);
								if (child.m_endtype == EndType.etOpenSquare)
									DoSquare(num7, k4);
								else
									DoRound(num7, k4);
							}

							for (var index7 = count - 1; index7 > 0; --index7)
								m_normals[index7] = new DoublePoint(-m_normals[index7 - 1].X, -m_normals[index7 - 1].Y);
							m_normals[0] = new DoublePoint(-m_normals[1].X, -m_normals[1].Y);
							var k5 = count - 1;
							for (var j = k5 - 1; j > 0; --j)
								OffsetPoint(j, ref k5, child.m_jointype);
							if (child.m_endtype == EndType.etOpenButt) {
								intPoint = new IntPoint(Round(m_srcPoly[0].X - m_normals[0].X * delta),
									Round(m_srcPoly[0].Y - m_normals[0].Y * delta));
								m_destPoly.Add(intPoint);
								intPoint = new IntPoint(Round(m_srcPoly[0].X + m_normals[0].X * delta),
									Round(m_srcPoly[0].Y + m_normals[0].Y * delta));
								m_destPoly.Add(intPoint);
							} else {
								m_sinA = 0.0;
								if (child.m_endtype == EndType.etOpenSquare)
									DoSquare(0, 1);
								else
									DoRound(0, 1);
							}

							m_destPolys.Add(m_destPoly);
						}
					}
				}
			}
		}
	}

	public void Execute(ref List<List<IntPoint>> solution, double delta) {
		solution.Clear();
		FixOrientations();
		DoOffset(delta);
		var clipper = new Clipper();
		clipper.AddPaths(m_destPolys, PolyType.ptSubject, true);
		if (delta > 0.0)
			clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
		else {
			var bounds = ClipperBase.GetBounds(m_destPolys);
			clipper.AddPath(new List<IntPoint>(4) {
				new(bounds.left - 10L, bounds.bottom + 10L),
				new(bounds.right + 10L, bounds.bottom + 10L),
				new(bounds.right + 10L, bounds.top - 10L),
				new(bounds.left - 10L, bounds.top - 10L)
			}, PolyType.ptSubject, true);
			clipper.ReverseSolution = true;
			clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
			if (solution.Count > 0)
				solution.RemoveAt(0);
		}
	}

	public void Execute(ref PolyTree solution, double delta) {
		solution.Clear();
		FixOrientations();
		DoOffset(delta);
		var clipper = new Clipper();
		clipper.AddPaths(m_destPolys, PolyType.ptSubject, true);
		if (delta > 0.0)
			clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
		else {
			var bounds = ClipperBase.GetBounds(m_destPolys);
			clipper.AddPath(new List<IntPoint>(4) {
				new(bounds.left - 10L, bounds.bottom + 10L),
				new(bounds.right + 10L, bounds.bottom + 10L),
				new(bounds.right + 10L, bounds.top - 10L),
				new(bounds.left - 10L, bounds.top - 10L)
			}, PolyType.ptSubject, true);
			clipper.ReverseSolution = true;
			clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
			if (solution.ChildCount == 1 && solution.Childs[0].ChildCount > 0) {
				var child = solution.Childs[0];
				solution.Childs.Capacity = child.ChildCount;
				solution.Childs[0] = child.Childs[0];
				solution.Childs[0].m_Parent = solution;
				for (var index = 1; index < child.ChildCount; ++index)
					solution.AddChild(child.Childs[index]);
			} else
				solution.Clear();
		}
	}

	private void OffsetPoint(int j, ref int k, JoinType jointype) {
		m_sinA = m_normals[k].X * m_normals[j].Y - m_normals[j].X * m_normals[k].Y;
		if (Math.Abs(m_sinA * m_delta) < 1.0) {
			if (m_normals[k].X * m_normals[j].X + m_normals[j].Y * m_normals[k].Y > 0.0) {
				m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[k].X * m_delta),
					Round(m_srcPoly[j].Y + m_normals[k].Y * m_delta)));
				return;
			}
		} else if (m_sinA > 1.0)
			m_sinA = 1.0;
		else if (m_sinA < -1.0)
			m_sinA = -1.0;

		if (m_sinA * m_delta < 0.0) {
			m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[k].X * m_delta),
				Round(m_srcPoly[j].Y + m_normals[k].Y * m_delta)));
			m_destPoly.Add(m_srcPoly[j]);
			m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[j].X * m_delta),
				Round(m_srcPoly[j].Y + m_normals[j].Y * m_delta)));
		} else
			switch (jointype) {
				case JoinType.jtSquare:
					DoSquare(j, k);
					break;
				case JoinType.jtRound:
					DoRound(j, k);
					break;
				case JoinType.jtMiter:
					var r = 1.0 + (m_normals[j].X * m_normals[k].X + m_normals[j].Y * m_normals[k].Y);
					if (r >= m_miterLim) {
						DoMiter(j, k, r);
						break;
					}

					DoSquare(j, k);
					break;
			}

		k = j;
	}

	internal void DoSquare(int j, int k) {
		var num = Math.Tan(Math.Atan2(m_sinA, m_normals[k].X * m_normals[j].X + m_normals[k].Y * m_normals[j].Y) / 4.0);
		m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_delta * (m_normals[k].X - m_normals[k].Y * num)),
			Round(m_srcPoly[j].Y + m_delta * (m_normals[k].Y + m_normals[k].X * num))));
		m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_delta * (m_normals[j].X + m_normals[j].Y * num)),
			Round(m_srcPoly[j].Y + m_delta * (m_normals[j].Y - m_normals[j].X * num))));
	}

	internal void DoMiter(int j, int k, double r) {
		var num = m_delta / r;
		m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + (m_normals[k].X + m_normals[j].X) * num),
			Round(m_srcPoly[j].Y + (m_normals[k].Y + m_normals[j].Y) * num)));
	}

	internal void DoRound(int j, int k) {
		var num1 = Math.Max(
			(int)Round(m_StepsPerRad *
			           Math.Abs(Math.Atan2(m_sinA, m_normals[k].X * m_normals[j].X + m_normals[k].Y * m_normals[j].Y))),
			1);
		var num2 = m_normals[k].X;
		var num3 = m_normals[k].Y;
		for (var index = 0; index < num1; ++index) {
			m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + num2 * m_delta),
				Round(m_srcPoly[j].Y + num3 * m_delta)));
			var num4 = num2;
			num2 = num2 * m_cos - m_sin * num3;
			num3 = num4 * m_sin + num3 * m_cos;
		}

		m_destPoly.Add(new IntPoint(Round(m_srcPoly[j].X + m_normals[j].X * m_delta),
			Round(m_srcPoly[j].Y + m_normals[j].Y * m_delta)));
	}
}