using UnityEngine;

namespace Scripts.Utility;

public static class EasyUtility {
	private const float PI = 3.14159274f;
	private const float HALFPI = 1.57079637f;

	public static float Interpolate(float p, Functions function) {
		switch (function) {
			case Functions.QuadraticEaseIn:
				return QuadraticEaseIn(p);
			case Functions.QuadraticEaseOut:
				return QuadraticEaseOut(p);
			case Functions.QuadraticEaseInOut:
				return QuadraticEaseInOut(p);
			case Functions.CubicEaseIn:
				return CubicEaseIn(p);
			case Functions.CubicEaseOut:
				return CubicEaseOut(p);
			case Functions.CubicEaseInOut:
				return CubicEaseInOut(p);
			case Functions.QuarticEaseIn:
				return QuarticEaseIn(p);
			case Functions.QuarticEaseOut:
				return QuarticEaseOut(p);
			case Functions.QuarticEaseInOut:
				return QuarticEaseInOut(p);
			case Functions.QuinticEaseIn:
				return QuinticEaseIn(p);
			case Functions.QuinticEaseOut:
				return QuinticEaseOut(p);
			case Functions.QuinticEaseInOut:
				return QuinticEaseInOut(p);
			case Functions.SineEaseIn:
				return SineEaseIn(p);
			case Functions.SineEaseOut:
				return SineEaseOut(p);
			case Functions.SineEaseInOut:
				return SineEaseInOut(p);
			case Functions.CircularEaseIn:
				return CircularEaseIn(p);
			case Functions.CircularEaseOut:
				return CircularEaseOut(p);
			case Functions.CircularEaseInOut:
				return CircularEaseInOut(p);
			case Functions.ExponentialEaseIn:
				return ExponentialEaseIn(p);
			case Functions.ExponentialEaseOut:
				return ExponentialEaseOut(p);
			case Functions.ExponentialEaseInOut:
				return ExponentialEaseInOut(p);
			case Functions.ElasticEaseIn:
				return ElasticEaseIn(p);
			case Functions.ElasticEaseOut:
				return ElasticEaseOut(p);
			case Functions.ElasticEaseInOut:
				return ElasticEaseInOut(p);
			case Functions.BackEaseIn:
				return BackEaseIn(p);
			case Functions.BackEaseOut:
				return BackEaseOut(p);
			case Functions.BackEaseInOut:
				return BackEaseInOut(p);
			case Functions.BounceEaseIn:
				return BounceEaseIn(p);
			case Functions.BounceEaseOut:
				return BounceEaseOut(p);
			case Functions.BounceEaseInOut:
				return BounceEaseInOut(p);
			default:
				return Linear(p);
		}
	}

	public static float Linear(float p) {
		return p;
	}

	public static float QuadraticEaseIn(float p) {
		return p * p;
	}

	public static float QuadraticEaseOut(float p) {
		return (float)-(p * (p - 2.0));
	}

	public static float QuadraticEaseInOut(float p) {
		return p < 0.5 ? 2f * p * p : (float)(-2.0 * p * p + 4.0 * p - 1.0);
	}

	public static float CubicEaseIn(float p) {
		return p * p * p;
	}

	public static float CubicEaseOut(float p) {
		var num = p - 1f;
		return (float)(num * (double)num * num + 1.0);
	}

	public static float CubicEaseInOut(float p) {
		if (p < 0.5)
			return 4f * p * p * p;
		var num = (float)(2.0 * p - 2.0);
		return (float)(0.5 * num * num * num + 1.0);
	}

	public static float QuarticEaseIn(float p) {
		return p * p * p * p;
	}

	public static float QuarticEaseOut(float p) {
		var num = p - 1f;
		return (float)(num * (double)num * num * (1.0 - p) + 1.0);
	}

	public static float QuarticEaseInOut(float p) {
		if (p < 0.5)
			return 8f * p * p * p * p;
		var num = p - 1f;
		return (float)(-8.0 * num * num * num * num + 1.0);
	}

	public static float QuinticEaseIn(float p) {
		return p * p * p * p * p;
	}

	public static float QuinticEaseOut(float p) {
		var num = p - 1f;
		return (float)(num * (double)num * num * num * num + 1.0);
	}

	public static float QuinticEaseInOut(float p) {
		if (p < 0.5)
			return 16f * p * p * p * p * p;
		var num = (float)(2.0 * p - 2.0);
		return (float)(0.5 * num * num * num * num * num + 1.0);
	}

	public static float SineEaseIn(float p) {
		return Mathf.Sin((float)((p - 1.0) * 1.5707963705062866)) + 1f;
	}

	public static float SineEaseOut(float p) {
		return Mathf.Sin(p * 1.57079637f);
	}

	public static float SineEaseInOut(float p) {
		return (float)(0.5 * (1.0 - Mathf.Cos(p * 3.14159274f)));
	}

