using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet;

public static class Primitives {
	private static double splitter;
	private static double epsilon;
	private static double ccwerrboundA;
	private static double iccerrboundA;

	public static void ExactInit() {
		var flag = true;
		var num1 = 0.5;
		epsilon = 1.0;
		splitter = 1.0;
		var num2 = 1.0;
		double num3;
		do {
			num3 = num2;
			epsilon *= num1;
			if (flag)
				splitter *= 2.0;
			flag = !flag;
			num2 = 1.0 + epsilon;
		} while (num2 != 1.0 && num2 != num3);

		++splitter;
		ccwerrboundA = (3.0 + 16.0 * epsilon) * epsilon;
		iccerrboundA = (10.0 + 96.0 * epsilon) * epsilon;
	}

	public static double CounterClockwise(Point pa, Point pb, Point pc) {
		++Statistic.CounterClockwiseCount;
		var num1 = (pa.x - pc.x) * (pb.y - pc.y);
		var num2 = (pa.y - pc.y) * (pb.x - pc.x);
		var num3 = num1 - num2;
		if (Behavior.NoExact)
			return num3;
		double num4;
		if (num1 > 0.0) {
			if (num2 <= 0.0)
				return num3;
			num4 = num1 + num2;
		} else {
			if (num1 >= 0.0 || num2 >= 0.0)
				return num3;
			num4 = -num1 - num2;
		}

		var num5 = ccwerrboundA * num4;
		return num3 >= num5 || -num3 >= num5 ? num3 : (double)CounterClockwiseDecimal(pa, pb, pc);
	}

	private static decimal CounterClockwiseDecimal(Point pa, Point pb, Point pc) {
		++Statistic.CounterClockwiseCountDecimal;
		var num1 = ((decimal)pa.x - (decimal)pc.x) * ((decimal)pb.y - (decimal)pc.y);
		var num2 = ((decimal)pa.y - (decimal)pc.y) * ((decimal)pb.x - (decimal)pc.x);
		var num3 = num1 - num2;
		decimal num4;
		if (num1 > 0.0M) {
			if (num2 <= 0.0M)
				return num3;
			num4 = num1 + num2;
		} else if (num1 < 0.0M && !(num2 >= 0.0M))
			num4 = -num1 - num2;

		return num3;
	}

	public static double InCircle(Point pa, Point pb, Point pc, Point pd) {
		++Statistic.InCircleCount;
		var num1 = pa.x - pd.x;
		var num2 = pb.x - pd.x;
		var num3 = pc.x - pd.x;
		var num4 = pa.y - pd.y;
		var num5 = pb.y - pd.y;
		var num6 = pc.y - pd.y;
		var num7 = num2 * num6;
		var num8 = num3 * num5;
		var num9 = num1 * num1 + num4 * num4;
		var num10 = num3 * num4;
		var num11 = num1 * num6;
		var num12 = num2 * num2 + num5 * num5;
		var num13 = num1 * num5;
		var num14 = num2 * num4;
		var num15 = num3 * num3 + num6 * num6;
		var num16 = num9 * (num7 - num8) + num12 * (num10 - num11) + num15 * (num13 - num14);
		if (Behavior.NoExact)
			return num16;
		var num17 = (Math.Abs(num7) + Math.Abs(num8)) * num9 + (Math.Abs(num10) + Math.Abs(num11)) * num12 +
		            (Math.Abs(num13) + Math.Abs(num14)) * num15;
		var num18 = iccerrboundA * num17;
		return num16 > num18 || -num16 > num18 ? num16 : (double)InCircleDecimal(pa, pb, pc, pd);
	}