	public static float CircularEaseIn(float p) {
		return 1f - Mathf.Sqrt((float)(1.0 - p * (double)p));
	}

	public static float CircularEaseOut(float p) {
		return Mathf.Sqrt((2f - p) * p);
	}

	public static float CircularEaseInOut(float p) {
		return p < 0.5
			? (float)(0.5 * (1.0 - Mathf.Sqrt((float)(1.0 - 4.0 * (p * (double)p)))))
			: (float)(0.5 * (Mathf.Sqrt((float)(-(2.0 * p - 3.0) * (2.0 * p - 1.0))) + 1.0));
	}

	public static float ExponentialEaseIn(float p) {
		return p == 0.0 ? p : Mathf.Pow(2f, (float)(10.0 * (p - 1.0)));
	}

	public static float ExponentialEaseOut(float p) {
		return p == 1.0 ? p : 1f - Mathf.Pow(2f, -10f * p);
	}

	public static float ExponentialEaseInOut(float p) {
		if (p == 0.0 || p == 1.0)
			return p;
		return p < 0.5
			? 0.5f * Mathf.Pow(2f, (float)(20.0 * p - 10.0))
			: (float)(-0.5 * Mathf.Pow(2f, (float)(-20.0 * p + 10.0)) + 1.0);
	}

	public static float ElasticEaseIn(float p) {
		return Mathf.Sin(20.4203529f * p) * Mathf.Pow(2f, (float)(10.0 * (p - 1.0)));
	}

	public static float ElasticEaseOut(float p) {
		return (float)(Mathf.Sin((float)(-20.420352935791016 * (p + 1.0))) * (double)Mathf.Pow(2f, -10f * p) + 1.0);
	}

	public static float ElasticEaseInOut(float p) {
		return p < 0.5
			? 0.5f * Mathf.Sin((float)(20.420352935791016 * (2.0 * p))) * Mathf.Pow(2f, (float)(10.0 * (2.0 * p - 1.0)))
			: (float)(0.5 * (Mathf.Sin((float)(-20.420352935791016 * (2.0 * p - 1.0 + 1.0))) *
				(double)Mathf.Pow(2f, (float)(-10.0 * (2.0 * p - 1.0))) + 2.0));
	}

	public static float BackEaseIn(float p) {
		return (float)(p * (double)p * p - p * (double)Mathf.Sin(p * 3.14159274f));
	}

	public static float BackEaseOut(float p) {
		var num = 1f - p;
		return (float)(1.0 - (num * (double)num * num - num * (double)Mathf.Sin(num * 3.14159274f)));
	}

	public static float BackEaseInOut(float p) {
		if (p < 0.5) {
			var num = 2f * p;
			return (float)(0.5 * (num * (double)num * num - num * (double)Mathf.Sin(num * 3.14159274f)));
		}

		var num1 = (float)(1.0 - (2.0 * p - 1.0));
		return (float)(0.5 * (1.0 - (num1 * (double)num1 * num1 - num1 * (double)Mathf.Sin(num1 * 3.14159274f))) + 0.5);
	}

	public static float BounceEaseIn(float p) {
		return 1f - BounceEaseOut(1f - p);
	}

	public static float BounceEaseOut(float p) {
		if (p < 0.36363637447357178)
			return (float)(121.0 * p * p / 16.0);
		if (p < 0.72727274894714355)
			return (float)(9.0749998092651367 * p * p - 9.8999996185302734 * p + 3.4000000953674316);
		return p < 0.89999997615814209
			? (float)(12.066481590270996 * p * p - 19.635457992553711 * p + 8.89806079864502)
			: (float)(10.800000190734863 * p * p - 20.520000457763672 * p + 10.720000267028809);
	}

	public static float BounceEaseInOut(float p) {
		return p < 0.5 ? 0.5f * BounceEaseIn(p * 2f) : (float)(0.5 * BounceEaseOut((float)(p * 2.0 - 1.0)) + 0.5);
	}

	public enum Functions {
		Linear,
		QuadraticEaseIn,
		QuadraticEaseOut,
		QuadraticEaseInOut,
		CubicEaseIn,
		CubicEaseOut,
		CubicEaseInOut,
		QuarticEaseIn,
		QuarticEaseOut,
		QuarticEaseInOut,
		QuinticEaseIn,
		QuinticEaseOut,
		QuinticEaseInOut,
		SineEaseIn,
		SineEaseOut,
		SineEaseInOut,
		CircularEaseIn,
		CircularEaseOut,
		CircularEaseInOut,
		ExponentialEaseIn,
		ExponentialEaseOut,
		ExponentialEaseInOut,
		ElasticEaseIn,
		ElasticEaseOut,
		ElasticEaseInOut,
		BackEaseIn,
		BackEaseOut,
		BackEaseInOut,
		BounceEaseIn,
		BounceEaseOut,
		BounceEaseInOut
	}
}