	private static decimal InCircleDecimal(Point pa, Point pb, Point pc, Point pd) {
		++Statistic.InCircleCountDecimal;
		var num1 = (decimal)pa.x - (decimal)pd.x;
		var num2 = (decimal)pb.x - (decimal)pd.x;
		var num3 = (decimal)pc.x - (decimal)pd.x;
		var num4 = (decimal)pa.y - (decimal)pd.y;
		var num5 = (decimal)pb.y - (decimal)pd.y;
		var num6 = (decimal)pc.y - (decimal)pd.y;
		var num7 = num2 * num6;
		var num8 = num3 * num5;
		var num9 = num1 * num1 + num4 * num4;
		var num10 = num3 * num4;
		var num11 = num1 * num6;
		var num12 = num2 * num2 + num5 * num5;
		var num13 = num1 * num5;
		var num14 = num2 * num4;
		var num15 = num3 * num3 + num6 * num6;
		return num9 * (num7 - num8) + num12 * (num10 - num11) + num15 * (num13 - num14);
	}

	public static double NonRegular(Point pa, Point pb, Point pc, Point pd) {
		return InCircle(pa, pb, pc, pd);
	}

	public static Point FindCircumcenter(
		Point torg,
		Point tdest,
		Point tapex,
		ref double xi,
		ref double eta,
		double offconstant) {
		++Statistic.CircumcenterCount;
		var num1 = tdest.x - torg.x;
		var num2 = tdest.y - torg.y;
		var num3 = tapex.x - torg.x;
		var num4 = tapex.y - torg.y;
		var num5 = num1 * num1 + num2 * num2;
		var num6 = num3 * num3 + num4 * num4;
		var num7 = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
		double num8;
		if (Behavior.NoExact)
			num8 = 0.5 / (num1 * num4 - num3 * num2);
		else {
			num8 = 0.5 / CounterClockwise(tdest, tapex, torg);
			--Statistic.CounterClockwiseCount;
		}

		var num9 = (num4 * num5 - num2 * num6) * num8;
		var num10 = (num1 * num6 - num3 * num5) * num8;
		if (num5 < num6 && num5 < num7) {
			if (offconstant > 0.0) {
				var num11 = 0.5 * num1 - offconstant * num2;
				var num12 = 0.5 * num2 + offconstant * num1;
				if (num11 * num11 + num12 * num12 < num9 * num9 + num10 * num10) {
					num9 = num11;
					num10 = num12;
				}
			}
		} else if (num6 < num7) {
			if (offconstant > 0.0) {
				var num13 = 0.5 * num3 + offconstant * num4;
				var num14 = 0.5 * num4 - offconstant * num3;
				if (num13 * num13 + num14 * num14 < num9 * num9 + num10 * num10) {
					num9 = num13;
					num10 = num14;
				}
			}
		} else if (offconstant > 0.0) {
			var num15 = 0.5 * (tapex.x - tdest.x) - offconstant * (tapex.y - tdest.y);
			var num16 = 0.5 * (tapex.y - tdest.y) + offconstant * (tapex.x - tdest.x);
			if (num15 * num15 + num16 * num16 < (num9 - num1) * (num9 - num1) + (num10 - num2) * (num10 - num2)) {
				num9 = num1 + num15;
				num10 = num2 + num16;
			}
		}

		xi = (num4 * num9 - num3 * num10) * (2.0 * num8);
		eta = (num1 * num10 - num2 * num9) * (2.0 * num8);
		return new Point(torg.x + num9, torg.y + num10);
	}

	public static Point FindCircumcenter(
		Point torg,
		Point tdest,
		Point tapex,
		ref double xi,
		ref double eta) {
		++Statistic.CircumcenterCount;
		var num1 = tdest.x - torg.x;
		var num2 = tdest.y - torg.y;
		var num3 = tapex.x - torg.x;
		var num4 = tapex.y - torg.y;
		var num5 = num1 * num1 + num2 * num2;
		var num6 = num3 * num3 + num4 * num4;
		double num7;
		if (Behavior.NoExact)
			num7 = 0.5 / (num1 * num4 - num3 * num2);
		else {
			num7 = 0.5 / CounterClockwise(tdest, tapex, torg);
			--Statistic.CounterClockwiseCount;
		}

		var num8 = (num4 * num5 - num2 * num6) * num7;
		var num9 = (num1 * num6 - num3 * num5) * num7;
		xi = (num4 * num8 - num3 * num9) * (2.0 * num7);
		eta = (num1 * num9 - num2 * num8) * (2.0 * num7);
		return new Point(torg.x + num8, torg.y + num9);
	}
}